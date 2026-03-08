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
