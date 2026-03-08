using System.ComponentModel.DataAnnotations;

namespace XafMaui.Data;

public class LocalClient
{
    [Key] public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? VatNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? BillingStreet { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }
    public string? VisitStreet { get; set; }
    public string? VisitCity { get; set; }
    public string? VisitPostalCode { get; set; }
    public string? VisitCountry { get; set; }
}

public class LocalContactPerson
{
    [Key] public int ID { get; set; }
    public int ClientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
}

public class LocalProject
{
    [Key] public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BudgetHours { get; set; }
    public int? ClientID { get; set; }
    public string? ClientName { get; set; }
}

public class LocalProjectTask
{
    [Key] public int ID { get; set; }
    public int ProjectID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public decimal EstimatedHours { get; set; }
    public int SortOrder { get; set; }
}

public class LocalTimeEntry
{
    [Key] public int LocalId { get; set; }
    public int? ServerID { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Note { get; set; }
    public int Status { get; set; }
    public int ProjectTaskID { get; set; }
    public string? ProjectTaskName { get; set; }
    public string? ProjectName { get; set; }
    public bool IsPendingSync { get; set; }
}
