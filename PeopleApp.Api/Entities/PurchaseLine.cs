using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleApp.Api.Entities;

public class PurchaseLine
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    // Congelar precio al momento de compra (clave para producción)
    public decimal UnitPrice { get; set; }

    // Puedes calcularlo o guardarlo. Yo lo dejo calculado (no mapeado).
    [NotMapped]
    public decimal LineTotal => UnitPrice * Quantity;
}
