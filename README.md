# XafMaui — Project Administration MVP

A project administration system combining **DevExpress XAF Blazor Server** (back-office) with a **.NET MAUI** mobile companion app.

## What is this?

A realistic MVP for a consulting/services firm:
- **Clients** — company info, contacts, addresses
- **Projects** — tasks, team assignments, hour budgets
- **Time Tracking** — consultants log hours against project tasks
- **Reporting** — project progress, budget burn, weekly hours

## Architecture

| Component | Technology | Purpose |
|---|---|---|
| XafMaui.Blazor.Server | DevExpress XAF, Blazor Server, .NET 9 | Full back-office: CRUD, user management, approvals |
| XafMaui.Module | EF Core, SQL Server | Shared business objects and data access |
| XafMaui (MAUI app) | .NET MAUI, DevExpress MAUI, .NET 10 | Mobile: time entry, client lookup, reports |

The MAUI app communicates with XAF via its built-in OData Web API with JWT authentication. SQLite provides offline storage.

## Prerequisites

- .NET 9 SDK (for XAF)
- .NET 10 SDK (for MAUI)
- [DevExpress NuGet feed](https://docs.devexpress.com/GeneralInformation/116042/installation/install-devexpress-controls-using-nuget-packages) configured
- Docker Desktop (for SQL Server)
- Android SDK / Xcode (for MAUI)

## Getting Started

### 1. Start the database

```bash
docker run -d --name xafmaui-sql \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=XafMaui@2026" \
  -p 1434:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Run the XAF Blazor Server app

```bash
dotnet build XafMaui.Xaf.slnx
dotnet run --project XafMaui.Blazor.Server
```

The app auto-creates the database and seeds test users on first run (Admin, Manager, Consultant, Office — all with empty passwords).

API docs available at `/scalar`.

### 3. Run the MAUI app

```bash
dotnet build XafMaui/XafMaui.csproj -f net10.0-android
```

Deploy to an Android emulator or device.

## Roles

| Role | App | Access |
|---|---|---|
| Admin | Blazor | Full system access |
| Project Manager | Both | Manage projects, approve time entries |
| Consultant | MAUI | Log time, view projects, lookup clients |
| Back Office | Blazor | Client management, reporting |

## Project Structure

```
XafMaui.slnx              # MAUI-only solution
XafMaui.Xaf.slnx          # XAF Blazor Server solution
XafMaui/                   # .NET MAUI mobile app
XafMaui.Blazor.Server/     # XAF Blazor Server host
XafMaui.Module/            # Shared business objects (EF Core)
docs/plans/                # Design docs and implementation plans
```

## Tech Stack

- **DevExpress XAF 25.2.3** — application framework with built-in security, Web API, Blazor UI
- **DevExpress MAUI 25.2** — DataGrid, Charts, Gauges, CollectionView, Editors
- **Entity Framework Core** — SQL Server (server), SQLite (mobile)
- **Scalar** — API documentation UI

## License

Private project. DevExpress licenses required.
