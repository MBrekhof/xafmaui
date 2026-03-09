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
- [ ] Task 28: Verify end-to-end flow
