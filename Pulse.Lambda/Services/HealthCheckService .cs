using Pulse.Lambda.Interfaces;
using Pulse.Shared.DTOs;
using Pulse.Shared.Enums;
using System.Diagnostics;
using System.Net;
using System.Runtime.Intrinsics.X86;

namespace Pulse.Lambda.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly HttpClient _httpClient;
    private readonly LatencyTracker _latencyTracker;
    private readonly SslInspector _sslInspector;

    public HealthCheckService(
        HttpClient httpClient,
        LatencyTracker latencyTracker,
        SslInspector sslInspector)
    {
        _httpClient = httpClient;
        _latencyTracker = latencyTracker;
        _sslInspector = sslInspector;
    }

    private async Task<(bool IsUp, int StatusCode, long LatencyMs)> PingAsync(string url)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync(url);
            sw.Stop();
            var isUp = response.StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices;
            return (isUp, (int)response.StatusCode, sw.ElapsedMilliseconds);
        }
        catch
        {
            return (false, 0, 0);
        }
    }

    public async Task<HealthCheckResult> RunHealthCheckAsync(Guid endpointId, string url)
    {
        var (isUp, statusCode, latencyMs) = await PingAsync(url);
        //var latencyMs = await MeasureLatencyAsync(url);
        var (isExpiringSoon, expiryDate, issuer) = await _sslInspector.InspectAsync(url);

        var status = isUp
            ? latencyMs > 3000 ? EndpointStatus.Degraded : EndpointStatus.Operational
            : EndpointStatus.Downtime;

        return new HealthCheckResult
        {
            EndpointId = endpointId,
            Status = status,
            StatusCode = statusCode,
            LatencyMs = latencyMs,
            SslIssuer = issuer,
            SslExpiresAt = expiryDate,
            SslDaysRemaining = (int)(expiryDate - DateTime.UtcNow).TotalDays,
            CheckedAt = DateTime.UtcNow
        };
    }
}