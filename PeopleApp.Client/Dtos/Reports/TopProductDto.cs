namespace PeopleApp.Client.Dtos.Reports;

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}
