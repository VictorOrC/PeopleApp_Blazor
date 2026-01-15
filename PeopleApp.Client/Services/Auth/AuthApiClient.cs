using System.Net.Http.Json;
using PeopleApp.Client.Dtos;

namespace PeopleApp.Client.Services.Auth;

/// <summary>
/// Servicio de alto nivel para consumir los endpoints de autenticación
/// </summary>
public class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="dto">Datos de registro</param>
    /// <returns>Respuesta con token JWT</returns>
    /// <exception cref="HttpRequestException">Si la solicitud falla</exception>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                return result ?? throw new InvalidOperationException("No se recibió token en la respuesta.");
            }

            // Si no es exitoso, leer el mensaje de error
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error en registro: {response.StatusCode} - {errorContent}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error al registrar usuario: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Autentica un usuario existente
    /// </summary>
    /// <param name="dto">Credenciales de login</param>
    /// <returns>Respuesta con token JWT</returns>
    /// <exception cref="HttpRequestException">Si la solicitud falla</exception>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                return result ?? throw new InvalidOperationException("No se recibió token en la respuesta.");
            }

            // Si no es exitoso, leer el mensaje de error
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error en login: {response.StatusCode} - {errorContent}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error al iniciar sesión: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene los datos del usuario autenticado (requiere token válido)
    /// </summary>
    /// <returns>Datos del usuario</returns>
    /// <exception cref="HttpRequestException">Si la solicitud falla o el token no es válido</exception>
    public async Task<UserMeDto> MeAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/auth/me");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserMeDto>();
                return result ?? throw new InvalidOperationException("No se recibieron datos del usuario.");
            }

            // Si no es exitoso, probablemente el token es inválido
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error al obtener datos del usuario: {response.StatusCode} - {errorContent}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error al obtener datos del usuario: {ex.Message}", ex);
        }
    }
}
