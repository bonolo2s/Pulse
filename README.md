# Pulse — Endpoint Monitoring Platform

> Know when your APIs go down, before your users do.

## Overview

Pulse started as an internal frustration — manually checking whether services were up, getting blindsided by silent failures, and only finding out something broke when a user complained. That frustration became a platform.

Pulse monitors your endpoints around the clock, measures latency, inspects SSL certificates, tracks uptime history, and alerts your team the moment something degrades or goes down — before your users notice.

Started as my own internal dev tooling — then built with a SaaS mindset from the ground up, holding itself to the same reliability standards it monitors for.

**Free tier:** Monitor up to 3 endpoints with basic email alerts.
**Pro tier:** Unlimited endpoints, sub-minute check intervals, SSL monitoring, Slack/webhook alerts, status pages, and full uptime analytics.

---

## Core Features

- **HTTP/HTTPS Health Checks** — monitors endpoint availability and response codes on configurable intervals
- **Latency Tracking** — measures response time per check; flags degradation before it becomes downtime
- **SSL Certificate Monitoring** — inspects the certificate returned during the TLS handshake; alerts before expiry
- **Instant Alerts** — notifies via email, Slack, or webhook the moment an endpoint goes down or degrades
- **Uptime & Latency History** — 30, 60, and 90-day windows with trend analytics
- **Status Pages** — public or private pages showing live system status per service
- **Real-Time Dashboard** — total endpoints, operational count, active downtime, and average uptime at a glance
- **Tier Enforcement** — free vs pro limits enforced at the API Gateway layer

---

## Why Minimal APIs Over Controllers

- **Architecture fit** — endpoints are defined directly inside feature folders, keeping each feature self-contained. Controllers force a separate folder that works against this structure.
- **Less ceremony** — with CQRS + MediatR already handling separation, controllers become a pointless wrapper around handler dispatch.
- **Performance** — slightly lower middleware overhead per request. Pulse needs to be fast — it's measuring latency in other systems.
- **Production standard** — .NET 9 minimal APIs are not POC tooling. They are production-grade and used at scale.

---

## Tech Stack

### Backend

| Tool | Purpose |
|---|---|
| .NET 9 Minimal APIs | Lightweight, explicit API routing |
| C# | Primary language |
| CQRS + MediatR | Clean command/query separation — read-heavy dashboard doesn't compete with write commands |
| FluentValidation | Consistent, expressive input validation |
| Entity Framework Core | ORM, migrations |
| PostgreSQL (AWS RDS) | Primary store — uptime history, incidents, users, endpoint configs. ACID-compliant, relational integrity where it matters |
| Redis (AWS ElastiCache) | Check state caching, free tier rate limiting, fast reads |

### Cloud (AWS)

| Service | Purpose |
|---|---|
| API Gateway | Routing, throttling, rate limiting per tier — free vs pro enforced here |
| ECS | Hosts the .NET 9 API |
| EventBridge | Cron scheduler — triggers health check Lambdas on configurable intervals |
| Lambda | Executes health checks — pings endpoints, measures latency, reads SSL cert |
| SNS | Alert fan-out — decouples detection from notification |
| SES | Email alert delivery |
| RDS (PostgreSQL) | Managed relational database |
| ElastiCache (Redis) | Managed cache layer |
| S3 | Log archives, uptime reports |
| CloudWatch | Structured logs, metrics, alarms, dashboards — CloudWatch-first observability |
| VPC + Subnets | Public subnet: API Gateway, Lambda. Private subnet: RDS, ElastiCache |
| Route53 | DNS — custom domain routing |
| ACM | Free managed SSL certificates for Pulse itself |
| IAM | Least privilege roles per service — no over-permissioned access |
| SAM / CDK | Infrastructure as code — repeatable, version-controlled deployments |
| GitHub Actions | CI/CD pipeline |

### Frontend

| Tool | Purpose |
|---|---|
| Next.js + TypeScript | SSR-capable, type-safe frontend |
| Tailwind CSS + ShadCN UI | Clean, lightweight components |

