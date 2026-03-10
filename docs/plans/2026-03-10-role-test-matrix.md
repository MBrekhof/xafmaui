# Role Security Test Matrix

Task 33 — Test all 4 user roles (XAF Blazor + MAUI).

## Test Users

| User | Password | Role | XAF Role Name |
|---|---|---|---|
| Admin | *(empty)* | Admin | Administrators (`IsAdministrative = true`) |
| Manager | *(empty)* | ProjectManager | ProjectManagers |
| Consultant | *(empty)* | Consultant | Consultants |
| Office | *(empty)* | BackOffice | BackOffice |

## Permission Matrix

Legend: **F** = FullAccess, **RW** = ReadWriteAccess, **CRUD** = CRUDAccess, **R** = Read-only, **X** = No access

| Entity | Admin | ProjectManager | Consultant | BackOffice |
|---|---|---|---|---|
| Client | F | R | R | F |
| ContactPerson | F | R | R | F |
| Address | F | R | R | F |
| Project | F | F | R | R |
| ProjectTask | F | F | R | R |
| ProjectAssignment | F | F | R | R |
| TimeEntry | F | RW | CRUD | R |
| ApplicationUser | F | own-only | own-only | own-only |

### Key Differences: RW vs CRUD vs FullAccess

- **CRUDAccess** = Create + Read + Write + Delete (no Navigate permission)
- **ReadWriteAccess** = Read + Write (no Create, no Delete)
- **FullAccess** = CRUD + Navigate

This means:
- **ProjectManager** can READ and WRITE (edit) TimeEntries but CANNOT create or delete them
- **Consultant** can Create, Read, Write, and Delete TimeEntries

## Findings & Gaps

### No owner filter on TimeEntries for Consultant

The Consultant role grants `CRUDAccess` on `TimeEntry` without any object-level criteria filter. This means a Consultant can see, edit, and delete ALL time entries from all users, not just their own. This is likely a security gap.

**Recommended fix:** Add an object permission with `User.ID == CurrentUserId()` criteria for Consultant's TimeEntry access.

### ProjectManager cannot create TimeEntries

`ReadWriteAccess` = Read + Write only. The PM can edit existing time entries (e.g., change status from Submitted to Approved/Rejected) but cannot create new ones or delete them. This seems intentional for an approval workflow but should be verified.

### No explicit Navigate permissions for entity types

Non-admin roles rely on `AddTypePermissionsRecursively` which includes Navigate for FullAccess but not for Read or ReadWriteAccess. This may affect whether entities appear in XAF navigation. For the MAUI app (API-only access) this does not matter.

---

## Test Checklist

### 1. Admin (user: "Admin")

**XAF Blazor:**
- [ ] Login succeeds
- [ ] All navigation items visible (Clients, Projects, Tasks, Time Entries, Users, Roles)
- [ ] Can CRUD all entity types
- [ ] Can access Role/User administration

**MAUI App:**
- [ ] Login via JWT succeeds
- [ ] All tabs accessible: Clients, Day Sheet, Projects, Reports, Settings

### 2. ProjectManager (user: "Manager")

**XAF Blazor:**
- [ ] Login succeeds
- [ ] Can see Projects, Tasks, Assignments in navigation
- [ ] Can create/edit/delete Projects
- [ ] Can create/edit/delete ProjectTasks
- [ ] Can create/edit/delete ProjectAssignments
- [ ] Can view Clients (read-only) — verify cannot create/edit/delete
- [ ] Can view ContactPersons (read-only)
- [ ] Can view TimeEntries — verify can edit (change status) but CANNOT create or delete
- [ ] Cannot access User/Role administration

**MAUI App:**
- [ ] Login via JWT succeeds
- [ ] Clients tab: list loads, detail view is read-only
- [ ] Day Sheet tab: can view time entries, verify cannot create new ones (RW, not CRUD)
- [ ] Projects tab: list loads, can view project details
- [ ] Reports tab: data loads correctly

**Expected denials:**
- [ ] POST to `/api/TimeEntry` returns 403 (no Create permission)
- [ ] DELETE to `/api/TimeEntry/{id}` returns 403 (no Delete permission)
- [ ] PUT/PATCH to `/api/Client/{id}` returns 403

### 3. Consultant (user: "Consultant")

**XAF Blazor:**
- [ ] Login succeeds
- [ ] Can view Clients, Projects, Tasks, Assignments (all read-only)
- [ ] Can CRUD TimeEntries (create, view, edit, delete)
- [ ] **BUG CHECK:** Can see/edit/delete OTHER users' time entries (no owner filter!)
- [ ] Cannot access User/Role administration

**MAUI App:**
- [ ] Login via JWT succeeds
- [ ] Clients tab: list loads, read-only
- [ ] Day Sheet tab: can create new time entry, edit own entries, delete own entries
- [ ] **BUG CHECK:** Can the user see time entries from Sarah/Mike? (expected: yes, due to missing owner filter)
- [ ] Projects tab: list loads, read-only detail views
- [ ] Reports tab: data loads

**Expected denials:**
- [ ] PUT/PATCH to `/api/Client/{id}` returns 403
- [ ] POST to `/api/Project` returns 403
- [ ] DELETE to `/api/Project/{id}` returns 403

### 4. BackOffice (user: "Office")

**XAF Blazor:**
- [ ] Login succeeds
- [ ] Can CRUD Clients, ContactPersons, Addresses
- [ ] Can view Projects, Tasks, Assignments (read-only)
- [ ] Can view TimeEntries (read-only)
- [ ] Cannot access User/Role administration

**MAUI App:**
- [ ] Login via JWT succeeds
- [ ] Clients tab: list loads, can create/edit clients
- [ ] Day Sheet tab: can view time entries but CANNOT create/edit/delete
- [ ] Projects tab: list loads, read-only
- [ ] Reports tab: data loads

**Expected denials:**
- [ ] POST to `/api/TimeEntry` returns 403
- [ ] PUT/PATCH to `/api/TimeEntry/{id}` returns 403
- [ ] POST to `/api/Project` returns 403

---

## API-Specific Tests (All Roles)

- [ ] `POST /api/Authentication/Authenticate` with correct username/empty password returns JWT
- [ ] `POST /api/Authentication/Authenticate` with wrong password returns 401
- [ ] API calls without `Authorization: Bearer <token>` header return 401
- [ ] API calls with expired token return 401

## Time Entry Workflow Tests

| Action | Admin | PM | Consultant | BackOffice |
|---|---|---|---|---|
| Create time entry | Yes | **No** | Yes | No |
| Edit own time entry | Yes | Yes | Yes | No |
| Edit others' time entry | Yes | Yes | **Yes (gap!)** | No |
| Delete time entry | Yes | **No** | Yes | No |
| Change status to Submitted | Yes | Yes | Yes | No |
| Change status to Approved | Yes | Yes | **Yes (gap!)** | No |
| Change status to Rejected | Yes | Yes | **Yes (gap!)** | No |

**Gaps marked above:** Consultant can approve/reject entries and edit other users' entries because there is no object-level filter or member-level restriction on the `Status` field.
