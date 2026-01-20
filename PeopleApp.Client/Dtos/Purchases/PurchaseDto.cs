namespace PeopleApp.Client.Dtos.Purchases;

public class PurchaseDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Total { get; set; }
    public List<PurchaseLineDto> Lines { get; set; } = new();
}

public class PurchaseLineDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public decimal LineTotal { get; set; }
}
