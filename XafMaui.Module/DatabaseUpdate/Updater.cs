using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
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
                    user.DisplayName = "Administrator";
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

                role.AddTypePermissionsRecursively<Project>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Client>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.Read, SecurityPermissionState.Allow);

                AddDefaultUserPermissions(role);
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

                role.AddTypePermissionsRecursively<Client>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Project>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);

                AddDefaultUserPermissions(role);
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

                role.AddTypePermissionsRecursively<Client>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ContactPerson>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Address>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Project>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectTask>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<ProjectAssignment>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<TimeEntry>(SecurityOperations.Read, SecurityPermissionState.Allow);

                AddDefaultUserPermissions(role);
            }
            return role;
        }

        void AddDefaultUserPermissions(PermissionPolicyRole role)
        {
            role.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
            role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            role.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            role.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
    }
}
