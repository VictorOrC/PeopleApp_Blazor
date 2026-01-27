namespace PeopleApp.Api.Dtos.Reports;

public class DailySalesDto
{
    public DateTime Date { get; set; }      // Usamos DateTime pero siempre a medianoche
    public int PurchasesCount { get; set; }
    public decimal TotalAmount { get; set; }

    // útil para chart (opcional)
    public string Label => Date.ToString("MM/dd/yyyy");
}
