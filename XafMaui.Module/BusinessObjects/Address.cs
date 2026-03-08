using DevExpress.Persistent.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace XafMaui.Module.BusinessObjects;

[DefaultClassOptions]
[DefaultProperty(nameof(City))]
public class Address : BaseObjectInt
{
    public virtual string? Street { get; set; }
    public virtual string? City { get; set; }
    public virtual string? PostalCode { get; set; }
    public virtual string? Country { get; set; }

    [NotMapped]
    public string FullAddress => string.Join(", ",
        new[] { Street, PostalCode, City, Country }
        .Where(s => !string.IsNullOrWhiteSpace(s)));
}
