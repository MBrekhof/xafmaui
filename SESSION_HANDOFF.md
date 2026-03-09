# Session Handoff

## Last Session: 2026-03-09

### What was done
- **Material 3 theme**: Applied TealGreen seed color via `ThemeManager.Theme = new Theme(ThemeSeedColor.TealGreen)` in `MauiProgram.cs`
- **Day Sheet BottomSheet**: Tap any time entry row to see a slide-up detail panel showing Project, Task, Hours, Status, and Notes. Uses DevExpress `dx:BottomSheet` with `HalfExpandedRatio="0.45"`, controlled from code-behind via `detailSheet.State = BottomSheetState.HalfExpanded`
- **Server-side PDF reports**: Created `ProjectReportController` with two endpoints:
  - `GET /api/ProjectReport/WeeklyTimesheet?weekStart=yyyy-MM-dd` — time entries grouped by day
  - `GET /api/ProjectReport/ProjectBudget` — all active/on-hold projects with budget vs actual
  Reports are generated programmatically using XtraReports (no stored report definitions needed)
- Added `GetBytesAsync()` to `ApiClient` for raw byte downloads
- MAUI Reports tab now has "Download Weekly Timesheet" and "Download Project Budget Report" buttons that fetch PDFs and open with system viewer via `Launcher.OpenAsync`
- Previous session's features still in place: all 28 MVP tasks, seed data, pull-to-refresh, logout

### What's next
- Deploy latest to phone and test all three new features
- Time entry creation flow (Add button on Day Sheet)
- Test all 4 user roles (Admin, Manager, Consultant, BackOffice)
- Deploy XAF backend to Docker for persistent hosting

### Decisions made
- Used `TealGreen` from `ThemeSeedColor` enum (not custom color) for M3 theme
- BottomSheet controlled from code-behind (not MVVM binding) — simpler, no converter needed
- PDF reports use programmatic XtraReport generation, not stored ReportDataV2 definitions
- `ProjectReportController` uses `IObjectSpaceFactory` for EF Core data access with XAF security
- Reports open via `Launcher.OpenAsync` with system PDF viewer (no in-app viewer)
- Existing `ReportController.cs` (DownloadByName/DownloadByKey) left unchanged — requires stored report definitions

### Blockers
- Visual Studio locks Android build output — must close VS or stop debug before CLI build
- XAF server must be restarted after code changes to `ProjectReportController`
