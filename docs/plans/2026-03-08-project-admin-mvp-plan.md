# Project Administration MVP Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a project administration system with XAF Blazor Server back-office and .NET MAUI mobile companion for time tracking, client lookup, and reporting.

**Architecture:** XAF Blazor Server app with EF Core provides full CRUD and OData Web API. MAUI app consumes the API via JWT auth, caches data in SQLite for offline use, and uses DevExpress MAUI controls for all UI. Shared `BaseObjectInt` base class with int primary keys.

**Tech Stack:** .NET 9 (XAF), .NET 10 (MAUI), DevExpress XAF 25.2.3, DevExpress MAUI 25.2.*, EF Core, SQL Server, SQLite, OData, JWT

---

## Phase 1: XAF Data Model & Business Objects

### Task 1: Add BaseObjectInt base class

**Files:**
- Create: `XafMaui.Module/BusinessObjects/BaseObjectInt.cs`

**Step 1: Create BaseObjectInt**

```csharp
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

public abstract class BaseObjectInt : IXafEntityObject, IObjectSpaceLink
{
    protected IObjectSpace ObjectSpace;

    [Key]
    [VisibleInListView(false)]
    [VisibleInDetailView(false)]
    [VisibleInLookupListView(false)]
    public virtual int ID { get; set; }

    IObjectSpace IObjectSpaceLink.ObjectSpace
    {
        get => ObjectSpace;
        set => ObjectSpace = value;
    }

    public virtual void OnCreated() { }
    public virtual void OnSaving() { }
    public virtual void OnLoaded() { }
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded, 0 errors

**Step 3: Commit**

```bash
git add XafMaui.Module/BusinessObjects/BaseObjectInt.cs
git commit -m "feat: add BaseObjectInt base class with int primary key"
```

---

### Task 2: Add enums

**Files:**
- Create: `XafMaui.Module/BusinessObjects/Enums.cs`

**Step 1: Create enum file**

```csharp
namespace XafMaui.Module.BusinessObjects;

public enum ProjectStatus
{
    Draft,
    Active,
    OnHold,
    Completed,
    Archived
}

public enum ProjectTaskStatus
{
    Open,
    InProgress,
    Done
}

public enum TimeEntryStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected
}

public enum ProjectRole
{
    Manager,
    Member
}

public enum AppRole
{
    Admin,
    ProjectManager,
    Consultant,
    BackOffice
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Module/BusinessObjects/Enums.cs
git commit -m "feat: add business enums for project, task, time entry status and roles"
```

---

### Task 3: Add Address entity

**Files:**
- Create: `XafMaui.Module/BusinessObjects/Address.cs`

**Step 1: Create Address**

```csharp
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(City))]
public class Address : BaseObjectInt
{
    public virtual string? Street { get; set; }
    public virtual string? City { get; set; }
    public virtual string? PostalCode { get; set; }
    public virtual string? Country { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string FullAddress => string.Join(", ",
        new[] { Street, PostalCode, City, Country }
        .Where(s => !string.IsNullOrWhiteSpace(s)));
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Module/BusinessObjects/Address.cs
git commit -m "feat: add Address entity"
```

---

### Task 4: Add Client and ContactPerson entities

**Files:**
- Create: `XafMaui.Module/BusinessObjects/Client.cs`
- Create: `XafMaui.Module/BusinessObjects/ContactPerson.cs`

**Step 1: Create Client**

```csharp
using DevExpress.Persistent.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class Client : BaseObjectInt
{
    [Required]
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? CompanyName { get; set; }
    public virtual string? VatNumber { get; set; }
    public virtual string? Email { get; set; }
    public virtual string? Phone { get; set; }

    public virtual Address? BillingAddress { get; set; }
    public virtual Address? VisitAddress { get; set; }

    [DevExpress.ExpressApp.DC.Aggregated]
    public virtual IList<ContactPerson> ContactPersons { get; set; } = new ObservableCollection<ContactPerson>();
}
```

**Step 2: Create ContactPerson**

```csharp
using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class ContactPerson : BaseObjectInt
{
    [Required]
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? Email { get; set; }
    public virtual string? Phone { get; set; }
    public virtual string? JobTitle { get; set; }

    public virtual Client? Client { get; set; }
}
```

**Step 3: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add XafMaui.Module/BusinessObjects/Client.cs XafMaui.Module/BusinessObjects/ContactPerson.cs
git commit -m "feat: add Client and ContactPerson entities"
```

---

### Task 5: Add Project and ProjectTask entities

**Files:**
- Create: `XafMaui.Module/BusinessObjects/Project.cs`
- Create: `XafMaui.Module/BusinessObjects/ProjectTask.cs`

**Step 1: Create Project**

```csharp
using DevExpress.Persistent.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class Project : BaseObjectInt
{
    [Required]
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? Description { get; set; }
    public virtual ProjectStatus Status { get; set; }
    public virtual DateTime? StartDate { get; set; }
    public virtual DateTime? EndDate { get; set; }
    public virtual decimal BudgetHours { get; set; }

    public virtual Client? Client { get; set; }

    [DevExpress.ExpressApp.DC.Aggregated]
    public virtual IList<ProjectTask> ProjectTasks { get; set; } = new ObservableCollection<ProjectTask>();
    public virtual IList<ProjectAssignment> ProjectAssignments { get; set; } = new ObservableCollection<ProjectAssignment>();
}
```

**Step 2: Create ProjectTask**

```csharp
using DevExpress.Persistent.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class ProjectTask : BaseObjectInt
{
    [Required]
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? Description { get; set; }
    public virtual ProjectTaskStatus Status { get; set; }
    public virtual decimal EstimatedHours { get; set; }
    public virtual int SortOrder { get; set; }

    public virtual Project? Project { get; set; }

    public virtual IList<TimeEntry> TimeEntries { get; set; } = new ObservableCollection<TimeEntry>();
}
```

**Step 3: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add XafMaui.Module/BusinessObjects/Project.cs XafMaui.Module/BusinessObjects/ProjectTask.cs
git commit -m "feat: add Project and ProjectTask entities"
```

---

### Task 6: Add ProjectAssignment and TimeEntry entities

**Files:**
- Create: `XafMaui.Module/BusinessObjects/ProjectAssignment.cs`
- Create: `XafMaui.Module/BusinessObjects/TimeEntry.cs`

**Step 1: Create ProjectAssignment**

```csharp
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
public class ProjectAssignment : BaseObjectInt
{
    public virtual Project? Project { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ProjectRole Role { get; set; }
}
```

**Step 2: Create TimeEntry**

```csharp
using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Date))]
public class TimeEntry : BaseObjectInt
{
    [Required]
    public virtual DateTime Date { get; set; }
    [Required]
    public virtual decimal Hours { get; set; }
    public virtual string? Note { get; set; }
    public virtual TimeEntryStatus Status { get; set; }

