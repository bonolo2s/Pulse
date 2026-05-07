using Pulse.Api.Endpoints;
using Pulse.Billing;
using Pulse.Identity;
using Pulse.Monitoring;
using Pulse.Notifications;
using Pulse.Observability;
using Pulse.StatusPages;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddIdentity(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddMonitoring(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddObservability(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddNotifications(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddStatusPages(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddBilling(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.MapIdentityEndpoints();
app.MapMonitoringEndpoints();
app.MapObservabilityEndpoints();
app.MapNotificationsEndpoints();
app.MapStatusPagesEndpoints();
app.MapBillingEndpoints();

app.Run();