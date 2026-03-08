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