    public virtual ApplicationUser? User { get; set; }
    public virtual ProjectTask? ProjectTask { get; set; }
}
```

**Step 3: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add XafMaui.Module/BusinessObjects/ProjectAssignment.cs XafMaui.Module/BusinessObjects/TimeEntry.cs
git commit -m "feat: add ProjectAssignment and TimeEntry entities"
```

---

### Task 7: Extend ApplicationUser with new properties

**Files:**
- Modify: `XafMaui.Module/BusinessObjects/ApplicationUser.cs`

**Step 1: Add DisplayName, Phone, and AppRole properties**

Add the following properties to the `ApplicationUser` class after the existing properties:

```csharp
public virtual string? DisplayName { get; set; }
public virtual string? Phone { get; set; }
public virtual AppRole AppRole { get; set; }
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Module/BusinessObjects/ApplicationUser.cs
git commit -m "feat: extend ApplicationUser with DisplayName, Phone, AppRole"
```

---

### Task 8: Register entities in DbContext

**Files:**
- Modify: `XafMaui.Module/BusinessObjects/XafMauiDbContext.cs`

**Step 1: Add DbSet properties**

Add the following after the existing `DbSet` properties in `XafMauiEFCoreDbContext`:

```csharp
public DbSet<Address> Addresses { get; set; }
public DbSet<Client> Clients { get; set; }
public DbSet<ContactPerson> ContactPersons { get; set; }
public DbSet<Project> Projects { get; set; }
public DbSet<ProjectTask> ProjectTasks { get; set; }
public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
public DbSet<TimeEntry> TimeEntries { get; set; }
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Module/BusinessObjects/XafMauiDbContext.cs
git commit -m "feat: register all new entities in DbContext"
```

---

### Task 9: Fix AuthenticationController Swashbuckle reference

The `AuthenticationController.cs` still uses `Swashbuckle.AspNetCore.Annotations` which was removed. Replace with standard attributes.

**Files:**
- Modify: `XafMaui.Blazor.Server/API/Security/AuthenticationController.cs`

**Step 1: Replace Swashbuckle annotations**

Replace the entire file content:

```csharp
using DevExpress.ExpressApp.Security.Authentication;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using Microsoft.AspNetCore.Mvc;

namespace XafMaui.WebApi.JWT
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        readonly IAuthenticationTokenProvider tokenProvider;
        public AuthenticationController(IAuthenticationTokenProvider tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(
            [FromBody] AuthenticationStandardLogonParameters logonParameters)
        {
            try
            {
                return Ok(tokenProvider.Authenticate(logonParameters));
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.GetJson());
            }
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Blazor.Server/API/Security/AuthenticationController.cs
git commit -m "fix: remove Swashbuckle annotations from AuthenticationController"
```

---

## Phase 2: XAF Web API & Security Configuration

### Task 10: Register business objects in Web API

**Files:**
- Modify: `XafMaui.Blazor.Server/Startup.cs`

**Step 1: Add business object registrations**

In `Startup.cs`, find the `webApiBuilder.ConfigureOptions` section (around line 50-54) and replace the commented placeholder with:

```csharp
webApiBuilder.ConfigureOptions(options =>
{
    options.BusinessObject<XafMaui.Module.BusinessObjects.Address>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.Client>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.ContactPerson>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.Project>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.ProjectTask>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.ProjectAssignment>();
    options.BusinessObject<XafMaui.Module.BusinessObjects.TimeEntry>();
});
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Blazor.Server/Startup.cs
git commit -m "feat: register business objects in XAF Web API"
```

---

### Task 11: Configure security roles in Updater.cs

**Files:**
- Modify: `XafMaui.Module/DatabaseUpdate/Updater.cs`

**Step 1: Add role creation methods and seed users**

Replace the entire `Updater.cs` with the expanded version that creates all four roles (Admin, ProjectManager, Consultant, BackOffice) and seed users:

