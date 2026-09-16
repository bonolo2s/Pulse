FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Pulse.Api/Pulse.Api.csproj Pulse.Api/
COPY Pulse.Monitoring/Pulse.Monitoring.csproj Pulse.Monitoring/
COPY Pulse.Observability/Pulse.Observability.csproj Pulse.Observability/
COPY Pulse.Notifications/Pulse.Notifications.csproj Pulse.Notifications/
COPY Pulse.StatusPages/Pulse.StatusPages.csproj Pulse.StatusPages/
COPY Pulse.Billing/Pulse.Billing.csproj Pulse.Billing/
COPY Pulse.Identity/Pulse.Identity.csproj Pulse.Identity/
COPY Pulse.Infrastructure/Pulse.Infrastructure.csproj Pulse.Infrastructure/

RUN dotnet restore Pulse.Api/Pulse.Api.csproj

COPY . .
RUN dotnet publish Pulse.Api/Pulse.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Pulse.Api.dll"]