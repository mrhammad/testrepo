namespace OrderWiseErp.App.Models;

public sealed class Contact
{
    public int Id { get; set; }

    public string ContactType { get; set; } = "Customer";

    public string ContactCategory { get; set; } = "Business";

    public string BusinessName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Email { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string AccountNo { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal OpeningBalance { get; set; }

    public string City { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string VatNumber { get; set; } = string.Empty;

    public int PaymentTermDays { get; set; }

    public decimal CreditLimit { get; set; }

    public string Notes { get; set; } = string.Empty;
}