---

## Cloud Architecture

```
                        ┌─────────────────┐
                        │   Next.js FE     │
                        └────────┬────────┘
                                 │
                        ┌────────▼────────┐
                        │   API Gateway    │  ← Rate limiting, throttling, tier enforcement
                        └────────┬────────┘
                                 │
                        ┌────────▼────────┐
                        │  .NET 9 API      │  ← Minimal APIs, CQRS/MediatR
                        │  (ECS)           │
                        └──┬──────────┬───┘
                           │          │
              ┌────────────▼─┐    ┌───▼────────────┐
              │ PostgreSQL    │    │ Redis           │
              │ (RDS)         │    │ (ElastiCache)   │
              └──────────────┘    └────────────────┘

   ┌─────────────────────────────────────────────────┐
   │         EventBridge (Cron Scheduler)             │
   └──────────────────────┬──────────────────────────┘
                          │  triggers per check interval
                 ┌────────▼────────┐
                 │     Lambda       │  ← Pings endpoint, measures latency, reads SSL cert
                 └────────┬────────┘
                          │  on failure / degradation / SSL expiry
                 ┌────────▼────────┐
                 │      SNS         │  ← Fan-out, decoupled alerting
                 └──┬──────────┬───┘
                    │          │
              ┌─────▼──┐  ┌────▼──────┐
              │  SES    │  │  Slack /  │
              │ (Email) │  │  Webhook  │
              └─────────┘  └──────────┘

   CloudWatch  — logs, metrics, alarms across all services
   VPC         — private subnets for RDS + ElastiCache, public for API Gateway + Lambda
   IAM         — least privilege per service
   Route53     — DNS
   ACM         — SSL for Pulse itself
   S3          — log archives, reports
```

**Why this architecture:**
- Lambda for health checks means checks are isolated, stateless, and scale independently — a slow check doesn't block the API
- EventBridge gives precise scheduling without a persistent background service
- SNS decouples alert detection from delivery — adding a new alert channel requires no API changes
- Private subnets for RDS and ElastiCache mean the database is never directly exposed to the internet
- CloudWatch-first observability mirrors how production teams operate at scale

---

## Backend Folder Structure

