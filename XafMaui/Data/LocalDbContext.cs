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
        MigrateSchema();
    }

    void MigrateSchema()
    {
        // EnsureCreated won't add new columns to existing tables — patch manually
        try
        {
            Database.ExecuteSqlRaw("ALTER TABLE TimeEntries ADD COLUMN ReviewNote TEXT");
        }
        catch { /* column already exists */ }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "xafmaui.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
