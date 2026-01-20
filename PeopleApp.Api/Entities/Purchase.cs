using System.ComponentModel.DataAnnotations;

namespace PeopleApp.Api.Entities;

public class Purchase
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    // Si ahorita “Cliente” es texto, lo dejamos así para avanzar.
    // Después lo normalizamos a CustomerId.
    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    // Opcional pero recomendado:
    public decimal Total { get; set; }

    public List<PurchaseLine> Lines { get; set; } = new();
}
