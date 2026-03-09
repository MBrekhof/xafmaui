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

            SeedDemoData(userManager);
#endif
        }

        void SeedDemoData(UserManager userManager)
        {
            // Skip if data already exists
            if (ObjectSpace.FirstOrDefault<Client>(c => c.Name == "Contoso Ltd") != null)
                return;

            // --- Consultants ---
            var consultant1 = userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Consultant")!;

            ApplicationUser? consultant2 = null;
            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Sarah") == null)
            {
                var sarahResult = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Sarah", "", user =>
                {
                    user.Roles.Add(ObjectSpace.FirstOrDefault<DevExpress.Persistent.BaseImpl.EF.PermissionPolicy.PermissionPolicyRole>(r => r.Name == "Consultants")!);
                    user.AppRole = AppRole.Consultant;
                    user.DisplayName = "Sarah Johnson";
                });
                consultant2 = sarahResult.User;
            }
            else
            {
                consultant2 = userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Sarah");
            }

            ApplicationUser? consultant3 = null;
            if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Mike") == null)
            {
                var mikeResult = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Mike", "", user =>
                {
                    user.Roles.Add(ObjectSpace.FirstOrDefault<DevExpress.Persistent.BaseImpl.EF.PermissionPolicy.PermissionPolicyRole>(r => r.Name == "Consultants")!);
                    user.AppRole = AppRole.Consultant;
                    user.DisplayName = "Mike Chen";
                });
                consultant3 = mikeResult.User;
            }
            else
            {
                consultant3 = userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Mike");
            }

            var manager = userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Manager")!;

            ObjectSpace.CommitChanges();

            // --- Addresses ---
            var addr1 = ObjectSpace.CreateObject<Address>();
            addr1.Street = "123 Innovation Drive"; addr1.City = "Seattle"; addr1.PostalCode = "98101"; addr1.Country = "USA";
            var addr2 = ObjectSpace.CreateObject<Address>();
            addr2.Street = "456 Tech Park Blvd"; addr2.City = "San Francisco"; addr2.PostalCode = "94105"; addr2.Country = "USA";
            var addr3 = ObjectSpace.CreateObject<Address>();
            addr3.Street = "Keizersgracht 123"; addr3.City = "Amsterdam"; addr3.PostalCode = "1015 AA"; addr3.Country = "Netherlands";
            var addr4 = ObjectSpace.CreateObject<Address>();
            addr4.Street = "Friedrichstraße 45"; addr4.City = "Berlin"; addr4.PostalCode = "10117"; addr4.Country = "Germany";
            var addr5 = ObjectSpace.CreateObject<Address>();
            addr5.Street = "10 Downing Street"; addr5.City = "London"; addr5.PostalCode = "SW1A 2AA"; addr5.Country = "UK";
            var addr6 = ObjectSpace.CreateObject<Address>();
            addr6.Street = "Via Roma 42"; addr6.City = "Milan"; addr6.PostalCode = "20121"; addr6.Country = "Italy";
            var addr7 = ObjectSpace.CreateObject<Address>();
            addr7.Street = "Rue de Rivoli 88"; addr7.City = "Paris"; addr7.PostalCode = "75001"; addr7.Country = "France";
            var addr8 = ObjectSpace.CreateObject<Address>();
            addr8.Street = "Paseo de la Castellana 200"; addr8.City = "Madrid"; addr8.PostalCode = "28046"; addr8.Country = "Spain";
            var addr9 = ObjectSpace.CreateObject<Address>();
            addr9.Street = "Herengracht 500"; addr9.City = "Rotterdam"; addr9.PostalCode = "3011 AA"; addr9.Country = "Netherlands";
            var addr10 = ObjectSpace.CreateObject<Address>();
            addr10.Street = "1600 Amphitheatre Pkwy"; addr10.City = "Mountain View"; addr10.PostalCode = "94043"; addr10.Country = "USA";

            // --- Clients ---
            var contoso = ObjectSpace.CreateObject<Client>();
            contoso.Name = "Contoso Ltd"; contoso.CompanyName = "Contoso Corporation"; contoso.VatNumber = "US12-3456789";
            contoso.Email = "info@contoso.com"; contoso.Phone = "+1-206-555-0100";
            contoso.BillingAddress = addr1; contoso.VisitAddress = addr2;

            var northwind = ObjectSpace.CreateObject<Client>();
            northwind.Name = "Northwind Traders"; northwind.CompanyName = "Northwind Trading BV"; northwind.VatNumber = "NL123456789B01";
            northwind.Email = "contact@northwind.nl"; northwind.Phone = "+31-20-555-0200";
            northwind.BillingAddress = addr3; northwind.VisitAddress = addr9;

            var fabrikam = ObjectSpace.CreateObject<Client>();
            fabrikam.Name = "Fabrikam GmbH"; fabrikam.CompanyName = "Fabrikam Industries"; fabrikam.VatNumber = "DE123456789";
            fabrikam.Email = "hello@fabrikam.de"; fabrikam.Phone = "+49-30-555-0300";
            fabrikam.BillingAddress = addr4;

            var adventureWorks = ObjectSpace.CreateObject<Client>();
            adventureWorks.Name = "Adventure Works"; adventureWorks.CompanyName = "Adventure Works Cycles"; adventureWorks.VatNumber = "GB123456789";
            adventureWorks.Email = "sales@adventureworks.co.uk"; adventureWorks.Phone = "+44-20-555-0400";
            adventureWorks.BillingAddress = addr5;

            var tailspin = ObjectSpace.CreateObject<Client>();
            tailspin.Name = "Tailspin Toys"; tailspin.CompanyName = "Tailspin Toys SRL"; tailspin.VatNumber = "IT12345678901";
            tailspin.Email = "info@tailspin.it"; tailspin.Phone = "+39-02-555-0500";
            tailspin.BillingAddress = addr6;

            var wideworldImporters = ObjectSpace.CreateObject<Client>();
            wideworldImporters.Name = "Wide World Importers"; wideworldImporters.CompanyName = "Wide World Importers SA"; wideworldImporters.VatNumber = "FR12345678901";
            wideworldImporters.Email = "contact@wideworldimporters.fr"; wideworldImporters.Phone = "+33-1-555-0600";
            wideworldImporters.BillingAddress = addr7;

            var alpineSkiHouse = ObjectSpace.CreateObject<Client>();
            alpineSkiHouse.Name = "Alpine Ski House"; alpineSkiHouse.CompanyName = "Alpine Ski House SL"; alpineSkiHouse.VatNumber = "ES12345678A";
            alpineSkiHouse.Email = "info@alpineskihouse.es"; alpineSkiHouse.Phone = "+34-91-555-0700";
            alpineSkiHouse.BillingAddress = addr8;

            var woodgrove = ObjectSpace.CreateObject<Client>();
            woodgrove.Name = "Woodgrove Bank"; woodgrove.CompanyName = "Woodgrove Financial Services"; woodgrove.VatNumber = "US98-7654321";
            woodgrove.Email = "corporate@woodgrove.com"; woodgrove.Phone = "+1-650-555-0800";
            woodgrove.BillingAddress = addr10;

            // --- Contact Persons ---
            AddContact("John Smith", "john@contoso.com", "+1-206-555-0101", "CTO", contoso);
            AddContact("Lisa Park", "lisa@contoso.com", "+1-206-555-0102", "Product Owner", contoso);
            AddContact("Emma Williams", "emma@contoso.com", "+1-206-555-0103", "Developer Lead", contoso);
            AddContact("Jan de Vries", "jan@northwind.nl", "+31-20-555-0201", "Managing Director", northwind);
            AddContact("Pieter Jansen", "pieter@northwind.nl", "+31-20-555-0202", "IT Manager", northwind);
            AddContact("Klaus Weber", "klaus@fabrikam.de", "+49-30-555-0301", "Head of Engineering", fabrikam);
            AddContact("Anna Müller", "anna@fabrikam.de", "+49-30-555-0302", "Project Coordinator", fabrikam);
            AddContact("James Wilson", "james@adventureworks.co.uk", "+44-20-555-0401", "CEO", adventureWorks);
            AddContact("Sophie Brown", "sophie@adventureworks.co.uk", "+44-20-555-0402", "Finance Director", adventureWorks);
            AddContact("Marco Rossi", "marco@tailspin.it", "+39-02-555-0501", "Operations Manager", tailspin);
            AddContact("Pierre Dupont", "pierre@wideworldimporters.fr", "+33-1-555-0601", "Logistics Director", wideworldImporters);
            AddContact("Carlos García", "carlos@alpineskihouse.es", "+34-91-555-0701", "Digital Manager", alpineSkiHouse);
            AddContact("Robert Taylor", "robert@woodgrove.com", "+1-650-555-0801", "VP Technology", woodgrove);
            AddContact("Maria Chen", "maria@woodgrove.com", "+1-650-555-0802", "Compliance Officer", woodgrove);

            // --- Projects ---
            // Active projects
            var p1 = CreateProject("ERP Migration", "Full ERP system migration from legacy to cloud-based solution", ProjectStatus.Active,
                new DateTime(2026, 1, 6), new DateTime(2026, 6, 30), 800, contoso);
            var p2 = CreateProject("E-Commerce Platform", "New B2B e-commerce platform with product catalog and ordering", ProjectStatus.Active,
                new DateTime(2026, 2, 1), new DateTime(2026, 5, 31), 500, northwind);
            var p3 = CreateProject("IoT Dashboard", "Real-time IoT sensor monitoring dashboard", ProjectStatus.Active,
                new DateTime(2026, 2, 15), new DateTime(2026, 4, 30), 300, fabrikam);
            var p4 = CreateProject("Mobile App v2", "Second version of the customer-facing mobile app", ProjectStatus.Active,
                new DateTime(2026, 1, 15), new DateTime(2026, 7, 15), 600, adventureWorks);
            var p5 = CreateProject("Data Warehouse", "Central data warehouse with BI reporting layer", ProjectStatus.Active,
                new DateTime(2026, 3, 1), new DateTime(2026, 8, 31), 450, woodgrove);

            // Draft projects
            var p6 = CreateProject("AI Chatbot Integration", "Customer service chatbot with natural language processing", ProjectStatus.Draft,
                null, null, 200, tailspin);
            var p7 = CreateProject("Supply Chain Optimization", "ML-based supply chain forecasting system", ProjectStatus.Draft,
                null, null, 350, wideworldImporters);

            // Completed project
            var p8 = CreateProject("Website Redesign", "Corporate website redesign with CMS", ProjectStatus.Completed,
                new DateTime(2025, 9, 1), new DateTime(2025, 12, 15), 250, alpineSkiHouse);

            // On Hold
            var p9 = CreateProject("CRM Integration", "Salesforce CRM integration with internal systems", ProjectStatus.OnHold,
                new DateTime(2026, 1, 10), null, 400, contoso);

            // --- Project Tasks ---
            // ERP Migration tasks
            var t1a = CreateTask("Requirements Analysis", "Gather and document all business requirements", ProjectTaskStatus.Done, 80, 1, p1);
            var t1b = CreateTask("Data Migration Design", "Design data migration strategy and mappings", ProjectTaskStatus.Done, 60, 2, p1);
            var t1c = CreateTask("Core Module Development", "Develop core ERP modules (Finance, HR, Inventory)", ProjectTaskStatus.InProgress, 200, 3, p1);
            var t1d = CreateTask("Integration Layer", "Build API integration layer for third-party systems", ProjectTaskStatus.InProgress, 120, 4, p1);
            var t1e = CreateTask("Data Migration Execution", "Execute data migration from legacy system", ProjectTaskStatus.Open, 100, 5, p1);
            var t1f = CreateTask("UAT & Go-Live", "User acceptance testing and production deployment", ProjectTaskStatus.Open, 80, 6, p1);

            // E-Commerce Platform tasks
            var t2a = CreateTask("UI/UX Design", "Design product catalog and checkout flow", ProjectTaskStatus.Done, 60, 1, p2);
            var t2b = CreateTask("Product Catalog API", "Build product catalog backend with search", ProjectTaskStatus.InProgress, 80, 2, p2);
            var t2c = CreateTask("Order Management", "Implement order processing and fulfillment", ProjectTaskStatus.InProgress, 100, 3, p2);
            var t2d = CreateTask("Payment Integration", "Integrate Stripe/PayPal payment gateways", ProjectTaskStatus.Open, 40, 4, p2);
            var t2e = CreateTask("Performance Testing", "Load testing and optimization", ProjectTaskStatus.Open, 30, 5, p2);

            // IoT Dashboard tasks
            var t3a = CreateTask("Sensor Data Ingestion", "Build real-time data pipeline from IoT sensors", ProjectTaskStatus.Done, 50, 1, p3);
            var t3b = CreateTask("Dashboard UI", "Build interactive monitoring dashboard", ProjectTaskStatus.InProgress, 80, 2, p3);
            var t3c = CreateTask("Alerting System", "Configure threshold-based alerting and notifications", ProjectTaskStatus.Open, 40, 3, p3);
            var t3d = CreateTask("Historical Analytics", "Implement data aggregation and trend analysis", ProjectTaskStatus.Open, 60, 4, p3);

            // Mobile App v2 tasks
            var t4a = CreateTask("App Architecture", "Design MAUI app architecture and navigation", ProjectTaskStatus.Done, 40, 1, p4);
            var t4b = CreateTask("Offline Sync Engine", "Implement bidirectional offline data sync", ProjectTaskStatus.Done, 80, 2, p4);
            var t4c = CreateTask("Push Notifications", "Integrate push notification service", ProjectTaskStatus.InProgress, 50, 3, p4);
            var t4d = CreateTask("Biometric Auth", "Add fingerprint/face recognition login", ProjectTaskStatus.InProgress, 30, 4, p4);
            var t4e = CreateTask("App Store Submission", "Prepare and submit to app stores", ProjectTaskStatus.Open, 20, 5, p4);

            // Data Warehouse tasks
            var t5a = CreateTask("Schema Design", "Design star schema for data warehouse", ProjectTaskStatus.InProgress, 60, 1, p5);
            var t5b = CreateTask("ETL Pipelines", "Build ETL pipelines for data sources", ProjectTaskStatus.Open, 120, 2, p5);
            var t5c = CreateTask("BI Reports", "Create Power BI reports and dashboards", ProjectTaskStatus.Open, 80, 3, p5);
            var t5d = CreateTask("Data Governance", "Implement data quality and governance rules", ProjectTaskStatus.Open, 40, 4, p5);

            // Completed project tasks
            var t8a = CreateTask("Design Mockups", "Create website design mockups", ProjectTaskStatus.Done, 40, 1, p8);
            var t8b = CreateTask("Frontend Development", "Build responsive frontend", ProjectTaskStatus.Done, 80, 2, p8);
            var t8c = CreateTask("CMS Setup", "Configure headless CMS", ProjectTaskStatus.Done, 50, 3, p8);
            var t8d = CreateTask("Content Migration", "Migrate existing content", ProjectTaskStatus.Done, 30, 4, p8);
            var t8e = CreateTask("SEO Optimization", "On-page SEO and sitemap", ProjectTaskStatus.Done, 20, 5, p8);

            // CRM Integration tasks (on hold)
            var t9a = CreateTask("API Assessment", "Evaluate Salesforce API capabilities", ProjectTaskStatus.Done, 30, 1, p9);
            var t9b = CreateTask("Sync Architecture", "Design bidirectional sync architecture", ProjectTaskStatus.InProgress, 50, 2, p9);
            var t9c = CreateTask("Contact Sync", "Implement contact data synchronization", ProjectTaskStatus.Open, 80, 3, p9);

            // Draft project tasks
            CreateTask("NLP Model Selection", "Evaluate and select NLP models", ProjectTaskStatus.Open, 40, 1, p6);
            CreateTask("Chat Widget UI", "Design and build chat widget", ProjectTaskStatus.Open, 60, 2, p6);
            CreateTask("Training Data Prep", "Prepare training data from support tickets", ProjectTaskStatus.Open, 50, 3, p6);

            CreateTask("Data Analysis", "Analyze historical supply chain data", ProjectTaskStatus.Open, 60, 1, p7);
            CreateTask("ML Model Development", "Develop forecasting models", ProjectTaskStatus.Open, 100, 2, p7);
            CreateTask("System Integration", "Integrate with ERP and WMS", ProjectTaskStatus.Open, 80, 3, p7);

            // --- Project Assignments ---
            CreateAssignment(p1, manager, ProjectRole.Manager);
            CreateAssignment(p1, consultant1, ProjectRole.Member);
            CreateAssignment(p1, consultant2!, ProjectRole.Member);
            CreateAssignment(p2, manager, ProjectRole.Manager);
            CreateAssignment(p2, consultant3!, ProjectRole.Member);
            CreateAssignment(p3, consultant1, ProjectRole.Member);
            CreateAssignment(p3, consultant2!, ProjectRole.Member);
            CreateAssignment(p4, manager, ProjectRole.Manager);
            CreateAssignment(p4, consultant1, ProjectRole.Member);
            CreateAssignment(p4, consultant3!, ProjectRole.Member);
            CreateAssignment(p5, consultant2!, ProjectRole.Member);
            CreateAssignment(p5, consultant3!, ProjectRole.Member);
            CreateAssignment(p8, consultant1, ProjectRole.Member);
            CreateAssignment(p9, manager, ProjectRole.Manager);
            CreateAssignment(p9, consultant2!, ProjectRole.Member);

            // --- Time Entries (last 3 weeks of realistic data) ---
            var today = DateTime.Today;
            var monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            if (today.DayOfWeek == DayOfWeek.Sunday) monday = monday.AddDays(-7);

            // This week (partial — up to today)
            for (int d = 0; d < Math.Min((int)(today - monday).TotalDays + 1, 5); d++)
            {
                var date = monday.AddDays(d);
                CreateTimeEntry(date, 3.0m, "Core module: finance ledger implementation", TimeEntryStatus.Draft, consultant1, t1c);
                CreateTimeEntry(date, 2.0m, "API endpoint design and documentation", TimeEntryStatus.Draft, consultant1, t1d);
                CreateTimeEntry(date, 4.0m, "Product search implementation with Elasticsearch", TimeEntryStatus.Draft, consultant3!, t2b);
                CreateTimeEntry(date, 2.5m, "Dashboard component: live sensor charts", TimeEntryStatus.Draft, consultant2!, t3b);
                CreateTimeEntry(date, 1.5m, "Push notification service setup", TimeEntryStatus.Draft, consultant1, t4c);
                if (d < 3) CreateTimeEntry(date, 3.5m, "Star schema iteration and review", TimeEntryStatus.Draft, consultant2!, t5a);
            }

            // Last week (submitted)
            var lastMonday = monday.AddDays(-7);
            for (int d = 0; d < 5; d++)
            {
                var date = lastMonday.AddDays(d);
                CreateTimeEntry(date, 4.0m, "Core module: inventory management", TimeEntryStatus.Submitted, consultant1, t1c);
                CreateTimeEntry(date, 2.0m, "Integration layer: REST API scaffolding", TimeEntryStatus.Submitted, consultant1, t1d);
                CreateTimeEntry(date, 1.5m, "Biometric auth: Android implementation", TimeEntryStatus.Submitted, consultant1, t4d);
                CreateTimeEntry(date, 5.0m, "Product catalog API: CRUD and filtering", TimeEntryStatus.Submitted, consultant3!, t2b);
                CreateTimeEntry(date, 3.0m, "Order management: workflow engine", TimeEntryStatus.Submitted, consultant3!, t2c);
                CreateTimeEntry(date, 4.0m, "Dashboard: real-time WebSocket integration", TimeEntryStatus.Submitted, consultant2!, t3b);
                CreateTimeEntry(date, 2.0m, "Alerting: threshold configuration UI", TimeEntryStatus.Submitted, consultant2!, t3c);
                CreateTimeEntry(date, 2.0m, "Data warehouse: source system analysis", TimeEntryStatus.Submitted, consultant2!, t5a);
            }

            // Two weeks ago (approved)
            var twoWeeksAgo = monday.AddDays(-14);
            for (int d = 0; d < 5; d++)
            {
                var date = twoWeeksAgo.AddDays(d);
                CreateTimeEntry(date, 5.0m, "Core module: HR sub-system", TimeEntryStatus.Approved, consultant1, t1c);
                CreateTimeEntry(date, 3.0m, "Integration layer: authentication middleware", TimeEntryStatus.Approved, consultant1, t1d);
                CreateTimeEntry(date, 6.0m, "UI/UX: checkout flow finalization", TimeEntryStatus.Approved, consultant3!, t2a);
                CreateTimeEntry(date, 2.0m, "Order management: initial setup", TimeEntryStatus.Approved, consultant3!, t2c);
                CreateTimeEntry(date, 5.0m, "Sensor data pipeline: Kafka integration", TimeEntryStatus.Approved, consultant2!, t3a);
                CreateTimeEntry(date, 3.0m, "Dashboard: initial wireframe implementation", TimeEntryStatus.Approved, consultant2!, t3b);
            }

            // Three weeks ago (approved) — for completed project too
            var threeWeeksAgo = monday.AddDays(-21);
            for (int d = 0; d < 5; d++)
            {
                var date = threeWeeksAgo.AddDays(d);
                CreateTimeEntry(date, 4.0m, "Requirements workshops with stakeholders", TimeEntryStatus.Approved, consultant1, t1a);
                CreateTimeEntry(date, 3.0m, "Data migration: source analysis", TimeEntryStatus.Approved, consultant1, t1b);
                CreateTimeEntry(date, 6.0m, "UI/UX: product catalog design", TimeEntryStatus.Approved, consultant3!, t2a);
                CreateTimeEntry(date, 2.0m, "Sensor pipeline: initial prototype", TimeEntryStatus.Approved, consultant2!, t3a);
                CreateTimeEntry(date, 4.0m, "CRM: Salesforce API evaluation", TimeEntryStatus.Approved, consultant2!, t9a);
            }

            ObjectSpace.CommitChanges();
        }

        void AddContact(string name, string email, string phone, string jobTitle, Client client)
        {
            var cp = ObjectSpace.CreateObject<ContactPerson>();
            cp.Name = name; cp.Email = email; cp.Phone = phone; cp.JobTitle = jobTitle; cp.Client = client;
        }

        Project CreateProject(string name, string description, ProjectStatus status, DateTime? start, DateTime? end, decimal budgetHours, Client client)
        {
            var p = ObjectSpace.CreateObject<Project>();
            p.Name = name; p.Description = description; p.Status = status;
            p.StartDate = start; p.EndDate = end; p.BudgetHours = budgetHours; p.Client = client;
            return p;
        }

        ProjectTask CreateTask(string name, string description, ProjectTaskStatus status, decimal estimatedHours, int sortOrder, Project project)
        {
            var t = ObjectSpace.CreateObject<ProjectTask>();
            t.Name = name; t.Description = description; t.Status = status;
            t.EstimatedHours = estimatedHours; t.SortOrder = sortOrder; t.Project = project;
            return t;
        }

        void CreateAssignment(Project project, ApplicationUser user, ProjectRole role)
        {
            var a = ObjectSpace.CreateObject<ProjectAssignment>();
            a.Project = project; a.User = user; a.Role = role;
        }

        void CreateTimeEntry(DateTime date, decimal hours, string note, TimeEntryStatus status, ApplicationUser user, ProjectTask task)
        {
            var e = ObjectSpace.CreateObject<TimeEntry>();
            e.Date = date; e.Hours = hours; e.Note = note; e.Status = status;
            e.User = user; e.ProjectTask = task;
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
