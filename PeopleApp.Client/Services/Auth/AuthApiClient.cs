using System.Net.Http.Json;
using PeopleApp.Client.Dtos;
using PeopleApp.Client.Services.Http;

namespace PeopleApp.Client.Services.Auth;

public class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return result ?? throw new InvalidOperationException("No se recibió token en la respuesta.");
        }

        var msg = await ApiErrorParser.ToUserMessageAsync(response);
        throw new HttpRequestException(msg);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return result ?? throw new InvalidOperationException("No se recibió token en la respuesta.");
        }

        var msg = await ApiErrorParser.ToUserMessageAsync(response);
        throw new HttpRequestException(msg);
    }

    public async Task<UserMeDto> MeAsync()
    {
        var response = await _httpClient.GetAsync("api/auth/me");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserMeDto>();
            return result ?? throw new InvalidOperationException("No se recibieron datos del usuario.");
        }

        var msg = await ApiErrorParser.ToUserMessageAsync(response);
        throw new HttpRequestException(msg);
    }
}
