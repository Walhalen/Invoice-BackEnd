using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Invoice
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime IssuedAt { get; set; }
}
