# Session Handoff

## Last Session: 2026-03-08

### What was done
- Created XAF Blazor Server project (XafMaui.Blazor.Server + XafMaui.Module) from DevExpress template
- Retargeted from net8.0 to net9.0 (XAF 25.2 supports .NET 9 runtime)
- Replaced Swashbuckle/Swagger with Scalar for API docs
- Created separate solution file `XafMaui.Xaf.slnx` (coexists with `XafMaui.slnx` for MAUI)
- Set up SQL Server 2022 Docker container (`xafmaui-sql`, port 1434, SA password `XafMaui@2026`)
- Configured connection string in appsettings.json
- XAF solution builds successfully (0 errors, 0 warnings)
- Designed the full MVP: data model, API, MAUI screens, security roles, offline storage
- Wrote design doc: `docs/plans/2026-03-08-project-admin-mvp-design.md`
- Wrote implementation plan: `docs/plans/2026-03-08-project-admin-mvp-plan.md` (28 tasks, 5 phases)
- Created CLAUDE.md, TODO.md, SESSION_HANDOFF.md, README.md
- Initialized git repo, created public GitHub remote

### What's next
- Start Phase 1, Task 1: Add BaseObjectInt base class
- Work through Phase 1-2 (Tasks 1-12) to get XAF backend running with all entities and security
- Then Phase 3-5 for MAUI app

### Decisions made
- `BaseObjectInt` (int key) for all custom entities, copied from XafSearch project pattern
- `ApplicationUser` keeps Guid key (inherits from `PermissionPolicyUser`)
- EF Core `OwnsOne` not supported by XAF — Address is a separate entity
- Invoicing is out of scope (future phase)
- DevExpress components only, unless DevExpress doesn't provide it
- SQLite in MAUI for offline: sync-down reference data, bidirectional for time entries
- Four roles: Admin, ProjectManager, Consultant, BackOffice

### Blockers
- None
