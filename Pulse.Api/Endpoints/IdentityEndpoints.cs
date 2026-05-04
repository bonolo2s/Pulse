using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.DTOs;
using Pulse.Identity.Queries;

namespace Pulse.Api.Endpoints;

public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/identity");

        group.MapPost("/register", async (RegisterUserRequest request, IMediator mediator) =>
        {
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email
            };

            var result = await mediator.Send(new RegisterUserCommand(user, request.Password));

            return Results.Created($"/api/identity/{result.Id}", new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            });
        })
        .WithName("RegisterUser")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapPost("/login", async (LoginUserRequest request, IMediator mediator) =>
        {
            var response = await mediator.Send(new LoginUserCommand(request.Email, request.Password));

            return Results.Ok(response);
        })
        .WithName("LoginUser")
        .WithTags("Identity")
        .WithOpenApi();


        group.MapGet("/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCurrentUserQuery(userId));

            return Results.Ok(new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            });
        })
        .WithName("GetCurrentUser")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapPut("/{userId:guid}", async (Guid userId, UpdateUserRequest request, IMediator mediator) =>
        {
            var updated = new User
            {
                FullName = request.FullName,
                Email = request.Email
            };

            var result = await mediator.Send(new UpdateUserCommand(userId, updated));

            return Results.Ok(new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            });
        })
        .WithName("UpdateUser")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapPatch("/{userId:guid}/password", async (Guid userId, ChangePasswordRequest request, IMediator mediator) =>
        {
            await mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));
            return Results.NoContent();
        })
        .WithName("ChangePassword")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapDelete("/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            await mediator.Send(new DeleteAccountCommand(userId));
            return Results.NoContent();
        })
        .WithName("DeleteAccount")
        .WithTags("Identity")
        .WithOpenApi();
    }
}