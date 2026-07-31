using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

// Mapped from the <poz> node of the supplier's VFP XML export.
public class InvoiceItem
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int LineNumber { get; set; }       // lp
    public string ProductCode { get; set; } = string.Empty; // tow_kod
    public int WarehouseNumber { get; set; }  // magazyn
    public int SubWarehouseNumber { get; set; } // podmaga
    public string? Brand { get; set; }        // marka
    public string? IndexCode { get; set; }    // indeks
    public string? PersonalCode { get; set; } // pesel
    public string Name { get; set; } = string.Empty; // nazwa
    public string? Description { get; set; }  // opis

    [Column(TypeName = "decimal(5,0)")]
    public decimal Quantity { get; set; }     // ilosc

    [Column(TypeName = "decimal(9,2)")]
    public decimal UnitPrice { get; set; }    // cena

    [Column(TypeName = "decimal(2,0)")]
    public decimal VatRate { get; set; }      // vat

    public string? Sww { get; set; }          // sww
    public string? GroupName { get; set; }    // grunaz
}