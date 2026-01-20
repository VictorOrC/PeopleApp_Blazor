using System.ComponentModel.DataAnnotations;

namespace PeopleApp.Api.Entities;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Sku { get; set; }

    // Recomendado aunque ahorita no lo uses: te servirá para totales y PDF
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
}
