using MediatR;

namespace Pulse.Identity.Commands;

public record DeleteAccountCommand(Guid UserId) : IRequest;