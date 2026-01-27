namespace PeopleApp.Client.Dtos.Reports;

public class MonthlyPurchasesDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int PurchasesCount { get; set; }
    public decimal TotalAmount { get; set; }

    public string Label => $"{Year}-{Month:00}";
}
