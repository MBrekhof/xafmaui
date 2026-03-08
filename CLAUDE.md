# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Session Protocol

**Every session MUST start by reading:**
1. `CLAUDE.md` (this file)
2. `TODO.md` — current task list and progress
3. `SESSION_HANDOFF.md` — context from the previous session

**Every session MUST end by:**
1. Updating `TODO.md` with current progress
2. Writing `SESSION_HANDOFF.md` with: what was done, what's next, any blockers or decisions made
3. Committing all changes

**Work mantra: commit, test, push.** After every logical unit of work:
1. Commit with a meaningful message
2. Build/test to verify nothing is broken
3. Push to remote

## Project Overview

**Project Administration MVP** — combined XAF Blazor Server back-office + .NET MAUI mobile companion.

Two coexisting solutions in the same root directory:

1. **XafMaui.slnx** — .NET MAUI mobile app (Android + iOS) using DevExpress MAUI controls
2. **XafMaui.Xaf.slnx** — DevExpress XAF Blazor Server app with EF Core, targeting net9.0

The MAUI app connects to the XAF Web API (OData + JWT). They share the same root so DTOs can be shared in the future.

### Domain

A consulting/services firm project administration system:
- **Clients** with contact persons and addresses
- **Projects** with tasks and team assignments
- **Time entries** logged by consultants against project tasks
- **Reports** — project progress, budget burn, weekly hours

### Roles

| Role | Primary App | Capabilities |
|---|---|---|
| Admin | Blazor | Full system access, user management |
| ProjectManager | Both | Manage projects/tasks, assign team, approve time entries |
| Consultant | MAUI (primary) | Log time, view assigned projects, lookup clients |
| BackOffice | Blazor | Client CRUD, reporting |

## Build & Run

```bash
# XAF Blazor Server app
dotnet build XafMaui.Xaf.slnx
dotnet run --project XafMaui.Blazor.Server

# MAUI app (Android)
dotnet build XafMaui/XafMaui.csproj -f net10.0-android

# MAUI app (iOS, macOS only)
dotnet build XafMaui/XafMaui.csproj -f net10.0-ios
```

No test projects exist yet. Solutions use `.slnx` format.

## Architecture

### XAF Blazor Server
- **XafMaui.Blazor.Server** — Blazor Server host, `Startup.cs` pattern (not minimal API)
  - `Startup.cs`: `services.AddXaf()` / `app.UseXaf()` / `endpoints.MapXafEndpoints()`
  - API docs via Scalar (`endpoints.MapOpenApi()` + `endpoints.MapScalarApiReference()`)
  - JWT auth via `AuthenticationController`
- **XafMaui.Module** — Platform-independent module with business objects, EF Core DbContext
  - All entities inherit from `BaseObjectInt` (int auto-increment key), except `ApplicationUser` (Guid from `PermissionPolicyUser`)
  - EF Core with change tracking proxies, `ObservableCollection<T>` for collections

### MAUI App
- DevExpress MAUI controls for all UI (DXCollectionView, DXDataGrid, DXChartView, DXGaugeView, editors)
- SQLite offline storage with sync-down for reference data, bidirectional sync for time entries
- JWT token in `SecureStorage`
- Tab navigation: Clients, Day Sheet, Projects, Reports

## Key Dependencies

- **DevExpress XAF v25.2.3** — pinned versions, not wildcards
- **DevExpress MAUI v25.2.*** — MAUI controls
- **EF Core 9.0.*** (XAF) / **EF Core 10.0.*** (MAUI SQLite)
- **Scalar.AspNetCore** — OpenAPI documentation UI (not Swagger/Swashbuckle)
- DevExpress NuGet feed required: `https://nuget.devexpress.com/{key}/api`

## Database

- **SQL Server 2022** in Docker container `xafmaui-sql` on port **1434**
- SA password: `XafMaui@2026`
- Connection string in `XafMaui.Blazor.Server/appsettings.json`

```bash
docker start xafmaui-sql
```

## Implementation Plan

See `docs/plans/2026-03-08-project-admin-mvp-plan.md` for the full implementation plan (28 tasks across 5 phases).

## XAF-Specific Patterns

- XAF EF Core does **not** support `OwnsOne` / owned entity types — use separate entities
- All entities use `virtual` auto-properties for change tracking proxies
- Use `[DefaultClassOptions]` to make entities visible in XAF navigation
- Use `[DefaultProperty]` for display in lookups
- Use `[DevExpress.ExpressApp.DC.Aggregated]` for parent-child (cascade delete) relationships
- Security permissions configured in `DatabaseUpdate/Updater.cs`
- Web API business objects registered in `Startup.cs` → `webApiBuilder.ConfigureOptions`

## Reference Projects

- `C:\projects\xafdynamicassemblies` — XAF Blazor Server reference for XAF patterns
- `C:\projects\xaftemplate` — Clean DevExpress XAF template (source for this project)
- `C:\projects\xafsearch` — Source of `BaseObjectInt` pattern
