using DevExpress.Persistent.Base;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
public class ProjectAssignment : BaseObjectInt
{
    public virtual Project? Project { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ProjectRole Role { get; set; }
}
