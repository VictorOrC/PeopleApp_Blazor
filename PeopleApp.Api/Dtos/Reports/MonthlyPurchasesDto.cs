namespace PeopleApp.Api.Dtos.Reports;

public class MonthlyPurchasesDto
{
    public int Year { get; set; }
    public int Month { get; set; } // 1-12
    public int PurchasesCount { get; set; }
    public decimal TotalAmount { get; set; }

    // opcional: útil para UI
    public string Label => $"{Year}-{Month:00}";
}
