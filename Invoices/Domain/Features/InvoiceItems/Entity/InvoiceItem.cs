using System.ComponentModel.DataAnnotations.Schema;
using Domain.Features.Products.Entity;

namespace Domain;

// Mapped from the <poz> node of the supplier's VFP XML export.
public class InvoiceItem
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public int LineNumber { get; set; }       
    public string ProductCode { get; set; } = string.Empty; 
    public int WarehouseNumber { get; set; }  
    public int SubWarehouseNumber { get; set; } 
    public string? Brand { get; set; }        
    public string? IndexCode { get; set; }    
    public string? PersonalCode { get; set; } 
    public string Name { get; set; } = string.Empty; 
    public string? Description { get; set; }  

    [Column(TypeName = "decimal(5,0)")]
    public decimal Quantity { get; set; }     

    [Column(TypeName = "decimal(9,2)")]
    public decimal UnitPrice { get; set; }    

    [Column(TypeName = "decimal(2,0)")]
    public decimal VatRate { get; set; }      

    public string? Sww { get; set; }          
    public string? GroupName { get; set; }    
}