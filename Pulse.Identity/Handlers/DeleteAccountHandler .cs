using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly IAuthService _authService;

    public DeleteAccountHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        await _authService.DeleteAccountAsync(request.UserId);
    }
}