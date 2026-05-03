using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Monitoring.Handlers;

public class AddEndpointHandler : IRequestHandler<AddEndpointCommand, MonitoredEndpoint>
{
    private readonly IMonitoringService _monitoringService;
    private readonly IBillingValidator _billingValidator;

    public AddEndpointHandler(IMonitoringService monitoringService, IBillingValidator billingValidator)
    {
        _monitoringService = monitoringService;
        _billingValidator = billingValidator;
    }

    public async Task<MonitoredEndpoint> Handle(AddEndpointCommand request, CancellationToken cancellationToken)
    {
        var currentCount = await _monitoringService.GetEndpointCountAsync(request.Endpoint.UserId);
        await _billingValidator.ValidateEndpointLimitAsync(request.Endpoint.UserId, currentCount);
        return await _monitoringService.AddEndpointAsync(request.Endpoint);
    }
}