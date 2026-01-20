using System.Net.Http.Json;
using PeopleApp.Client.Dtos.Products;

namespace PeopleApp.Client.Services.Products;

public class ProductsApiClient
{
    private readonly HttpClient _http;

    public ProductsApiClient(HttpClient http) => _http = http;

    public async Task<List<ProductSearchDto>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
            return new List<ProductSearchDto>();

        var url = $"api/products/search?term={Uri.EscapeDataString(term.Trim())}";
        return await _http.GetFromJsonAsync<List<ProductSearchDto>>(url) ?? new();
    }
}
