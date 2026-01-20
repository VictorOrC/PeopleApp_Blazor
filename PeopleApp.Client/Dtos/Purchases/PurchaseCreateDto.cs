using System.ComponentModel.DataAnnotations;

namespace PeopleApp.Client.Dtos.Purchases;

public class PurchaseCreateDto
{
    [Required, MaxLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;

    [Required, MinLength(1)]
    public List<PurchaseLineCreateDto> Lines { get; set; } = new();
}

public class PurchaseLineCreateDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 100000)]
    public int Quantity { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }
}