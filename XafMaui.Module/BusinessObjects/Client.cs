using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(Name))]
public class Client : BaseObjectInt
{
    [Required]
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? CompanyName { get; set; }
    public virtual string? VatNumber { get; set; }
    public virtual string? Email { get; set; }
    public virtual string? Phone { get; set; }

    public virtual Address? BillingAddress { get; set; }
    public virtual Address? VisitAddress { get; set; }

    [Aggregated]
    public virtual IList<ContactPerson> ContactPersons { get; set; } = new ObservableCollection<ContactPerson>();
}
