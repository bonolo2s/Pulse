using MediatR;
using Pulse.Observability.Entities;

namespace Pulse.Observability.Commands;

public record RecordCheckResultCommand(CheckResult Result) : IRequest;