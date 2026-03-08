using XafMaui.Data;

namespace XafMaui.Views;

[QueryProperty(nameof(ClientId), "id")]
public partial class ClientDetailPage : ContentPage
{
    public int ClientId { get; set; }

    public ClientDetailPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadClient();
    }

    void LoadClient()
    {
        using var db = new LocalDbContext();
        var client = db.Clients.FirstOrDefault(c => c.ID == ClientId);
        if (client == null) return;

        nameLabel.Text = client.Name;
        companyLabel.Text = client.CompanyName;
        vatLabel.Text = !string.IsNullOrEmpty(client.VatNumber) ? $"VAT: {client.VatNumber}" : "";
        emailLabel.Text = client.Email;
        phoneLabel.Text = client.Phone;

        billingAddressLabel.Text = FormatAddress(client.BillingStreet, client.BillingCity, client.BillingPostalCode, client.BillingCountry);
        visitAddressLabel.Text = FormatAddress(client.VisitStreet, client.VisitCity, client.VisitPostalCode, client.VisitCountry);

        var contacts = db.ContactPersons.Where(cp => cp.ClientID == ClientId).ToList();
        contactsView.ItemsSource = contacts;
    }

    static string FormatAddress(string? street, string? city, string? postal, string? country)
    {
        var parts = new[] { street, $"{postal} {city}".Trim(), country }
            .Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join("\n", parts);
    }
}
