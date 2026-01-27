using System.Net.Http.Json;
using PeopleApp.Client.Dtos.Reports;

namespace PeopleApp.Client.Services.ApiClients;

public class ReportsApiClient
{
    private readonly HttpClient _http;

    public ReportsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MonthlyPurchasesDto>> GetMonthlyPurchasesAsync(int months = 12)
        => await _http.GetFromJsonAsync<List<MonthlyPurchasesDto>>($"api/reports/purchases/monthly?months={months}")
           ?? new();

    public async Task<List<DailySalesDto>> GetDailySalesAsync(DateTime from, DateTime to)
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");

        return await _http.GetFromJsonAsync<List<DailySalesDto>>(
            $"api/reports/purchases/daily?from={fromStr}&to={toStr}"
        ) ?? new();
    }

    public async Task<SalesKpisDto> GetKpisAsync(DateTime from, DateTime to)
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");

        return await _http.GetFromJsonAsync<SalesKpisDto>(
            $"api/reports/purchases/kpis?from={fromStr}&to={toStr}"
        ) ?? new SalesKpisDto();
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10)
    {
        var fromStr = from.ToString("yyyy-MM-dd");
        var toStr = to.ToString("yyyy-MM-dd");

        return await _http.GetFromJsonAsync<List<TopProductDto>>(
            $"api/reports/products/top?from={fromStr}&to={toStr}&top={top}"
        ) ?? new();
    }
}
