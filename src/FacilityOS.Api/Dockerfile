FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["FacilityOS.sln", "."]
COPY ["src/FacilityOS.Shared/FacilityOS.Shared.csproj", "src/FacilityOS.Shared/"]
COPY ["src/FacilityOS.Infrastructure/FacilityOS.Infrastructure.csproj", "src/FacilityOS.Infrastructure/"]
COPY ["src/modules/Maintenance.Domain/FacilityOS.Modules.Maintenance.Domain.csproj", "src/modules/Maintenance.Domain/"]
COPY ["src/modules/Maintenance/FacilityOS.Modules.Maintenance.csproj", "src/modules/Maintenance/"]
COPY ["src/modules/Asset.Domain/FacilityOS.Modules.Asset.Domain.csproj", "src/modules/Asset.Domain/"]
COPY ["src/modules/Asset/FacilityOS.Modules.Asset.csproj", "src/modules/Asset/"]
COPY ["src/modules/Facility.Domain/FacilityOS.Modules.Facility.Domain.csproj", "src/modules/Facility.Domain/"]
COPY ["src/modules/Facility/FacilityOS.Modules.Facility.csproj", "src/modules/Facility/"]
COPY ["src/modules/Inventory.Domain/FacilityOS.Modules.Inventory.Domain.csproj", "src/modules/Inventory.Domain/"]
COPY ["src/modules/Inventory/FacilityOS.Modules.Inventory.csproj", "src/modules/Inventory/"]
COPY ["src/modules/Vendor.Domain/FacilityOS.Modules.Vendor.Domain.csproj", "src/modules/Vendor.Domain/"]
COPY ["src/modules/Vendor/FacilityOS.Modules.Vendor.csproj", "src/modules/Vendor/"]
COPY ["src/modules/HSE.Domain/FacilityOS.Modules.HSE.Domain.csproj", "src/modules/HSE.Domain/"]
COPY ["src/modules/HSE/FacilityOS.Modules.HSE.csproj", "src/modules/HSE/"]
COPY ["src/modules/Personnel.Domain/FacilityOS.Modules.Personnel.Domain.csproj", "src/modules/Personnel.Domain/"]
COPY ["src/modules/Personnel/FacilityOS.Modules.Personnel.csproj", "src/modules/Personnel/"]
COPY ["src/modules/Finance.Domain/FacilityOS.Modules.Finance.Domain.csproj", "src/modules/Finance.Domain/"]
COPY ["src/modules/Finance/FacilityOS.Modules.Finance.csproj", "src/modules/Finance/"]
COPY ["src/modules/Compliance.Domain/FacilityOS.Modules.Compliance.Domain.csproj", "src/modules/Compliance.Domain/"]
COPY ["src/modules/Compliance/FacilityOS.Modules.Compliance.csproj", "src/modules/Compliance/"]
COPY ["src/FacilityOS.Api/FacilityOS.Api/FacilityOS.Api.csproj", "src/FacilityOS.Api/FacilityOS.Api/"]
COPY ["tests/FacilityOS.UnitTests/FacilityOS.UnitTests.csproj", "tests/FacilityOS.UnitTests/"]
COPY ["tests/FacilityOS.IntegrationTests/FacilityOS.IntegrationTests/FacilityOS.IntegrationTests.csproj", "tests/FacilityOS.IntegrationTests/FacilityOS.IntegrationTests/"]
RUN dotnet restore "FacilityOS.sln"
COPY . .
WORKDIR "/src/src/FacilityOS.Api/FacilityOS.Api"
RUN dotnet build "FacilityOS.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "FacilityOS.Api.csproj" -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacilityOS.Api.dll"]
