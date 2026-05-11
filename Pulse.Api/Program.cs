using Pulse.Api.Endpoints;
using Pulse.Api.Middleware;
using Pulse.Billing;
using Pulse.Identity;
using Pulse.Infrastructure;
using Pulse.Monitoring;
using Pulse.Notifications;
using Pulse.Observability;
using Pulse.Shared.Results;
using Pulse.StatusPages;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentity(
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    builder.Configuration);
builder.Services.AddMonitoring(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddObservability(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddNotifications(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddStatusPages(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddBilling(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePages(async context =>
{
    var code = context.HttpContext.Response.StatusCode;
    if (code is 401 or 403 or 404)
    {
        context.HttpContext.Response.ContentType = "application/json";
        var message = code switch
        {
            401 => "You are not authenticated. Please log in.",
            403 => "You do not have permission to access this resource.",
            404 => "The requested resource was not found.",
            _ => "An unexpected error occurred."
        };
        var response = ApiResponse<object>.Failure(message);
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityEndpoints();
app.MapMonitoringEndpoints();
app.MapObservabilityEndpoints();
app.MapNotificationsEndpoints();
app.MapStatusPagesEndpoints();
app.MapBillingEndpoints();

app.Run();