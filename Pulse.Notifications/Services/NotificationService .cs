using Microsoft.EntityFrameworkCore;
using Pulse.Notifications.DataAccess;
using Pulse.Notifications.Entities;
using Pulse.Notifications.Interfaces;
using Pulse.Shared.DTOs;
using Pulse.Shared.Enums;
using Pulse.Shared.Interfaces;

namespace Pulse.Notifications.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationsDbContext _context;
    private readonly IEmailSender _emailSender;

    public NotificationService(NotificationsDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public async Task TriggerAlertAsync(HealthCheckResult result)
    {
        var rules = await _context.AlertRules
            .Where(r => r.IsActive && r.UserId == result.UserId)
            .ToListAsync();

        foreach (var rule in rules)
        {
            var hasUnacknowledged = await _context.AlertLogs
                .AnyAsync(l => l.EndpointId == result.EndpointId
                    && l.AlertRuleId == rule.Id
                    && !l.IsAcknowledged);

            if (hasUnacknowledged)
            {
                Console.WriteLine($"[TriggerAlertAsync] Skipped — unacknowledged alert exists for EndpointId={result.EndpointId}, RuleId={rule.Id}");
                continue;
            }


            var type = result.Status switch
            {
                EndpointStatus.Downtime => AlertType.Downtime,
                EndpointStatus.Degraded => AlertType.Degraded,
                EndpointStatus.Operational => AlertType.Recovery,
                _ => AlertType.Downtime
            };

            var log = new AlertLog
            {
                Id = Guid.NewGuid(),
                AlertRuleId = rule.Id,
                EndpointId = result.EndpointId,
                Channel = rule.Channel,
                Type = type,
                Message = $"Endpoint {result.EndpointId} status: {result.Status}",
                Delivered = false,
                IsAcknowledged = false,
                SentAt = DateTime.UtcNow
            };

            _context.AlertLogs.Add(log);
            await _context.SaveChangesAsync();

            switch (rule.Channel)
            {
                case AlertChannel.Email:
                    await DispatchEmailNotificationAsync(log);
                    break;
                case AlertChannel.Slack:
                    await DispatchSlackNotificationAsync(log);
                    break;
                case AlertChannel.Sms:
                    await DispatchSmsNotificationAsync(log);
                    break;
            }

            log.Delivered = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DispatchEmailNotificationAsync(AlertLog log)
    {
        Console.WriteLine("********Dispatch email notification got hit");
        var rule = await _context.AlertRules.FindAsync(log.AlertRuleId);
        await _emailSender.SendAsync(
            to: rule!.Destination,
            subject: $"Pulse Alert: {log.Type}",
            body: log.Message
        );
        log.Delivered = true;
        await _context.SaveChangesAsync();
    }

    public async Task DispatchSmsNotificationAsync(AlertLog log)
    {
        // AWS SNS SMS integration — wired in Infrastructure
        log.Delivered = true;
        await _context.SaveChangesAsync();
    }

    public async Task DispatchSlackNotificationAsync(AlertLog log)
    {
        // Slack webhook integration — wired in Infrastructure
        log.Delivered = true;
        await _context.SaveChangesAsync();
    }

    public async Task ResolveAlertAsync(Guid endpointId)
    {
        var activeRuleIds = await _context.AlertLogs
            .Where(l => l.EndpointId == endpointId && !l.IsAcknowledged)
            .Select(l => l.AlertRuleId)
            .Distinct()
            .ToListAsync();

        var rules = await _context.AlertRules
            .Where(r => activeRuleIds.Contains(r.Id) && r.IsActive)
            .ToListAsync();

        foreach (var rule in rules)
        {
            var log = new AlertLog
            {
                Id = Guid.NewGuid(),
                AlertRuleId = rule.Id,
                EndpointId = endpointId,
                Channel = rule.Channel,
                Type = AlertType.Recovery,
                Message = $"Endpoint {endpointId} has recovered.",
                Delivered = false,
                IsAcknowledged = false,
                SentAt = DateTime.UtcNow
            };

            _context.AlertLogs.Add(log);
            await _context.SaveChangesAsync();

            switch (rule.Channel)
            {
                case AlertChannel.Email:
                    await DispatchEmailNotificationAsync(log);
                    break;
                case AlertChannel.Slack:
                    await DispatchSlackNotificationAsync(log);
                    break;
                case AlertChannel.Sms:
                    await DispatchSmsNotificationAsync(log);
                    break;
            }
        }
    }

    public async Task ManageAlertRulesAsync(AlertRule rule)
    {
        var existing = await _context.AlertRules.FindAsync(rule.Id);

        if (existing is null)
        {
            rule.Id = Guid.NewGuid();
            rule.CreatedAt = DateTime.UtcNow;
            _context.AlertRules.Add(rule);
        }
        else
        {
            existing.Channel = rule.Channel;
            existing.Destination = rule.Destination;
            existing.IsActive = rule.IsActive;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AlertRule>> GetAlertSettingsAsync(Guid userId)
    {
        return await _context.AlertRules
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AlertLog>> GetAlertLogsAsync(Guid endpointId)
    {
        return await _context.AlertLogs
            .Where(l => l.EndpointId == endpointId)
            .OrderByDescending(l => l.SentAt)
            .ToListAsync();
    }

    public async Task AcknowledgeAlertAsync(Guid alertLogId)
    {
        var log = await _context.AlertLogs.FindAsync(alertLogId)
            ?? throw new KeyNotFoundException($"Alert log {alertLogId} not found.");

        log.IsAcknowledged = true;
        log.AcknowledgedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}