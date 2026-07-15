# BingehOS

BingehOS is a multi-tenant Computerized Maintenance Management System (CMMS) built with .NET 8 and a modular/clean architecture.

## Architecture

- **BingehOS.Api** - ASP.NET Core Web API entry point
- **BingehOS.Infrastructure** - Persistence, storage, messaging, cross-cutting concerns
- **BingehOS.Shared** - Base entities, value objects, common primitives
- **BingehOS.Modules.\*** - Feature modules split into `Domain` (entities, rules) and `Application` (commands, queries, handlers)
- **Tests** - `UnitTests` (xUnit) and `IntegrationTests` (WebApplicationFactory + Testcontainers)

## Prerequisites

- .NET 8 SDK
- Docker (optional but recommended for Postgres, MinIO, RabbitMQ)
- PostgreSQL 16
- MinIO
- RabbitMQ

## Local Setup

1. `git clone <repo>`
2. `dotnet restore BingehOS.sln`
3. Update connection strings in `appsettings.Development.json` or environment variables
4. `dotnet build`
5. `dotnet test tests/BingehOS.UnitTests`
6. Run the API: `dotnet run --project src/BingehOS.Api/BingehOS.Api`

## Docker

```bash
docker-compose up --build
```

## API Endpoints

| Method | Path                             | Description                        |
|--------|----------------------------------|------------------------------------|
| GET    | /health                          | Overall health check               |
| GET    | /health/live                     | Liveness check                     |
| GET    | /health/ready                    | Readiness check                    |
| POST   | /v1/auth/register                | Register a user (admin only)       |
| POST   | /v1/auth/login                   | Login (JWT)                        |
| POST   | /v1/auth/assign-role             | Assign a role (admin only)         |
| GET    | /v1/facilities                  | List facilities                    |
| GET    | /v1/facilities/{id}              | Get facility                       |
| POST   | /v1/facilities                   | Create facility                    |
| PATCH  | /v1/facilities/{id}              | Update facility                    |
| GET    | /v1/assets                      | List assets                        |
| POST   | /v1/assets                      | Create asset                       |
| PATCH  | /v1/assets/{id}                  | Update asset                       |
| GET    | /v1/assets/{id}                  | Get asset                          |
| GET    | /v1/parts                       | List parts                         |
| GET    | /v1/parts/{id}                   | Get part                           |
| POST   | /v1/parts                        | Create part                        |
| PATCH  | /v1/parts/{id}                   | Update part                        |
| POST   | /v1/parts/{id}/receive           | Receive stock for a part           |
| POST   | /v1/parts/{id}/issue             | Issue stock for a part             |
| POST   | /v1/parts/{id}/return            | Return stock for a part            |
| GET    | /v1/inventory/transactions       | List inventory transactions        |
| GET    | /v1/workers                     | List workers                       |
| GET    | /v1/workers/{id}                 | Get worker                         |
| POST   | /v1/workers                      | Create worker                      |
| PATCH  | /v1/workers/{id}                 | Update worker                      |
| GET    | /v1/users                        | List users (admin permission)      |
| GET    | /v1/users/{id}                   | Get user (admin permission)        |
| PATCH  | /v1/users/{id}                   | Update user (admin permission)     |
| DELETE | /v1/users/{id}                   | Soft-delete user (admin permission)|
| GET    | /v1/roles                        | List roles (admin permission)      |
| GET    | /v1/roles/{id}                   | Get role (admin permission)        |
| POST   | /v1/roles                        | Create role (admin permission)     |
| PATCH  | /v1/roles/{id}                   | Update role (admin permission)     |
| DELETE | /v1/roles/{id}                   | Soft-delete role (admin permission)|
| POST   | /v1/roles/{id}/permissions/{permissionId} | Assign role permission      |
| DELETE | /v1/roles/{id}/permissions/{permissionId} | Remove role permission      |
| GET    | /v1/permissions                  | List permissions (admin permission)|
| GET    | /v1/permissions/{id}              | Get permission (admin permission)  |
| POST   | /v1/permissions                  | Create permission (admin permission)|
| GET    | /v1/vendors                     | List vendors                       |
| GET    | /v1/vendors/{id}                 | Get vendor                         |
| POST   | /v1/vendors                      | Create vendor                      |
| PATCH  | /v1/vendors/{id}                 | Update vendor                      |
| GET    | /v1/work-orders                  | List work orders                   |
| GET    | /v1/work-orders/{id}             | Get work order                     |
| POST   | /v1/work-orders                  | Create work order                  |
| PATCH  | /v1/work-orders/{id}/status      | Change work order status           |
| GET    | /v1/permits                     | List permits                       |
| GET    | /v1/permits/{id}                 | Get permit                         |
| POST   | /v1/permits                      | Create permit                      |
| PATCH  | /v1/permits/{id}/approve         | Approve permit                     |
| PATCH  | /v1/permits/{id}/reject          | Reject permit                      |
| GET    | /v1/risk-assessments            | List risk assessments              |
| GET    | /v1/risk-assessments/{id}        | Get risk assessment                |
| POST   | /v1/risk-assessments             | Create risk assessment             |
| GET    | /v1/loto-procedures             | List LOTO procedures               |
| GET    | /v1/loto-procedures/{id}         | Get LOTO procedure                 |
| POST   | /v1/loto-procedures              | Create LOTO procedure              |
| PATCH  | /v1/loto-procedures/{id}/verify  | Verify LOTO procedure              |
| GET    | /v1/employees                   | List employees                     |
| GET    | /v1/employees/{id}               | Get employee                       |
| POST   | /v1/employees                    | Create employee                    |
| PATCH  | /v1/employees/{id}               | Update employee                    |
| GET    | /v1/sgk-records                 | List SGK records                   |
| GET    | /v1/sgk-records/{id}             | Get SGK record                     |
| POST   | /v1/sgk-records                  | Create SGK record                  |
| GET    | /v1/subcontractors              | List subcontractors                |
| GET    | /v1/subcontractors/{id}          | Get subcontractor                  |
| POST   | /v1/subcontractors               | Create subcontractor               |
| PATCH  | /v1/subcontractors/{id}          | Update subcontractor               |
| GET    | /v1/invoices                    | List invoices                      |
| GET    | /v1/invoices/{id}                | Get invoice                        |
| POST   | /v1/invoices                     | Create invoice                     |
| PATCH  | /v1/invoices/{id}                | Update invoice                     |
| GET    | /v1/tax-records                 | List tax records                   |
| GET    | /v1/tax-records/{id}             | Get tax record                     |
| POST   | /v1/tax-records                  | Create tax record                  |
| GET    | /v1/cost-centers                | List cost centers                  |
| GET    | /v1/cost-centers/{id}            | Get cost center                    |
| POST   | /v1/cost-centers                 | Create cost center                 |
| PATCH  | /v1/cost-centers/{id}            | Update cost center                 |
| GET    | /v1/compliance-records          | List compliance records            |
| GET    | /v1/compliance-records/{id}      | Get compliance record              |
| POST   | /v1/compliance-records           | Create compliance record           |
| PATCH  | /v1/compliance-records/{id}      | Update compliance record           |
| GET    | /v1/job-plan-templates          | List job plan templates            |
| GET    | /v1/job-plan-templates/{id}      | Get job plan template              |
| POST   | /v1/job-plan-templates           | Create job plan template           |
| PATCH  | /v1/job-plan-templates/{id}      | Update job plan template           |
| GET    | /v1/kvkk-consents               | List KVKK consents                 |
| GET    | /v1/kvkk-consents/{id}           | Get KVKK consent                   |
| POST   | /v1/kvkk-consents                | Grant KVKK consent                 |
| PATCH  | /v1/kvkk-consents/{id}/revoke    | Revoke KVKK consent                |
| GET    | /v1/automation-rules            | List automation rules              |
| GET    | /v1/automation-rules/{id}        | Get automation rule                |
| POST   | /v1/automation-rules             | Create automation rule             |
| PATCH  | /v1/automation-rules/{id}        | Update or enable/disable rule      |
| DELETE | /v1/automation-rules/{id}        | Soft-delete automation rule        |
| GET    | /v1/automation-rules/{id}/executions | Rule execution history         |
| GET    | /v1/plugins                     | List plugin registrations          |
| GET    | /v1/plugins/{id}                 | Get plugin registration            |
| POST   | /v1/plugins                      | Register plugin metadata           |
| PATCH  | /v1/plugins/{id}                 | Update plugin metadata             |
| DELETE | /v1/plugins/{id}                 | Soft-delete plugin registration    |
| GET    | /v1/insights/assets             | Risk-ranked asset failure insights |
| GET    | /v1/insights/parts              | Parts reorder threshold insights   |
| GET    | /v1/floor-plans                 | List floor plans                   |
| GET    | /v1/floor-plans/{id}             | Get floor plan                     |
| POST   | /v1/floor-plans                  | Create floor plan                  |
| PATCH  | /v1/floor-plans/{id}             | Update floor plan                  |
| DELETE | /v1/floor-plans/{id}             | Soft-delete floor plan             |
| GET    | /v1/floor-plans/{id}/positions   | List asset positions on plan       |
| PUT    | /v1/floor-plans/{id}/positions   | Replace asset positions on plan    |
| GET    | /v1/calibrations                | List calibration records           |
| GET    | /v1/calibrations/{id}            | Get calibration record             |
| POST   | /v1/calibrations                 | Create calibration record          |
| PATCH  | /v1/calibrations/{id}            | Update calibration record          |
| GET    | /v1/calendar/holidays?year=     | Turkish public holidays            |

## PWA Support

The web app is installable as a Turkish PWA with offline fallback. Full offline data sync is deferred.

## Testing

```bash
# Unit tests
dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj --configuration Release

# Integration tests (requires Docker for Testcontainers)
dotnet test tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/BingehOS.IntegrationTests.csproj --configuration Release
```

## License

MIT
