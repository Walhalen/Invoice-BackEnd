namespace Domain;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? VatNumber { get; set; }
    public string? DefaultCurrencyCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? IBAN { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
