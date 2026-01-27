namespace PeopleApp.Client.Dtos.Reports;

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public int PurchasesCount { get; set; }
    public decimal TotalAmount { get; set; }

    public string Label => Date.ToString("MM/dd/yyyy");
}
