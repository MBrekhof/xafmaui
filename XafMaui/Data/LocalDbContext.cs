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
