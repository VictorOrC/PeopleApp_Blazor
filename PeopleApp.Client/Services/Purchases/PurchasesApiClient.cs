using System.Net.Http.Json;
using PeopleApp.Client.Dtos.Purchases;

namespace PeopleApp.Client.Services.Purchases;

public class PurchasesApiClient
{
    private readonly HttpClient _http;
    public PurchasesApiClient(HttpClient http) => _http = http;

    public async Task<List<PurchaseListItemDto>> GetAllAsync()
        => await _http.GetFromJsonAsync<List<PurchaseListItemDto>>("api/purchases") ?? new();

    public async Task<PurchaseDto?> GetByIdAsync(int id)
        => await _http.GetFromJsonAsync<PurchaseDto>($"api/purchases/{id}");

    public async Task<PurchaseDto> CreateAsync(PurchaseCreateDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/purchases", dto);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<PurchaseDto>())!;
    }
    public async Task<byte[]> GetPdfAsync(int purchaseId)
    {
        var res = await _http.GetAsync($"api/purchases/{purchaseId}/export-pdf");
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsByteArrayAsync();
    }

}