```csharp
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EF;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.Extensions.DependencyInjection;
using XafMaui.Module.BusinessObjects;

namespace XafMaui.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) { }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

#if !RELEASE
            var adminRole = CreateAdminRole();
            var pmRole = CreateProjectManagerRole();
            var consultantRole = CreateConsultantRole();
            var backOfficeRole = CreateBackOfficeRole();

            ObjectSpace.CommitChanges();

            UserManager userManager = ObjectSpace.ServiceProvider.GetRequiredService<UserManager>();

            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Admin") == null)
            {
                _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Admin", "", (user) =>
                {
                    user.Roles.Add(adminRole);
                    user.AppRole = AppRole.Admin;
                });
            }

            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Manager") == null)
            {
                _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Manager", "", (user) =>
                {
                    user.Roles.Add(pmRole);
                    user.AppRole = AppRole.ProjectManager;
                    user.DisplayName = "Project Manager";
                });
            }

            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Consultant") == null)
            {
                _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Consultant", "", (user) =>
                {
                    user.Roles.Add(consultantRole);
                    user.AppRole = AppRole.Consultant;
                    user.DisplayName = "Test Consultant";
                });
            }

            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Office") == null)
            {
                _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Office", "", (user) =>
                {
                    user.Roles.Add(backOfficeRole);
                    user.AppRole = AppRole.BackOffice;
                    user.DisplayName = "Back Office";
                });
            }

            ObjectSpace.CommitChanges();
#endif
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }

        PermissionPolicyRole CreateAdminRole()
        {
            PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Administrators";
                role.IsAdministrative = true;
            }
            return role;
        }

        PermissionPolicyRole CreateProjectManagerRole()
        {
            PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "ProjectManagers");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "ProjectManagers";

                // Full access to projects and tasks
                role.AddTypePermissionsRecursively<Project>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

                // Read + approve time entries
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                // Read clients
                role.AddTypePermissionsRecursively<Client>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.Read, SecurityPermissionState.Allow);

                // Own user record
                role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);

                // Model differences
                role.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            }
            return role;
        }

        PermissionPolicyRole CreateConsultantRole()
        {
            PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Consultants");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Consultants";

                // Read clients
                role.AddTypePermissionsRecursively<Client>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.Read, SecurityPermissionState.Allow);

                // Read projects/tasks
                role.AddTypePermissionsRecursively<Project>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.Read, SecurityPermissionState.Allow);

                // Own time entries: full CRUD
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);

                // Own user record
                role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);

                // Model differences
                role.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            }
            return role;
        }

        PermissionPolicyRole CreateBackOfficeRole()
        {
            PermissionPolicyRole role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "BackOffice");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "BackOffice";

                // Full access to clients
                role.AddTypePermissionsRecursively<Client>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);

                // Read projects, tasks, time entries (for reporting)
                role.AddTypePermissionsRecursively<Project>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.Read, SecurityPermissionState.Allow);

                // Own user record
                role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
                role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);

                // Model differences
                role.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            }
            return role;
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui.Xaf.slnx`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui.Module/DatabaseUpdate/Updater.cs
git commit -m "feat: configure security roles and seed users for all four roles"
```

---

### Task 12: Run XAF app and verify database creation

**Step 1: Start the Docker SQL Server container**

Run: `docker start xafmaui-sql`

**Step 2: Run the XAF app**

Run: `dotnet run --project XafMaui.Blazor.Server`

Expected: App starts, auto-creates database tables, seeds roles and users. Accessible at `https://localhost:5001` or similar.

**Step 3: Verify via Scalar**

Navigate to `/scalar` in the browser. Confirm all OData endpoints are listed:
- `/api/odata/Client`
- `/api/odata/Project`
- `/api/odata/ProjectTask`
- `/api/odata/TimeEntry`
- `/api/odata/ProjectAssignment`
- `/api/odata/Address`
- `/api/odata/ContactPerson`

**Step 4: Test login**

Use Scalar or curl to POST to `/api/Authentication/Authenticate`:
```json
{ "userName": "Admin", "password": "" }
```
Expected: JWT token returned.

---

## Phase 3: MAUI App — Foundation & API Layer

### Task 13: Add required NuGet packages to MAUI project

**Files:**
- Modify: `XafMaui/XafMaui.csproj`

**Step 1: Add SQLite EF Core, System.Net.Http.Json packages**

Add to the `<ItemGroup>` with PackageReferences:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.14.0" />
```

Note: `System.Net.Http.Json` and `Microsoft.Maui.Essentials` (for SecureStorage) are included by default in MAUI.

**Step 2: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui/XafMaui.csproj
git commit -m "feat: add SQLite EF Core package to MAUI project"
```

---

### Task 14: Create MAUI DTOs (local models)

**Files:**
- Create: `XafMaui/Models/ClientDto.cs`
- Create: `XafMaui/Models/ProjectDto.cs`
- Create: `XafMaui/Models/TimeEntryDto.cs`
- Create: `XafMaui/Models/Enums.cs`

**Step 1: Create Enums (duplicated for MAUI, no shared project yet)**

```csharp
namespace XafMaui.Models;

public enum ProjectStatus { Draft, Active, OnHold, Completed, Archived }
public enum ProjectTaskStatus { Open, InProgress, Done }
public enum TimeEntryStatus { Draft, Submitted, Approved, Rejected }
public enum ProjectRole { Manager, Member }
public enum AppRole { Admin, ProjectManager, Consultant, BackOffice }
```

**Step 2: Create ClientDto**

```csharp
namespace XafMaui.Models;

public class ClientDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? VatNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public AddressDto? BillingAddress { get; set; }
    public AddressDto? VisitAddress { get; set; }
    public List<ContactPersonDto> ContactPersons { get; set; } = [];
}

public class AddressDto
{
    public int ID { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class ContactPersonDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
}
```

**Step 3: Create ProjectDto**

