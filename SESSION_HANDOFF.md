# Session Handoff

## Last Session: 2026-03-09

### What was done
- **Material 3 theme**: Applied via `ThemeManager.Theme` with saved preference from `Preferences`
- **Theme picker**: New Settings tab with all 10 M3 seed colors, instant apply + persistent
- **Day Sheet BottomSheet**: Tap time entry row → slide-up detail panel (project, task, hours, status, notes)
- **Server-side PDF reports**: `ProjectReportController` with WeeklyTimesheet and ProjectBudget endpoints
- **MAUI PDF download**: Reports tab has download buttons → fetch PDF → open with system viewer
- **Login error UX**: Invalid credentials show as field ErrorText, connection errors as dismissible alert
- **Task picker fix**: Changed `SelectedIndexChanged` → `SelectionChanged` on project ComboBoxEdit so tasks populate
- **LoginPage fix**: Restored `<dx:TextEdit>` element that was corrupted to `<A>`

### What's next
- Deploy latest to phone and test all new features
- Time entry creation flow — test full add → sync → verify on server
- Test all 4 user roles (Admin, Manager, Consultant, BackOffice)
- Deploy XAF backend to Docker for persistent hosting

### Decisions made
- Theme saved to `Preferences` as enum string, read at startup in `MauiProgram.cs`
- `ThemeManager.Theme` set at runtime for instant preview, persists across launches
- Settings tab added as 5th tab with gear icon
- PDF reports use programmatic XtraReport generation (no stored ReportDataV2 definitions)
- Login errors: field-level ErrorText for credentials, DisplayAlertAsync for connection errors

### Blockers
- Visual Studio locks Android build output — must close VS or stop debug before CLI build
- XAF server must be restarted after server-side code changes
