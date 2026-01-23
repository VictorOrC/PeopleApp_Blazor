using System.Net.Http.Json;
using PeopleApp.Client.Dtos.Purchases;

namespace PeopleApp.Client.Services.Purchases;

public class PurchasesApiClient
{
    private readonly HttpClient _http;
    public PurchasesApiClient(HttpClient http) => _http = http;

    public async Task<List<PurchaseListItemDto>> GetAllAsync()
    => await _http.GetFromJsonAsync<List<PurchaseListItemDto>>("api/purchases") ?? new();

    public async Task<List<PurchaseListItemDto>> GetAllAsync(
        string? search,
        DateTime? from,
        DateTime? to,
        decimal? minTotal,
        decimal? maxTotal)
    {
        var qs = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
            qs.Add($"search={Uri.EscapeDataString(search.Trim())}");

        if (from is not null)
            qs.Add($"from={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd"))}");

        if (to is not null)
            qs.Add($"to={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd"))}");

        if (minTotal is not null)
            qs.Add($"minTotal={minTotal.Value}");

        if (maxTotal is not null)
            qs.Add($"maxTotal={maxTotal.Value}");

        var url = "api/purchases" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        return await _http.GetFromJsonAsync<List<PurchaseListItemDto>>(url) ?? new();
    }



    public async Task<PurchaseDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<PurchaseDto>($"api/purchases/{id}");

    public async Task<PurchaseDto> CreateAsync(PurchaseCreateDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/purchases", dto);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<PurchaseDto>())!;
    }
    public async Task<byte[]> GetPdfAsync(int id)
    {
        var res = await _http.GetAsync($"api/purchases/{id}/export-pdf");
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsByteArrayAsync();
    }

}
