param(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString
)

$env:ConnectionStrings__DefaultConnection = $ConnectionString

$contexts = @(
    "BillingDbContext",
    "IdentityDbContext",
    "MonitoringDbContext",
    "NotificationsDbContext",
    "ObservabilityDbContext",
    "StatusPagesDbContext"
)

foreach ($context in $contexts) {
    Write-Host "Applying migrations for $context..."
    dotnet ef database update --project Pulse.Infrastructure --startup-project Pulse.Api --context $context
}

Remove-Item Env:\ConnectionStrings__DefaultConnection