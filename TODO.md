# TODO

Current implementation status for the Project Administration MVP.
See `docs/plans/2026-03-08-project-admin-mvp-plan.md` for full plan details.

## Phase 1: XAF Data Model & Business Objects
- [x] Task 1: Add BaseObjectInt base class
- [x] Task 2: Add enums
- [x] Task 3: Add Address entity
- [x] Task 4: Add Client and ContactPerson entities
- [x] Task 5: Add Project and ProjectTask entities
- [x] Task 6: Add ProjectAssignment and TimeEntry entities
- [x] Task 7: Extend ApplicationUser with new properties
- [x] Task 8: Register entities in DbContext
- [x] Task 9: Fix AuthenticationController Swashbuckle reference

## Phase 2: XAF Web API & Security Configuration
- [x] Task 10: Register business objects in Web API
- [x] Task 11: Configure security roles in Updater.cs
- [x] Task 12: Run XAF app and verify database creation

## Phase 3: MAUI App — Foundation & API Layer
- [x] Task 13: Add required NuGet packages to MAUI project
- [x] Task 14: Create MAUI DTOs (local models)
- [x] Task 15: Create SQLite local database context
- [x] Task 16: Create API client service
- [x] Task 17: Create sync service
- [x] Task 18: Register services in MauiProgram.cs

## Phase 4: MAUI App — UI Screens
- [x] Task 19: Create Login page
- [x] Task 20: Restructure AppShell with tab navigation
- [x] Task 21: Create stub pages for all four tabs (replaced with full implementations)
- [x] Task 22: Build Clients tab with search and detail
- [x] Task 23: Build Day Sheet tab
- [x] Task 24: Build Projects tab
- [x] Task 25: Build Reports tab

## Phase 5: Integration & Polish
- [x] Task 26: Add pull-to-refresh on all list pages
- [x] Task 27: Add logout functionality
- [x] Task 28: Verify end-to-end flow

## Phase 6: UI Polish & Advanced Features
- [x] Task 29: Material 3 theming (TealGreen seed color)
- [x] Task 30: BottomSheet for Day Sheet time entry details
- [x] Task 31: Server-side PDF reports (Weekly Timesheet, Project Budget)
- [x] Task 32: Time entry creation flow (Add button on Day Sheet)
- [x] Task 33: Test all 4 user roles (security review + role test matrix)
- [x] Task 34: Register predefined reports in XAF (Weekly Timesheet, Project Budget)

## Phase 7: Time Entry Approval Workflow
- [x] Task 35: Add `ReviewNote` field to TimeEntry entity (XAF)
- [x] Task 36: Manager approval flow — PM can set status to Approved/Rejected with ReviewNote
- [x] Task 37: MAUI — show rejection note in Day Sheet bottom sheet detail
- [x] Task 38: MAUI — allow editing rejected entries (tap → edit → resubmit as Draft)
- [x] Task 39: MAUI — visual status indicators on Day Sheet (color/icon per status)
- [ ] Task 40: (Optional) MAUI manager approval screen — list submitted entries for PM's projects

## Phase 8: Reporting & Self-Service
- [x] Task 41: Light/dark mode — verified restart persistence works correctly (code review)
- [x] Task 42: Expose ReportDataV2 via Web API (register in Startup.cs webApiBuilder)
- [x] Task 43: MAUI dynamic report picker — fetch available reports from ReportDataV2 OData endpoint, let user pick and download any report
- [x] Task 44: Add report parameters UI in MAUI (date range for timesheet, project picker for budget)
- [x] Task 45: XAF Blazor — Report Designer verified (EnableInplaceReports + ReportsV2 module enabled)

## Phase 9: Final Polish & Deployment
- [ ] Task 46: Custom splash screen (currently shows default ".NET" branding)
- [ ] Task 47: Deploy XAF backend to Docker
