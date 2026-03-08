# Project Administration MVP Design

## Overview

MVP for a combined XAF Blazor Server + .NET MAUI project administration system. The XAF app is the full back-office; the MAUI app is a mobile companion for consultants to look up client info, log hours, and view reports. They share the same SQL Server database via XAF's built-in Web API (OData + JWT auth).

## Data Model

All custom entities use `BaseObjectInt` (int auto-increment key, `IXafEntityObject`, `IObjectSpaceLink`). `ApplicationUser` keeps its existing Guid key from `PermissionPolicyUser`.

### Entities

**BaseObjectInt** (abstract, copied from XafSearch project)
- `ID` (int, Key, hidden from UI)
- Implements `IXafEntityObject`, `IObjectSpaceLink`

**Client : BaseObjectInt**
- `Name` (string, required)
- `CompanyName` (string)
- `VatNumber` (string)
- `Email` (string)
- `Phone` (string)
- `BillingAddress` → Address (FK)
- `VisitAddress` → Address (FK)
- `ContactPersons` → IList\<ContactPerson\>

**Address : BaseObjectInt**
- `Street` (string)
- `City` (string)
- `PostalCode` (string)
- `Country` (string)

**ContactPerson : BaseObjectInt**
- `Name` (string, required)
- `Email` (string)
- `Phone` (string)
- `JobTitle` (string)
- `Client` → Client (FK)

**Project : BaseObjectInt**
- `Name` (string, required)
- `Description` (string)
- `Status` (enum: Draft, Active, OnHold, Completed, Archived)
- `StartDate` (DateTime?)
- `EndDate` (DateTime?)
- `BudgetHours` (decimal)
- `Client` → Client (FK)
- `ProjectTasks` → IList\<ProjectTask\>
- `ProjectAssignments` → IList\<ProjectAssignment\>

**ProjectTask : BaseObjectInt**
- `Name` (string, required)
- `Description` (string)
- `Status` (enum: Open, InProgress, Done)
- `EstimatedHours` (decimal)
- `SortOrder` (int)
- `Project` → Project (FK)
- `TimeEntries` → IList\<TimeEntry\>

**ProjectAssignment : BaseObjectInt**
- `Project` → Project (FK)
- `User` → ApplicationUser (FK)
- `Role` (enum: Manager, Member)

**TimeEntry : BaseObjectInt**
- `Date` (DateTime, required)
- `Hours` (decimal, required)
- `Note` (string)
- `Status` (enum: Draft, Submitted, Approved, Rejected)
- `User` → ApplicationUser (FK)
- `ProjectTask` → ProjectTask (FK)

**ApplicationUser** (extend existing, keeps Guid key)
- Add `DisplayName` (string)
- Add `Phone` (string)
- Add `AppRole` (enum: Admin, ProjectManager, Consultant, BackOffice)

### Constraints

- XAF EF Core does not support owned entity types (`OwnsOne`), so Address is a separate entity
- All entities use `virtual` auto-properties for EF Core change tracking proxies
- Collections use `ObservableCollection<T>`

## XAF Web API

The MAUI app communicates via XAF's built-in OData Web API with JWT authentication.

### Registered Business Objects

All entities registered in `Startup.cs` → `webApiBuilder.ConfigureOptions`:

| Resource | OData Endpoint | MAUI Usage |
|---|---|---|
| Client | `/api/odata/Client` | Search/read-only, `$expand` addresses & contacts |
| Address | `/api/odata/Address` | Via client expand |
| ContactPerson | `/api/odata/ContactPerson` | Via client expand |
| Project | `/api/odata/Project` | Read assigned projects |
| ProjectTask | `/api/odata/ProjectTask` | Read tasks per project |
| ProjectAssignment | `/api/odata/ProjectAssignment` | Determine user's projects |
| TimeEntry | `/api/odata/TimeEntry` | Full CRUD, scoped to current user |

### Authentication

- Existing `AuthenticationController` handles JWT login (`POST /api/Authentication/Authenticate`)
- JWT token returned on successful login
- All OData endpoints require `[Authorize]` with JWT bearer scheme

## MAUI App Structure

### Navigation

Bottom tab bar (`DXTabView`) with four sections:

