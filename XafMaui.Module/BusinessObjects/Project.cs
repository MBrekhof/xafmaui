using DevExpress.ExpressApp.DC;
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

    [Aggregated]
    public virtual IList<ProjectTask> ProjectTasks { get; set; } = new ObservableCollection<ProjectTask>();
    public virtual IList<ProjectAssignment> ProjectAssignments { get; set; } = new ObservableCollection<ProjectAssignment>();
}
