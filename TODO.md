# TODO

Current implementation status for the Project Administration MVP.
See `docs/plans/2026-03-08-project-admin-mvp-plan.md` for full plan details.

## Phase 1: XAF Data Model & Business Objects
- [ ] Task 1: Add BaseObjectInt base class
- [ ] Task 2: Add enums
- [ ] Task 3: Add Address entity
- [ ] Task 4: Add Client and ContactPerson entities
- [ ] Task 5: Add Project and ProjectTask entities
- [ ] Task 6: Add ProjectAssignment and TimeEntry entities
- [ ] Task 7: Extend ApplicationUser with new properties
- [ ] Task 8: Register entities in DbContext
- [ ] Task 9: Fix AuthenticationController Swashbuckle reference

## Phase 2: XAF Web API & Security Configuration
- [ ] Task 10: Register business objects in Web API
- [ ] Task 11: Configure security roles in Updater.cs
- [ ] Task 12: Run XAF app and verify database creation

## Phase 3: MAUI App — Foundation & API Layer
- [ ] Task 13: Add required NuGet packages to MAUI project
- [ ] Task 14: Create MAUI DTOs (local models)
- [ ] Task 15: Create SQLite local database context
- [ ] Task 16: Create API client service
- [ ] Task 17: Create sync service
- [ ] Task 18: Register services in MauiProgram.cs

## Phase 4: MAUI App — UI Screens
- [ ] Task 19: Create Login page
- [ ] Task 20: Restructure AppShell with tab navigation
- [ ] Task 21: Create stub pages for all four tabs
- [ ] Task 22: Build Clients tab with search and detail
- [ ] Task 23: Build Day Sheet tab
- [ ] Task 24: Build Projects tab
- [ ] Task 25: Build Reports tab

## Phase 5: Integration & Polish
- [ ] Task 26: Add pull-to-refresh on all list pages
- [ ] Task 27: Add logout functionality
- [ ] Task 28: Verify end-to-end flow
