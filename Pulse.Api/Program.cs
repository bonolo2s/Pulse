using Pulse.Api.Endpoints;
using Pulse.Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMonitoring(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.MapMonitoringEndpoints();

app.Run();