using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.DTOs;
using Pulse.StatusPages.Queries;

namespace Pulse.Api.Endpoints;

public static class StatusPagesEndpoints
{
    public static void MapStatusPagesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/statuspages");

        group.MapPost("/create-status-page", async (CreateStatusPageRequest request, IMediator mediator) =>
        {
            var statusPage = new StatusPage
            {
                UserId = request.UserId,
                Name = request.Name,
                IsPublic = request.IsPublic
            };

            var result = await mediator.Send(new CreateStatusPageCommand(statusPage));

            return Results.Created($"/api/statuspages/{result.Id}", new StatusPageResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Slug = result.Slug,
                IsPublic = result.IsPublic,
                CreatedAt = result.CreatedAt
            });
        })
        .WithName("CreateStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/update-status-page/{id:guid}", async (Guid id, UpdateStatusPageRequest request, IMediator mediator) =>
        {
            var statusPage = new StatusPage
            {
                Name = request.Name,
                IsPublic = request.IsPublic
            };

            var result = await mediator.Send(new UpdateStatusPageCommand(id, statusPage));

            return Results.Ok(new StatusPageResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Slug = result.Slug,
                IsPublic = result.IsPublic,
                CreatedAt = result.CreatedAt
            });
        })
        .WithName("UpdateStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/delete-status-page/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteStatusPageCommand(id));
            return Results.NoContent();
        })
        .WithName("DeleteStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/add-endpoint", async (AddEndpointToStatusPageRequest request, IMediator mediator) =>
        {
            var statusPageEndpoint = new StatusPageEndpoint
            {
                StatusPageId = request.StatusPageId,
                EndpointId = request.EndpointId,
                DisplayName = request.DisplayName,
                DisplayOrder = request.DisplayOrder
            };

            var result = await mediator.Send(new AddEndpointToStatusPageCommand(statusPageEndpoint));

            return Results.Created($"/api/statuspages/endpoints/{result.Id}", new StatusPageEndpointResponse
            {
                Id = result.Id,
                StatusPageId = result.StatusPageId,
                EndpointId = result.EndpointId,
                DisplayName = result.DisplayName,
                DisplayOrder = result.DisplayOrder
            });
        })
        .WithName("AddEndpointToStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/remove-endpoint/{statusPageEndpointId:guid}", async (Guid statusPageEndpointId, IMediator mediator) =>
        {
            await mediator.Send(new RemoveEndpointFromStatusPageCommand(statusPageEndpointId));
            return Results.NoContent();
        })
        .WithName("RemoveEndpointFromStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/public/{slug}", async (string slug, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetPublicStatusPageQuery(slug));

            return Results.Ok(new StatusPageResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Slug = result.Slug,
                IsPublic = result.IsPublic,
                CreatedAt = result.CreatedAt,
                Endpoints = result.Endpoints.Select(e => new StatusPageEndpointResponse
                {
                    Id = e.Id,
                    StatusPageId = e.StatusPageId,
                    EndpointId = e.EndpointId,
                    DisplayName = e.DisplayName,
                    DisplayOrder = e.DisplayOrder
                })
            });
        })
        .WithName("GetPublicStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi();

        group.MapGet("/get-private/{statusPageId:guid}", async (Guid statusPageId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetPrivateStatusPageQuery(statusPageId));

            return Results.Ok(new StatusPageResponse
            {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Slug = result.Slug,
                IsPublic = result.IsPublic,
                CreatedAt = result.CreatedAt,
                Endpoints = result.Endpoints.Select(e => new StatusPageEndpointResponse
                {
                    Id = e.Id,
                    StatusPageId = e.StatusPageId,
                    EndpointId = e.EndpointId,
                    DisplayName = e.DisplayName,
                    DisplayOrder = e.DisplayOrder
                })
            });
        })
        .WithName("GetPrivateStatusPage")
        .WithTags("StatusPages")
        .WithOpenApi()
        .RequireAuthorization();
    }
}