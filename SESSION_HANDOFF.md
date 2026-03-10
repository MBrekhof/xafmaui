# Session Handoff

## Last Session: 2026-03-10

### What was done
- **Light/dark mode toggle**: Added Switch to Settings page, uses `Application.UserAppTheme` for instant switching. Set before `InitializeComponent()` for startup persistence.
- **Time entry creation fix**: XAF OData requires `ProjectTask@odata.bind` syntax, not `ProjectTaskID`. Fixed `PushPendingTimeEntriesAsync` to use dictionary payload with OData bind.
- **Consultant security hardening**: Added object-level owner filter on TimeEntries (`User.ID = CurrentUserId()`), plus project/task/assignment filtering to only show assigned projects.
- **Role test matrix**: Created `docs/plans/2026-03-10-role-test-matrix.md` with permission matrix, security gaps found, and test checklist for all 4 roles.
- **Database refreshed**: Dropped and recreated to pick up new role permissions.

### What's next
- Expose ReportDataV2 via Web API — Task 38
- MAUI dynamic report picker (replaces hardcoded buttons) — Task 39
- Report parameters UI in MAUI — Task 40
- Deploy XAF backend to Docker — Task 35
- Custom splash screen — Task 36
- Verify light/dark mode restart persistence — Task 37

### Decisions made
- DevExpress MAUI Theme class has no isDark constructor parameter; light/dark is controlled via `Application.UserAppTheme` which DevExpress respects via `{dx:ThemeColor}` extensions
- Consultant filtering: Projects/Tasks/Assignments filtered by `ProjectAssignments[User.ID = CurrentUserId()]`; Clients/Contacts/Addresses remain type-level Read (no back-link to Project)
- XAF OData POST requires `@odata.bind` for navigation properties, not foreign key integers

### Blockers
- Visual Studio locks Android build output — must close VS or stop debug before CLI build
- XAF server must be restarted after server-side code changes
- Database must be dropped/recreated when role definitions change (XAF skips existing roles)
