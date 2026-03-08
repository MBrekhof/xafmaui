namespace XafMaui.Models;

public class ClientDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? VatNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public AddressDto? BillingAddress { get; set; }
    public AddressDto? VisitAddress { get; set; }
    public List<ContactPersonDto> ContactPersons { get; set; } = [];
}

public class AddressDto
{
    public int ID { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class ContactPersonDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
}
