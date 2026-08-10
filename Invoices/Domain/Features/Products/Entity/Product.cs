using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Domain.Features.Products.Entity;

public class Product
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public string Code { get; set; }  = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IndexCode{ get; set; }
    public string? Brand { get; set; }
    public string? GroupName { get; set; }
    [Column(TypeName = "decimal(9,2)")]
    public decimal LastPurchasePrice { get; set; }
 
    [Column(TypeName = "decimal(4,2)")]
    public decimal LastVatRate { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal QuantityOnHand { get; set; }
 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
 
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}