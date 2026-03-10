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
            // XAF OData requires @odata.bind for navigation properties
            var payload = new Dictionary<string, object?>
            {
                ["Date"] = entry.Date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ["Hours"] = entry.Hours,
                ["Note"] = entry.Note,
                ["Status"] = ((TimeEntryStatus)entry.Status).ToString(),
                ["ReviewNote"] = entry.ReviewNote,
                ["ProjectTask@odata.bind"] = $"ProjectTask({entry.ProjectTaskID})",
            };

            if (entry.ServerID == null)
            {
                var created = await _api.PostAsync<TimeEntryDto>("TimeEntry", payload);
                if (created != null)
                    entry.ServerID = created.ID;
            }
            else
            {
                payload["ID"] = entry.ServerID.Value;
                await _api.PutAsync("TimeEntry", entry.ServerID.Value, payload);
            }
            entry.IsPendingSync = false;
        }
        await db.SaveChangesAsync();
    }

    public async Task PullTimeEntriesAsync()
    {
        var entries = await _api.GetListAsync<TimeEntryDto>("TimeEntry",
            "$orderby=Date desc&$top=100&$expand=ProjectTask($select=Name;$expand=Project($select=Name))");
        using var db = new LocalDbContext();

        var nonPending = db.TimeEntries.Where(t => !t.IsPendingSync).ToList();
        db.TimeEntries.RemoveRange(nonPending);

        foreach (var e in entries)
        {
            if (!db.TimeEntries.Any(t => t.ServerID == e.ID))
            {
                db.TimeEntries.Add(new LocalTimeEntry
                {
                    ServerID = e.ID, Date = e.Date, Hours = e.Hours, Note = e.Note,
                    Status = (int)e.Status, ReviewNote = e.ReviewNote, ProjectTaskID = e.ProjectTaskID,
                    ProjectTaskName = e.ProjectTask?.Name,
                    ProjectName = e.ProjectTask?.Project?.Name,
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
