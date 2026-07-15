# Remaining Work Plan

## State
- MVP progress ~65% (doc 18 s4).
- Observability: DONE.
- Security: tenant-header-to-JWT binding DONE (commit ba8b719). RBAC enforcement partial (only AssetsController gated).
- CI/CD: EXISTS but RED. Blocked by integration login 400 in TestAuthHelper.

## Immediate Blocker
CI run `29348577335` fails with `LOGIN FAILED status=BadRequest body={"success":false,"error":"IDX10703: Cannot create a 'Microsoft.IdentityModel.Tokens.SymmetricSecurityKey', key length is zero."}` on all 13 integration tests.

Root cause: `Program.cs` line 78 calls `Configure<JwtSettings>(...)`, but because `Program.cs` has `using BingehOS.Api.Auth;`, this configures `BingehOS.Api.Auth.JwtSettings`. Meanwhile `AddIdentityModule()` registers `ITokenService` → `BingehOS.Infrastructure.Security.JwtTokenService`, which consumes `BingehOS.Infrastructure.Security.JwtSettings`. That settings class is never configured, so `Secret` stays at its default `string.Empty`. `GenerateToken` then throws `IDX10703` (zero-length key), and `GlobalExceptionFilter` maps it to 400.

## Ordered Remaining Tasks

### 1. Get CI green (integration login 400)
- In `Program.cs`, after the existing `Configure<JwtSettings>(...)` block, add:
  ```csharp
  builder.Services.Configure<BingehOS.Infrastructure.Security.JwtSettings>(opt =>
  {
      opt.Secret = jwtSecret;
      opt.Issuer = jwtIssuer;
      opt.Audience = jwtAudience;
      opt.ExpiresInSeconds = 3600;
  });
  ```
- Revert the diagnostic assertion in `TestAuthHelper.cs` back to a clean equality check (or keep the body-surfacing version — both pass now).
- Build + run unit tests locally; push and let GitHub CI run integration tests (runner has Docker).
- Verify CI is green on `main`.

### 2. Security hardening
- Enforce RBAC on remaining controllers (`[HasPermission]` or global policy).
- Complete RLS for remaining tenant tables + EF Core global `TenantId` query filter.
- Replace hardcoded dev JWT secret fallback + docker-compose plaintext creds.

### 3. Missing MVP features
- Inventory transactions (Receiving / Issue / Return).
- Facility location-tree endpoints (Campus/Building/Floor/Zone/Room).
- CalibrationRecord endpoint.

### 4. Architecture improvements
- FluentValidation + MediatR `IPipelineBehavior`.
- Transactional Outbox for RabbitMQ.
- Align event architecture (INotification vs direct RabbitMQ).

### 5. Testing coverage
- E2E tests (Playwright).
- Enforce 80% coverage gate in CI.

### 6. Observability ops
- Wire OTel Collector / Loki / Prometheus / Grafana in docker-compose.

### 2. Security hardening
- Enforce RBAC on remaining controllers ([HasPermission] or global policy).
- Complete RLS for remaining tenant tables + EF Core global TenantId query filter.
- Replace hardcoded dev JWT secret fallback + docker-compose plaintext creds.

### 3. Missing MVP features
- Inventory transactions (Receiving / Issue / Return).
- Facility location-tree endpoints (Campus/Building/Floor/Zone/Room).
- CalibrationRecord endpoint.

### 4. Architecture improvements
- FluentValidation + MediatR IPipelineBehavior.
- Transactional Outbox for RabbitMQ.
- Align event architecture (INotification vs direct RabbitMQ).

### 5. Testing coverage
- E2E tests (Playwright).
- Enforce 80% coverage gate in CI.

### 6. Observability ops
- Wire OTel Collector / Loki / Prometheus / Grafana in docker-compose.

## Validation
- dotnet build + dotnet test per task.
- CI must be green on main.
- Commit with conventional English messages and push to origin/main after each step.
