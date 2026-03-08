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