```
TabBar
├── Clients     — search & view client info (read-only)
├── Day Sheet   — log hours per day
├── Projects    — view assigned projects & task progress
└── Reports     — charts & dashboards
```

### Screens

**1. Clients Tab**
- Client list with search/filter (`DXCollectionView` + `FilteringUI`)
- Client detail page: company info, billing/visit addresses, contact persons list
- All read-only

**2. Day Sheet Tab**
- Date picker at top (defaults to today)
- Editable list of time entries for selected date (`DXDataGrid`)
- Add row: cascading pick project → task, enter hours + note
- Submit button: changes status from Draft → Submitted
- Entries in Submitted/Approved/Rejected status are read-only

**3. Projects Tab**
- List of assigned projects (`DXCollectionView`, grouped by status)
- Tap project → task list with estimated vs logged hours per task
- Progress indicator per task

**4. Reports Tab**
- **My Week**: bar chart (`DXChartView`) — hours per day, current week
- **Project Progress**: select project → pie chart (task distribution by status) + bar chart (estimated vs actual hours per task)
- **Budget Burn**: gauge (`DXGaugeView`) showing % of budget consumed
- Date range filter

### DevExpress MAUI Controls Used

- `DXTabView` — bottom navigation
- `DXCollectionView` — client list, project list
- `DXDataGrid` — day sheet (editable time entries)
- `DXChartView` — bar charts, pie charts
- `DXGaugeView` — budget burn gauge
- `DateEdit`, `ComboBoxEdit`, `TextEdit` — form editors
- `FilteringUI` — client search

### API Communication Layer

- `HttpClient` with `DelegatingHandler` for JWT bearer token injection
- Service classes per entity: `ClientService`, `ProjectService`, `TimeEntryService`, etc.
- Token refresh on 401 response
- OData query building for `$filter`, `$expand`, `$orderby`

## SQLite Offline Storage

EF Core SQLite in the MAUI app with a separate local `MauiLocalDbContext`.

### Sync Strategy

| Entity | Direction | Offline Editable |
|---|---|---|
| Client, Address, ContactPerson | Server → Local | No |
| Project, ProjectTask | Server → Local | No |
| ProjectAssignment | Server → Local | No |
| TimeEntry | Bidirectional | Yes |
| User profile + JWT token | Stored locally | No |

### Sync Flow

1. **Login**: authenticate via API, store JWT in `SecureStorage`, download all assigned projects + related reference data to SQLite
2. **Time entries**: saved to SQLite immediately on create/edit, marked as pending sync
3. **Background sync**: pushes pending time entries to server when connectivity is available
4. **Pull-to-refresh**: on any list, re-downloads data from server
5. **Logout**: clears SQLite database and `SecureStorage`

### Conflict Handling

Last-write-wins for time entries. Conflicts are unlikely since time entries are user-scoped.

## Security & Roles

### XAF Permission Configuration

Roles configured in `Updater.cs` database seeder:

| Role | Blazor App | MAUI App / API |
|---|---|---|
| **Admin** | Full CRUD everything, user/role management | Full API access |
| **ProjectManager** | CRUD projects/tasks, assign team, approve/reject time entries, all reports | Read all assigned data, manage own projects' time entries |
| **Consultant** | View assigned projects (read-only), own time entries, client lookup | Read assigned projects/clients, CRUD own time entries |
| **BackOffice** | CRUD clients/contacts, reporting, view all time entries | Read clients/projects/reports |

### MAUI App Security

- JWT token stored in `SecureStorage` (platform-encrypted)
- Token attached to all HTTP requests via `DelegatingHandler`
- App role determines visible tabs/features
- SQLite database cleared on logout

### Key Business Rules

- TimeEntry: users can only create/edit their own entries while status is `Draft`
- Submitted/Approved/Rejected entries are read-only for the entry owner
- ProjectManagers can approve/reject time entries for projects they manage
- Client data is read-only for Consultant and ProjectManager roles
- Only Admin can manage users and roles

## Out of Scope (Future Phases)

- Invoicing and billing
- Email notifications
- File attachments on projects/tasks
- Multi-language support
- Approval workflow notifications (push notifications)