```csharp
namespace XafMaui.Models;

public class ProjectDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BudgetHours { get; set; }
    public int? ClientID { get; set; }
    public string? ClientName { get; set; }
    public List<ProjectTaskDto> ProjectTasks { get; set; } = [];
}

public class ProjectTaskDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public decimal EstimatedHours { get; set; }
    public int SortOrder { get; set; }
    public int ProjectID { get; set; }
    public decimal LoggedHours { get; set; } // Calculated client-side
}

public class ProjectAssignmentDto
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public ProjectRole Role { get; set; }
}
```

**Step 4: Create TimeEntryDto**

```csharp
namespace XafMaui.Models;

public class TimeEntryDto
{
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Note { get; set; }
    public TimeEntryStatus Status { get; set; }
    public int ProjectTaskID { get; set; }
    public string? ProjectTaskName { get; set; }
    public string? ProjectName { get; set; }

    // Local sync tracking
    public bool IsPendingSync { get; set; }
    public bool IsLocalOnly { get; set; }
}
```

**Step 5: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 6: Commit**

```bash
git add XafMaui/Models/
git commit -m "feat: add MAUI DTO models for API communication and local storage"
```

---

### Task 15: Create SQLite local database context

**Files:**
- Create: `XafMaui/Data/LocalDbContext.cs`
- Create: `XafMaui/Data/LocalEntities.cs`

**Step 1: Create local SQLite entities**

```csharp
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Data;

public class LocalClient
{
    [Key] public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? VatNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? BillingStreet { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }
    public string? VisitStreet { get; set; }
    public string? VisitCity { get; set; }
    public string? VisitPostalCode { get; set; }
    public string? VisitCountry { get; set; }
}

public class LocalContactPerson
{
    [Key] public int ID { get; set; }
    public int ClientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
}

public class LocalProject
{
    [Key] public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BudgetHours { get; set; }
    public int? ClientID { get; set; }
    public string? ClientName { get; set; }
}

public class LocalProjectTask
{
    [Key] public int ID { get; set; }
    public int ProjectID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public decimal EstimatedHours { get; set; }
    public int SortOrder { get; set; }
}

public class LocalTimeEntry
{
    [Key] public int LocalId { get; set; } // Auto-increment local ID
    public int? ServerID { get; set; }      // Null if not yet synced
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Note { get; set; }
    public int Status { get; set; }
    public int ProjectTaskID { get; set; }
    public string? ProjectTaskName { get; set; }
    public string? ProjectName { get; set; }
    public bool IsPendingSync { get; set; }
}
```

**Step 2: Create LocalDbContext**

```csharp
using Microsoft.EntityFrameworkCore;

namespace XafMaui.Data;

public class LocalDbContext : DbContext
{
    public DbSet<LocalClient> Clients { get; set; }
    public DbSet<LocalContactPerson> ContactPersons { get; set; }
    public DbSet<LocalProject> Projects { get; set; }
    public DbSet<LocalProjectTask> ProjectTasks { get; set; }
    public DbSet<LocalTimeEntry> TimeEntries { get; set; }

    public LocalDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "xafmaui.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
```

**Step 3: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 4: Commit**

```bash
git add XafMaui/Data/
git commit -m "feat: add SQLite local database context and entities for offline storage"
```

---

### Task 16: Create API client service

**Files:**
- Create: `XafMaui/Services/ApiConfig.cs`
- Create: `XafMaui/Services/AuthService.cs`
- Create: `XafMaui/Services/ApiClient.cs`

**Step 1: Create ApiConfig**

```csharp
namespace XafMaui.Services;

public static class ApiConfig
{
    // For Android emulator, 10.0.2.2 maps to host localhost
    // For iOS simulator, localhost works directly
    // For physical devices, use the actual machine IP
#if ANDROID
    public const string BaseUrl = "https://10.0.2.2:5001";
#else
    public const string BaseUrl = "https://localhost:5001";
#endif
}
```

**Step 2: Create AuthService**

```csharp
using System.Net.Http.Json;

namespace XafMaui.Services;

public class AuthService
{
    readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> LoginAsync(string userName, string password)
    {
        var response = await _http.PostAsJsonAsync(
            $"{ApiConfig.BaseUrl}/api/Authentication/Authenticate",
            new { userName, password });

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await response.Content.ReadAsStringAsync();
        await SecureStorage.SetAsync("jwt_token", token);
        return token;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("jwt_token");
    }

    public void Logout()
    {
        SecureStorage.Remove("jwt_token");
    }
}
```

**Step 3: Create ApiClient**

```csharp
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace XafMaui.Services;

public class ApiClient
{
    readonly HttpClient _http;
    readonly AuthService _auth;

    public ApiClient(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    async Task EnsureAuthHeaderAsync()
    {
        var token = await _auth.GetTokenAsync();
        if (token != null)
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<T>> GetListAsync<T>(string odataPath, string? queryParams = null)
    {
        await EnsureAuthHeaderAsync();
        var url = $"{ApiConfig.BaseUrl}/api/odata/{odataPath}";
        if (queryParams != null)
            url += $"?{queryParams}";

        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ODataResponse<T>>();
        return result?.Value ?? [];
    }

    public async Task<T?> PostAsync<T>(string odataPath, T entity)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}", entity);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PutAsync<T>(string odataPath, int id, T entity)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}/{id}", entity);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string odataPath, int id)
    {
        await EnsureAuthHeaderAsync();
        var response = await _http.DeleteAsync($"{ApiConfig.BaseUrl}/api/odata/{odataPath}/{id}");
        response.EnsureSuccessStatusCode();
    }
}

public class ODataResponse<T>
{
    public List<T> Value { get; set; } = [];
}
```

