# Session Handoff

## Last Session: 2026-03-09

### What was done
- All 28 MVP tasks complete and verified end-to-end on physical Android device
- Comprehensive seed data: 8 clients, 14 contacts, 9 projects, 30+ tasks, ~100 time entries
- Fixed multiple integration issues discovered during device testing:
  - `BlazorApplication.cs` — removed `Debugger.IsAttached` guard for DB auto-creation
  - Decimal precision warnings — added `HasPrecision(18, 2)` in `OnModelCreating`
  - OData `$top=200` exceeded server limit — reduced to 100
  - `JsonStringEnumConverter` — OData returns enums as strings, `System.Text.Json` expected numbers
  - TimeEntry sync missing Project/Task names — added `$expand=ProjectTask($expand=Project)`
  - Day Sheet grid — removed Note column, flex-width Project/Task columns for mobile
- Installed 34 .NET MAUI skills from davidortinau/maui-skills into `~/.claude/skills/`

### What's next
- UI polish: empty state messages, loading indicators
- Time entry creation flow (Add button on Day Sheet)
- Test all 4 user roles (Admin, Manager, Consultant, BackOffice)
- Deploy XAF backend to Docker for persistent hosting

### Decisions made
- `$top` limited to 100 per OData server default
- Physical device testing uses machine IP `192.168.50.73` in `ApiConfig.cs`
- `launchSettings.json` binds to `0.0.0.0` for LAN access (gitignored)
- OData enum deserialization uses `JsonStringEnumConverter` globally on `ApiClient`
- Seed data is idempotent (checks for existing "Contoso Ltd" client)
- Day Sheet grid hides Note column on mobile — too cramped
- TimeEntry DTO uses nested `ProjectTask.Project` structure matching OData `$expand`
- Two extra consultant users seeded: Sarah Johnson, Mike Chen (empty passwords)

### Blockers
- Visual Studio locks Android build output — must close VS or stop debug before CLI build
