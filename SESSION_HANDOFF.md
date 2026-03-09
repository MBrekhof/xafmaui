# Session Handoff

## Last Session: 2026-03-09

### What was done
- Task 12: Verified XAF app runs, database auto-creates, all OData endpoints work, JWT auth works, Scalar API docs accessible
  - Fixed `BlazorApplication.cs` — removed `Debugger.IsAttached` guard, now uses `#if DEBUG` so DB auto-creates with `dotnet run`
  - Added `HasPrecision(18, 2)` for decimal properties (BudgetHours, EstimatedHours, Hours) in `OnModelCreating`
- Task 26: Added pull-to-refresh on all list pages
  - ClientsPage: DXCollectionView `IsPullToRefreshEnabled` → syncs clients from API
  - DaySheetPage: DataGridView `IsPullToRefreshEnabled` → pushes pending + pulls time entries
  - ProjectsPage: `RefreshView` wrapping `CollectionView` → syncs projects from API
  - ReportsPage: `RefreshView` wrapping `ScrollView` → full sync
- Task 27: Added logout functionality
  - Toolbar item "Logout" (secondary/overflow menu) on AppShell
  - Confirmation dialog, clears JWT token + local SQLite data, navigates back to LoginPage
- Installed 34 .NET MAUI skills from davidortinau/maui-skills into `~/.claude/skills/`
- Both solutions build with 0 warnings, 0 errors

### What's next
- Task 28: End-to-end verification (XAF + MAUI together on device/emulator)
- All 28 tasks will be complete after Task 28

### Decisions made
- Pull-to-refresh uses `SyncService` methods per page (not full sync on every page)
- Logout uses `ToolbarItem Order="Secondary"` (overflow menu via three dots)
- `DisplayAlertAsync` used instead of obsolete `DisplayAlert` (.NET 10 MAUI)
- ViewModels resolve `SyncService` via `IPlatformApplication.Current.Services` since they're not DI-injected

### Blockers
- None. Ready for end-to-end testing on emulator/device.
