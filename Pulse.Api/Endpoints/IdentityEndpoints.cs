using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.DTOs;
using Pulse.Identity.Queries;
using Pulse.Shared.Results;

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

            return Results.Created($"/api/identity/{result.Id}", ApiResponse<UserResponse>.Success(new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            }, "User registered successfully."));
        })
        .Produces<ApiResponse<UserResponse>>(201)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(500)
        .WithName("RegisterUser")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapPost("/login", async (LoginUserRequest request, IMediator mediator) =>
        {
            var response = await mediator.Send(new LoginUserCommand(request.Email, request.Password));

            return Results.Ok(ApiResponse<AuthResponse>.Success(response, "Login successful."));
        })
        .Produces<ApiResponse<AuthResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("LoginUser")
        .WithTags("Identity")
        .WithOpenApi();

        group.MapGet("/get-user/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCurrentUserQuery(userId));

            return Results.Ok(ApiResponse<UserResponse>.Success(new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            }, "User retrieved successfully."));
        })
        .Produces<ApiResponse<UserResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("GetCurrentUser")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/update-user/{userId:guid}", async (Guid userId, UpdateUserRequest request, IMediator mediator) =>
        {
            var updated = new User
            {
                FullName = request.FullName,
                Email = request.Email
            };

            var result = await mediator.Send(new UpdateUserCommand(userId, updated));

            return Results.Ok(ApiResponse<UserResponse>.Success(new UserResponse
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                Plan = result.Plan,
                CreatedAt = result.CreatedAt,
                IsActive = result.IsActive
            }, "User updated successfully."));
        })
        .Produces<ApiResponse<UserResponse>>(200)
        .Produces<ApiResponse<object>>(400)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("UpdateUser")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPatch("/change-password/{userId:guid}", async (Guid userId, ChangePasswordRequest request, IMediator mediator) =>
        {
            await mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));
            return Results.Ok(ApiResponse<object>.Success(null, "Password changed successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("ChangePassword")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/delete-account/{userId:guid}", async (Guid userId, IMediator mediator) =>
        {
            await mediator.Send(new DeleteAccountCommand(userId));
            return Results.Ok(ApiResponse<object>.Success(null, "Account deleted successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(404)
        .Produces<ApiResponse<object>>(500)
        .WithName("DeleteAccount")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/refresh-token", async (RefreshTokenRequest request, IMediator mediator) =>
        {
            var response = await mediator.Send(new RefreshTokenCommand(request.UserId, request.RefreshToken));
            return Results.Ok(ApiResponse<AuthResponse>.Success(response, "Token refreshed successfully."));
        })
        .Produces<ApiResponse<AuthResponse>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("RefreshToken")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/logout", async (RevokeRefreshTokenRequest request, IMediator mediator) =>
        {
            await mediator.Send(new RevokeRefreshTokenCommand(request.UserId, request.RefreshToken));
            return Results.Ok(ApiResponse<object>.Success(null, "Logged out successfully."));
        })
        .Produces<ApiResponse<object>>(200)
        .Produces<ApiResponse<object>>(401)
        .Produces<ApiResponse<object>>(500)
        .WithName("Logout")
        .WithTags("Identity")
        .WithOpenApi()
        .RequireAuthorization();
    }
}