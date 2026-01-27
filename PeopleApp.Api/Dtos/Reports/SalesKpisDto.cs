namespace PeopleApp.Api.Dtos.Reports;

public class SalesKpisDto
{
    public int PurchasesCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AvgTicket { get; set; }
    public int TotalItems { get; set; } // suma de ca
}