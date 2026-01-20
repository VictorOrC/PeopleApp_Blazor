namespace PeopleApp.Api.Dtos.Purchases;

public class PurchaseListItemDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Total { get; set; }
}