```
Pulse/
├── Pulse.sln
│
├── Pulse.Api/                             # Entry point — minimal API endpoints, DI wiring
│   ├── Endpoints/
│   │   ├── IdentityEndpoints.cs
│   │   ├── MonitoringEndpoints.cs
│   │   ├── ObservabilityEndpoints.cs
│   │   ├── NotificationsEndpoints.cs
│   │   ├── StatusPagesEndpoints.cs
│   │   └── BillingEndpoints.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── Pulse.Identity/                        # User signup, login, and permissions
│   ├── Entities/
│   │   └── User.cs
│   ├── Commands/
│   │   ├── RegisterUserCommand.cs
│   │   └── LoginUserCommand.cs
│   ├── Queries/
│   │   └── GetCurrentUserQuery.cs
│   ├── Handlers/
│   │   ├── RegisterUserHandler.cs
│   │   └── LoginUserHandler.cs
│   ├── Interfaces/
│   │   └── IAuthService.cs
│   ├── Services/
│   │   └── AuthService.cs
│   ├── DataAccess/
│   │   └── IdentityDbContext.cs
│   └── DTOs/
│
├── Pulse.Monitoring/                      # Core engine — endpoint management + scheduled health checks
│   ├── Entities/
│   │   └── MonitoredEndpoint.cs
│   ├── Commands/
│   │   ├── AddEndpointCommand.cs
│   │   └── DeleteEndpointCommand.cs
│   ├── Queries/
│   │   └── GetEndpointsQuery.cs
│   ├── Handlers/
│   │   ├── AddEndpointHandler.cs
│   │   └── RunHealthCheckHandler.cs
│   ├── Interfaces/
│   │   └── IMonitoringService.cs
│   ├── Services/
│   │   └── MonitoringService.cs
│   ├── DataAccess/
│   │   └── MonitoringDbContext.cs
│   └── DTOs/
│
├── Pulse.Observability/                   # Uptime history, latency tracking, SSL certificate data
│   ├── Entities/
│   │   └── CheckResult.cs
│   ├── Commands/
│   │   └── RecordCheckResultCommand.cs
│   ├── Queries/
│   │   ├── GetUptimeHistoryQuery.cs
│   │   └── GetLatencyTrendsQuery.cs
│   ├── Handlers/
│   │   └── RecordCheckResultHandler.cs
│   ├── Interfaces/
│   │   └── IObservabilityService.cs
│   ├── Services/
│   │   └── ObservabilityService.cs
│   ├── DataAccess/
│   │   └── ObservabilityDbContext.cs
│   └── DTOs/
│
├── Pulse.Notifications/                   # Alert rules and notification dispatch
│   ├── Entities/
│   │   └── AlertRule.cs
│   ├── Commands/
│   │   └── TriggerAlertCommand.cs
│   ├── Handlers/
│   │   └── TriggerAlertHandler.cs
│   ├── Interfaces/
│   │   └── INotificationService.cs
│   ├── Services/
│   │   └── NotificationService.cs
│   ├── DataAccess/
│   │   └── NotificationsDbContext.cs
│   └── DTOs/
│
├── Pulse.StatusPages/                     # Public and private system health presentation
│   ├── Entities/
│   │   └── StatusPage.cs
│   ├── Commands/
│   │   ├── CreateStatusPageCommand.cs
│   │   └── UpdateStatusPageCommand.cs
│   ├── Queries/
│   │   ├── GetPublicStatusPageQuery.cs
│   │   └── GetPrivateStatusPageQuery.cs
│   ├── Handlers/
│   │   ├── CreateStatusPageHandler.cs
│   │   └── GetPublicStatusPageHandler.cs
│   ├── Interfaces/
│   │   └── IStatusPageService.cs
│   ├── Services/
│   │   └── StatusPageService.cs
│   ├── DataAccess/
│   │   └── StatusPagesDbContext.cs
│   └── DTOs/
│
├── Pulse.Billing/                         # Free vs Pro tier enforcement and subscription state
│   ├── Entities/
│   │   └── Subscription.cs
│   ├── Queries/
│   │   └── GetSubscriptionQuery.cs
│   ├── Handlers/
│   │   └── GetSubscriptionHandler.cs
│   ├── Interfaces/
│   │   └── IBillingService.cs
│   ├── Services/
│   │   └── BillingService.cs
│   ├── DataAccess/
│   │   └── BillingDbContext.cs
│   └── DTOs/
│
├── Pulse.Infrastructure/                  # Redis, SNS, SES clients, shared EF config
│   ├── Persistence/
│   │   └── PulseDbContext.cs
│   ├── Redis/
│   ├── Messaging/
│   │   └── SnsAlertPublisher.cs
│   └── Migrations/
│
├── Pulse.Shared/                          # Shared contracts, base classes, result types
│   ├── Results/
│   └── Interfaces/
│
└── Pulse.Tests/
    ├── Identity/
    ├── Monitoring/
    ├── Observability/
    ├── Notifications/
    ├── StatusPages/
    └── Billing/
```

---

## Design Philosophy

Every system I build goes through two prioritisation layers — in order.

**Layer 1 — Foundation (non-negotiable before anything else):**
- **Maintainability** — if the next engineer can't navigate and extend it confidently, it's already broken
- **Scalability** — designed to grow without rewrites; feature folders mean features can be extracted into services when scale demands it

There's no point securing or optimising something that will collapse under its own weight tomorrow.

**Layer 2 — Once the foundation holds:**
- **Security** — least privilege, secure secrets, input validation, data isolation
- **Speed** — optimised queries, caching where it earns its place, non-blocking I/O
- **Reliability & Predictability** — structured logging, actionable alarms, defined failure modes, graceful degradation
