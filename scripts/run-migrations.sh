#!/bin/sh
set -e

CONTEXTS="BillingDbContext IdentityDbContext MonitoringDbContext NotificationsDbContext ObservabilityDbContext StatusPagesDbContext"

for context in $CONTEXTS; do
    echo "Applying migrations for $context..."
    dotnet ef database update --project Pulse.Infrastructure --startup-project Pulse.Api --context "$context"
done