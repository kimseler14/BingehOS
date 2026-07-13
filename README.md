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
| GET    | /v1/health                       | Health check                       |
| POST   | /v1/auth/login                   | Login (JWT)                        |
| GET    | /v1/facilities/{id}              | Get facility                       |
| POST   | /v1/facilities                   | Create facility                    |
| PATCH  | /v1/facilities/{id}              | Update facility                    |
| GET    | /v1/assets/{id}                  | Get asset                          |
| POST   | /v1/assets                       | Create asset                       |
| PATCH  | /v1/assets/{id}                  | Update asset                       |
| GET    | /v1/work-orders/{id}             | Get work order                     |
| POST   | /v1/work-orders                  | Create work order                  |
| PATCH  | /v1/work-orders/{id}/status      | Change work order status           |
| GET    | /v1/parts/{id}                   | Get part                           |
| POST   | /v1/parts                        | Create part                        |
| PATCH  | /v1/parts/{id}                   | Update part                        |
| GET    | /v1/vendors/{id}                 | Get vendor                         |
| POST   | /v1/vendors                      | Create vendor                      |
| PATCH  | /v1/vendors/{id}                 | Update vendor                      |
| GET    | /v1/incidents/{id}               | Get incident                       |
| POST   | /v1/incidents                    | Create incident                    |
| PATCH  | /v1/incidents/{id}               | Update incident                    |
| GET    | /v1/employees/{id}              | Get employee                         |
| POST   | /v1/employees                   | Create employee                      |
| PATCH  | /v1/employees/{id}              | Update employee                      |
| GET    | /v1/sgk-records/{id}            | Get SGK record                       |
| POST   | /v1/sgk-records                 | Create SGK record                    |
| GET    | /v1/subcontractors/{id}         | Get subcontractor                    |
| POST   | /v1/subcontractors              | Create subcontractor                 |
| PATCH  | /v1/subcontractors/{id}         | Update subcontractor                 |
| GET    | /v1/work-order-costs/{id}        | Get work order cost                |
| POST   | /v1/work-order-costs             | Create work order cost             |
| PATCH  | /v1/work-order-costs/{id}/approve| Approve work order cost            |
| GET    | /v1/compliance-records/{id}      | Get compliance record              |
| POST   | /v1/compliance-records           | Create compliance record           |
| PATCH  | /v1/compliance-records/{id}      | Update compliance record              |
| GET    | /v1/job-plan-templates/{id}      | Get job plan template                 |
| POST   | /v1/job-plan-templates           | Create job plan template              |
| PATCH  | /v1/job-plan-templates/{id}      | Update job plan template              |
| GET    | /v1/kvkk-consents/{id}           | Get KVKK consent                      |
| POST   | /v1/kvkk-consents                | Grant KVKK consent                    |
| PATCH  | /v1/kvkk-consents/{id}/revoke    | Revoke KVKK consent                   |

## Testing

```bash
# Unit tests
dotnet test tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj --configuration Release

# Integration tests (requires Docker for Testcontainers)
dotnet test tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/BingehOS.IntegrationTests.csproj --configuration Release
```

## License

MIT
