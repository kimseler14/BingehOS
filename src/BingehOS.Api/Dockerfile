FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["BingehOS.sln", "."]
COPY ["src/BingehOS.Shared/BingehOS.Shared.csproj", "src/BingehOS.Shared/"]
COPY ["src/BingehOS.Infrastructure/BingehOS.Infrastructure.csproj", "src/BingehOS.Infrastructure/"]
COPY ["src/modules/Maintenance.Domain/BingehOS.Modules.Maintenance.Domain.csproj", "src/modules/Maintenance.Domain/"]
COPY ["src/modules/Maintenance/BingehOS.Modules.Maintenance.csproj", "src/modules/Maintenance/"]
COPY ["src/modules/Asset.Domain/BingehOS.Modules.Asset.Domain.csproj", "src/modules/Asset.Domain/"]
COPY ["src/modules/Asset/BingehOS.Modules.Asset.csproj", "src/modules/Asset/"]
COPY ["src/modules/Facility.Domain/BingehOS.Modules.Facility.Domain.csproj", "src/modules/Facility.Domain/"]
COPY ["src/modules/Facility/BingehOS.Modules.Facility.csproj", "src/modules/Facility/"]
COPY ["src/modules/Inventory.Domain/BingehOS.Modules.Inventory.Domain.csproj", "src/modules/Inventory.Domain/"]
COPY ["src/modules/Inventory/BingehOS.Modules.Inventory.csproj", "src/modules/Inventory/"]
COPY ["src/modules/Vendor.Domain/BingehOS.Modules.Vendor.Domain.csproj", "src/modules/Vendor.Domain/"]
COPY ["src/modules/Vendor/BingehOS.Modules.Vendor.csproj", "src/modules/Vendor/"]
COPY ["src/modules/HSE.Domain/BingehOS.Modules.HSE.Domain.csproj", "src/modules/HSE.Domain/"]
COPY ["src/modules/HSE/BingehOS.Modules.HSE.csproj", "src/modules/HSE/"]
COPY ["src/modules/Personnel.Domain/BingehOS.Modules.Personnel.Domain.csproj", "src/modules/Personnel.Domain/"]
COPY ["src/modules/Personnel/BingehOS.Modules.Personnel.csproj", "src/modules/Personnel/"]
COPY ["src/modules/Finance.Domain/BingehOS.Modules.Finance.Domain.csproj", "src/modules/Finance.Domain/"]
COPY ["src/modules/Finance/BingehOS.Modules.Finance.csproj", "src/modules/Finance/"]
COPY ["src/modules/Compliance.Domain/BingehOS.Modules.Compliance.Domain.csproj", "src/modules/Compliance.Domain/"]
COPY ["src/modules/Compliance/BingehOS.Modules.Compliance.csproj", "src/modules/Compliance/"]
COPY ["src/BingehOS.Api/BingehOS.Api/BingehOS.Api.csproj", "src/BingehOS.Api/BingehOS.Api/"]
COPY ["tests/BingehOS.UnitTests/BingehOS.UnitTests.csproj", "tests/BingehOS.UnitTests/"]
COPY ["tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/BingehOS.IntegrationTests.csproj", "tests/BingehOS.IntegrationTests/BingehOS.IntegrationTests/"]
RUN dotnet restore "BingehOS.sln"
COPY . .
WORKDIR "/src/src/BingehOS.Api/BingehOS.Api"
RUN dotnet build "BingehOS.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "BingehOS.Api.csproj" -c Release -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BingehOS.Api.dll"]