**Step 4: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add XafMaui/Services/
git commit -m "feat: add API client, auth service, and config for XAF Web API communication"
```

---

### Task 17: Create sync service

**Files:**
- Create: `XafMaui/Services/SyncService.cs`

**Step 1: Create SyncService for downloading reference data and syncing time entries**

```csharp
using XafMaui.Data;
using XafMaui.Models;

namespace XafMaui.Services;

public class SyncService
{
    readonly ApiClient _api;

    public SyncService(ApiClient api)
    {
        _api = api;
    }

    public async Task SyncAllAsync()
    {
        await SyncClientsAsync();
        await SyncProjectsAsync();
        await PushPendingTimeEntriesAsync();
        await PullTimeEntriesAsync();
    }

    public async Task SyncClientsAsync()
    {
        var clients = await _api.GetListAsync<ClientDto>("Client", "$expand=BillingAddress,VisitAddress,ContactPersons");
        using var db = new LocalDbContext();

        db.ContactPersons.RemoveRange(db.ContactPersons);
        db.Clients.RemoveRange(db.Clients);
        await db.SaveChangesAsync();

        foreach (var c in clients)
        {
            db.Clients.Add(new LocalClient
            {
                ID = c.ID, Name = c.Name, CompanyName = c.CompanyName,
                VatNumber = c.VatNumber, Email = c.Email, Phone = c.Phone,
                BillingStreet = c.BillingAddress?.Street, BillingCity = c.BillingAddress?.City,
                BillingPostalCode = c.BillingAddress?.PostalCode, BillingCountry = c.BillingAddress?.Country,
                VisitStreet = c.VisitAddress?.Street, VisitCity = c.VisitAddress?.City,
                VisitPostalCode = c.VisitAddress?.PostalCode, VisitCountry = c.VisitAddress?.Country,
            });
            foreach (var cp in c.ContactPersons)
            {
                db.ContactPersons.Add(new LocalContactPerson
                {
                    ID = cp.ID, ClientID = c.ID, Name = cp.Name,
                    Email = cp.Email, Phone = cp.Phone, JobTitle = cp.JobTitle,
                });
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task SyncProjectsAsync()
    {
        var projects = await _api.GetListAsync<ProjectDto>("Project", "$expand=ProjectTasks");
        using var db = new LocalDbContext();

        db.ProjectTasks.RemoveRange(db.ProjectTasks);
        db.Projects.RemoveRange(db.Projects);
        await db.SaveChangesAsync();

        foreach (var p in projects)
        {
            db.Projects.Add(new LocalProject
            {
                ID = p.ID, Name = p.Name, Description = p.Description,
                Status = (int)p.Status, StartDate = p.StartDate, EndDate = p.EndDate,
                BudgetHours = p.BudgetHours, ClientID = p.ClientID, ClientName = p.ClientName,
            });
            foreach (var t in p.ProjectTasks)
            {
                db.ProjectTasks.Add(new LocalProjectTask
                {
                    ID = t.ID, ProjectID = p.ID, Name = t.Name,
                    Description = t.Description, Status = (int)t.Status,
                    EstimatedHours = t.EstimatedHours, SortOrder = t.SortOrder,
                });
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task PushPendingTimeEntriesAsync()
    {
        using var db = new LocalDbContext();
        var pending = db.TimeEntries.Where(t => t.IsPendingSync).ToList();

        foreach (var entry in pending)
        {
            var dto = new TimeEntryDto
            {
                Date = entry.Date, Hours = entry.Hours, Note = entry.Note,
                Status = (TimeEntryStatus)entry.Status, ProjectTaskID = entry.ProjectTaskID,
            };

            if (entry.ServerID == null)
            {
                var created = await _api.PostAsync("TimeEntry", dto);
                if (created != null)
                    entry.ServerID = created.ID;
            }
            else
            {
                dto.ID = entry.ServerID.Value;
                await _api.PutAsync("TimeEntry", dto.ID, dto);
            }
            entry.IsPendingSync = false;
        }
        await db.SaveChangesAsync();
    }

    public async Task PullTimeEntriesAsync()
    {
        var entries = await _api.GetListAsync<TimeEntryDto>("TimeEntry", "$orderby=Date desc&$top=200");
        using var db = new LocalDbContext();

        // Only update non-pending entries
        var nonPending = db.TimeEntries.Where(t => !t.IsPendingSync).ToList();
        db.TimeEntries.RemoveRange(nonPending);

        foreach (var e in entries)
        {
            if (!db.TimeEntries.Any(t => t.ServerID == e.ID))
            {
                db.TimeEntries.Add(new LocalTimeEntry
                {
                    ServerID = e.ID, Date = e.Date, Hours = e.Hours, Note = e.Note,
                    Status = (int)e.Status, ProjectTaskID = e.ProjectTaskID,
                    ProjectTaskName = e.ProjectTaskName, ProjectName = e.ProjectName,
                    IsPendingSync = false,
                });
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task ClearLocalDataAsync()
    {
        using var db = new LocalDbContext();
        db.TimeEntries.RemoveRange(db.TimeEntries);
        db.ProjectTasks.RemoveRange(db.ProjectTasks);
        db.Projects.RemoveRange(db.Projects);
        db.ContactPersons.RemoveRange(db.ContactPersons);
        db.Clients.RemoveRange(db.Clients);
        await db.SaveChangesAsync();
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui/Services/SyncService.cs
git commit -m "feat: add sync service for offline data management"
```

---

### Task 18: Register services in MauiProgram.cs

**Files:**
- Modify: `XafMaui/MauiProgram.cs`

**Step 1: Add DI registrations**

Add after the `.ConfigureFonts(...)` block, before `return builder.Build();`:

```csharp
// HTTP client that ignores dev cert validation (development only)
builder.Services.AddSingleton(_ =>
{
    var handler = new HttpClientHandler();
#if DEBUG
    handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif
    return new HttpClient(handler);
});

builder.Services.AddSingleton<XafMaui.Services.AuthService>();
builder.Services.AddSingleton<XafMaui.Services.ApiClient>();
builder.Services.AddSingleton<XafMaui.Services.SyncService>();
```

Add `using XafMaui.Services;` at the top if not using fully qualified names.

**Step 2: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 3: Commit**

```bash
git add XafMaui/MauiProgram.cs
git commit -m "feat: register API and sync services in MAUI DI container"
```

---

## Phase 4: MAUI App — UI Screens

### Task 19: Create Login page

**Files:**
- Create: `XafMaui/Views/LoginPage.xaml`
- Create: `XafMaui/Views/LoginPage.xaml.cs`
- Modify: `XafMaui/App.xaml.cs`

**Step 1: Create LoginPage.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="XafMaui.Views.LoginPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:dx="http://schemas.devexpress.com/maui"
    Title="Login"
    Shell.NavBarIsVisible="False">

    <Grid RowDefinitions="2*,3*,2*" Padding="32">
        <dx:DXStackLayout Grid.Row="1" ItemSpacing="16">
            <Label
                FontAttributes="Bold"
                FontFamily="Roboto Medium"
                FontSize="28"
                Text="XafMaui"
                HorizontalOptions="Center"
                TextColor="{dx:ThemeColor Key=Primary}" />
            <dx:TextEdit
                x:Name="userNameEdit"
                LabelText="User Name"
                PlaceholderText="Enter user name" />
            <dx:PasswordEdit
                x:Name="passwordEdit"
                LabelText="Password"
                PlaceholderText="Enter password" />
            <Label
                x:Name="errorLabel"
                TextColor="Red"
                IsVisible="False"
                HorizontalOptions="Center" />
            <dx:DXButton
                x:Name="loginButton"
                Content="Sign In"
                Clicked="OnLoginClicked" />
        </dx:DXStackLayout>
    </Grid>
</ContentPage>
```

**Step 2: Create LoginPage.xaml.cs**

```csharp
using XafMaui.Services;

namespace XafMaui.Views;

public partial class LoginPage : ContentPage
{
    readonly AuthService _auth;
    readonly SyncService _sync;

    public LoginPage(AuthService auth, SyncService sync)
    {
        InitializeComponent();
        _auth = auth;
        _sync = sync;
    }

    async void OnLoginClicked(object sender, EventArgs e)
    {
        errorLabel.IsVisible = false;
        loginButton.IsEnabled = false;

        var token = await _auth.LoginAsync(
            userNameEdit.Text ?? "",
            passwordEdit.Text ?? "");

        if (token != null)
        {
            await _sync.SyncAllAsync();
            Application.Current!.Windows[0].Page = new AppShell();
        }
        else
        {
            errorLabel.Text = "Invalid credentials";
            errorLabel.IsVisible = true;
        }

        loginButton.IsEnabled = true;
    }
}
```

**Step 3: Modify App.xaml.cs to start with LoginPage**

```csharp
using XafMaui.Views;

namespace XafMaui
{
    public partial class App : Application
    {
        readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_services.GetRequiredService<LoginPage>());
        }
    }
}
```

Register LoginPage in DI (in MauiProgram.cs, add):
```csharp
builder.Services.AddTransient<LoginPage>();
```

**Step 4: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add XafMaui/Views/LoginPage.xaml XafMaui/Views/LoginPage.xaml.cs XafMaui/App.xaml.cs XafMaui/MauiProgram.cs
git commit -m "feat: add login page with JWT authentication"
```

---

### Task 20: Restructure AppShell with tab navigation

**Files:**
- Modify: `XafMaui/Views/AppShell.xaml`
- Modify: `XafMaui/Views/AppShell.xaml.cs`

**Step 1: Replace AppShell.xaml with tabbed layout**

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="XafMaui.Views.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:dx="http://schemas.devexpress.com/maui"
    xmlns:views="clr-namespace:XafMaui.Views">

    <TabBar>
        <ShellContent Title="Clients" Icon="person" ContentTemplate="{DataTemplate views:ClientsPage}" />
        <ShellContent Title="Day Sheet" Icon="calendar" ContentTemplate="{DataTemplate views:DaySheetPage}" />
        <ShellContent Title="Projects" Icon="folder" ContentTemplate="{DataTemplate views:ProjectsPage}" />
        <ShellContent Title="Reports" Icon="chart" ContentTemplate="{DataTemplate views:ReportsPage}" />
    </TabBar>
</Shell>
```

**Step 2: Build will fail** — the referenced pages don't exist yet. Create stubs in the following tasks.

**Step 3: Commit (after all stubs are created in Task 21)**

---

### Task 21: Create stub pages for all four tabs

**Files:**
- Create: `XafMaui/Views/ClientsPage.xaml` + `.xaml.cs`
- Create: `XafMaui/Views/DaySheetPage.xaml` + `.xaml.cs`
- Create: `XafMaui/Views/ProjectsPage.xaml` + `.xaml.cs`
- Create: `XafMaui/Views/ReportsPage.xaml` + `.xaml.cs`
- Remove: `XafMaui/Views/DefaultPage.xaml` + `.xaml.cs`

**Step 1: Create ClientsPage stub**

`ClientsPage.xaml`:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="XafMaui.Views.ClientsPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:dx="http://schemas.devexpress.com/maui"
    Title="Clients">
    <Label Text="Clients — coming soon" VerticalOptions="Center" HorizontalOptions="Center" />
</ContentPage>
```

`ClientsPage.xaml.cs`:
```csharp
namespace XafMaui.Views;

public partial class ClientsPage : ContentPage
{
    public ClientsPage()
    {
        InitializeComponent();
    }
}
```

**Step 2: Create DaySheetPage stub** (same pattern, Title="Day Sheet")

**Step 3: Create ProjectsPage stub** (same pattern, Title="Projects")

**Step 4: Create ReportsPage stub** (same pattern, Title="Reports")

**Step 5: Delete DefaultPage.xaml and DefaultPage.xaml.cs**

**Step 6: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 7: Commit**

```bash
git add XafMaui/Views/
git rm XafMaui/Views/DefaultPage.xaml XafMaui/Views/DefaultPage.xaml.cs
git commit -m "feat: restructure MAUI app with tabbed navigation (Clients, Day Sheet, Projects, Reports)"
```

---

### Task 22: Build Clients tab with search and detail

**Files:**
- Modify: `XafMaui/Views/ClientsPage.xaml` + `.xaml.cs`
- Create: `XafMaui/Views/ClientDetailPage.xaml` + `.xaml.cs`
- Create: `XafMaui/ViewModels/ClientsViewModel.cs`

**Step 1: Create ClientsViewModel**

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XafMaui.Data;

namespace XafMaui.ViewModels;

public class ClientsViewModel : INotifyPropertyChanged
{
    string _searchText = string.Empty;
    List<LocalClient> _allClients = [];

    public ObservableCollection<LocalClient> Clients { get; } = [];

    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); FilterClients(); }
    }

    public void LoadClients()
    {
        using var db = new LocalDbContext();
        _allClients = db.Clients.OrderBy(c => c.Name).ToList();
        FilterClients();
    }

    void FilterClients()
    {
        Clients.Clear();
        var filtered = string.IsNullOrWhiteSpace(_searchText)
            ? _allClients
            : _allClients.Where(c =>
                c.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                (c.CompanyName?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Email?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var c in filtered)
            Clients.Add(c);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

**Step 2: Update ClientsPage.xaml** with DXCollectionView + search

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    x:Class="XafMaui.Views.ClientsPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:dx="http://schemas.devexpress.com/maui"
    xmlns:vm="clr-namespace:XafMaui.ViewModels"
    Title="Clients">

    <ContentPage.BindingContext>
        <vm:ClientsViewModel />
    </ContentPage.BindingContext>

    <Grid RowDefinitions="Auto,*" Padding="8">
        <dx:TextEdit
            Grid.Row="0"
            PlaceholderText="Search clients..."
            Text="{Binding SearchText}"
            Margin="0,0,0,8" />

        <dx:DXCollectionView
            Grid.Row="1"
            ItemsSource="{Binding Clients}"
            Tap="OnClientTapped">
            <dx:DXCollectionView.ItemTemplate>
                <DataTemplate>
                    <Grid Padding="12,8" ColumnDefinitions="*,Auto">
                        <StackLayout>
                            <Label Text="{Binding Name}" FontAttributes="Bold" FontSize="16" />
                            <Label Text="{Binding CompanyName}" FontSize="13" TextColor="Gray" />
                        </StackLayout>
                        <Label Grid.Column="1" Text="{Binding Phone}" FontSize="13"
                               VerticalOptions="Center" TextColor="Gray" />
                    </Grid>
                </DataTemplate>
            </dx:DXCollectionView.ItemTemplate>
        </dx:DXCollectionView>
    </Grid>
</ContentPage>
```

**Step 3: Update ClientsPage.xaml.cs**

```csharp
using XafMaui.Data;
using XafMaui.ViewModels;

namespace XafMaui.Views;

public partial class ClientsPage : ContentPage
{
    public ClientsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ClientsViewModel)BindingContext).LoadClients();
    }

    async void OnClientTapped(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        if (e.Item is LocalClient client)
            await Navigation.PushAsync(new ClientDetailPage(client.ID));
    }
}
```

**Step 4: Create ClientDetailPage** showing read-only client info, addresses, and contact persons. Use `DXCollectionView` for contact persons list, labels for address fields. Load from SQLite by client ID.

**Step 5: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 6: Commit**

```bash
git add XafMaui/Views/ClientsPage.xaml XafMaui/Views/ClientsPage.xaml.cs XafMaui/Views/ClientDetailPage.xaml XafMaui/Views/ClientDetailPage.xaml.cs XafMaui/ViewModels/ClientsViewModel.cs
git commit -m "feat: build Clients tab with search and detail view"
```

---

### Task 23: Build Day Sheet tab

**Files:**
- Modify: `XafMaui/Views/DaySheetPage.xaml` + `.xaml.cs`
- Create: `XafMaui/ViewModels/DaySheetViewModel.cs`

This is the most complex MAUI screen. Uses `DXDataGrid` for editable time entries with:
- Date picker at top
- Grid columns: Project (read-only), Task (read-only), Hours (editable), Note (editable), Status
- Add button that navigates to a new entry form
- Submit button for Draft entries

**Step 1: Create DaySheetViewModel** with:
- `SelectedDate` property (default today)
- `TimeEntries` ObservableCollection loaded from SQLite filtered by date
- `AddTimeEntry()` — saves to SQLite with `IsPendingSync = true`
- `SubmitEntries()` — sets status to Submitted, marks pending sync

**Step 2: Create DaySheetPage.xaml** with DXDataGrid

**Step 3: Create AddTimeEntryPage** with cascading project → task pickers using `ComboBoxEdit`

**Step 4: Build to verify**

Run: `dotnet build XafMaui/XafMaui.csproj -f net10.0-android`
Expected: Build succeeded

**Step 5: Commit**

```bash
git add XafMaui/Views/DaySheetPage.xaml XafMaui/Views/DaySheetPage.xaml.cs XafMaui/Views/AddTimeEntryPage.xaml XafMaui/Views/AddTimeEntryPage.xaml.cs XafMaui/ViewModels/DaySheetViewModel.cs
git commit -m "feat: build Day Sheet tab with editable time entry grid"
```

---

### Task 24: Build Projects tab

**Files:**
- Modify: `XafMaui/Views/ProjectsPage.xaml` + `.xaml.cs`
- Create: `XafMaui/Views/ProjectDetailPage.xaml` + `.xaml.cs`
- Create: `XafMaui/ViewModels/ProjectsViewModel.cs`

**Step 1: Create ProjectsViewModel** — loads projects from SQLite, grouped by status

**Step 2: Create ProjectsPage.xaml** with `DXCollectionView` grouped by status, showing name + progress %

**Step 3: Create ProjectDetailPage** — shows task list with estimated vs logged hours per task, progress bars

**Step 4: Build to verify and commit**

```bash
git add XafMaui/Views/ProjectsPage.xaml XafMaui/Views/ProjectsPage.xaml.cs XafMaui/Views/ProjectDetailPage.xaml XafMaui/Views/ProjectDetailPage.xaml.cs XafMaui/ViewModels/ProjectsViewModel.cs
git commit -m "feat: build Projects tab with grouped list and task detail"
```

---

### Task 25: Build Reports tab

**Files:**
- Modify: `XafMaui/Views/ReportsPage.xaml` + `.xaml.cs`
- Create: `XafMaui/ViewModels/ReportsViewModel.cs`

**Step 1: Create ReportsViewModel** with:
- `WeeklyHours` — aggregated hours per day for current week
- `SelectedProject` — for project-specific reports
- `TaskDistribution` — hours per task for pie chart
- `EstimatedVsActual` — per task for bar chart
- `BudgetBurnPercent` — for gauge

All data loaded from SQLite (local time entries + project/task data).

**Step 2: Create ReportsPage.xaml** with:
- "My Week" section: `DXChartView` with bar series (hours per day)
- "Project Progress" section: project picker + `DXChartView` pie + bar
- "Budget Burn" section: `DXGaugeView` showing percentage
- Date range filter using `DateEdit`

**Step 3: Build to verify and commit**

```bash
git add XafMaui/Views/ReportsPage.xaml XafMaui/Views/ReportsPage.xaml.cs XafMaui/ViewModels/ReportsViewModel.cs
git commit -m "feat: build Reports tab with charts and gauges"
```

---

## Phase 5: Integration & Polish

### Task 26: Add pull-to-refresh on all list pages

**Files:**
- Modify: `XafMaui/Views/ClientsPage.xaml`
- Modify: `XafMaui/Views/ProjectsPage.xaml`
- Modify: `XafMaui/Views/DaySheetPage.xaml`

**Step 1:** Add `IsPullToRefreshEnabled="True"` and `PullToRefresh` event to each `DXCollectionView`/`DXDataGrid`. In the handler, call `SyncService` to re-download data, then reload the view model.

**Step 2: Build to verify and commit**

```bash
git add XafMaui/Views/ClientsPage.xaml XafMaui/Views/ProjectsPage.xaml XafMaui/Views/DaySheetPage.xaml
git commit -m "feat: add pull-to-refresh sync on all list pages"
```

---

### Task 27: Add logout functionality

**Files:**
- Modify: `XafMaui/Views/AppShell.xaml` (add menu item)
- Modify: `XafMaui/Views/AppShell.xaml.cs`

**Step 1:** Add a toolbar item or menu item "Logout" to AppShell. On click: call `AuthService.Logout()`, `SyncService.ClearLocalDataAsync()`, navigate back to LoginPage.

**Step 2: Build to verify and commit**

```bash
git add XafMaui/Views/AppShell.xaml XafMaui/Views/AppShell.xaml.cs
git commit -m "feat: add logout with local data cleanup"
```

---

### Task 28: Verify end-to-end flow

**Step 1:** Start Docker SQL Server: `docker start xafmaui-sql`

**Step 2:** Run XAF app: `dotnet run --project XafMaui.Blazor.Server`

**Step 3:** Log in to XAF Blazor as Admin, create:
- 2 clients with addresses and contacts
- 2 projects with 3-4 tasks each
- Assign Consultant user to both projects

**Step 4:** Run MAUI app on Android emulator

**Step 5:** Log in as Consultant, verify:
- Clients tab shows both clients with search working
- Day Sheet allows creating time entries
- Projects tab shows assigned projects with tasks
- Reports tab shows charts with entered data

**Step 6:** Test offline: disable network, create time entry, re-enable, verify sync

---

## Summary

| Phase | Tasks | Focus |
|---|---|---|
| Phase 1 | Tasks 1-9 | XAF data model & business objects |
| Phase 2 | Tasks 10-12 | XAF Web API & security |
| Phase 3 | Tasks 13-18 | MAUI foundation & API layer |
| Phase 4 | Tasks 19-25 | MAUI UI screens |
| Phase 5 | Tasks 26-28 | Integration & polish |

Total: 28 tasks across 5 phases. Phase 1-2 should be completed first and verified with a running XAF app before starting MAUI work.
