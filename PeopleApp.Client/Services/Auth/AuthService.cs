using PeopleApp.Client.Dtos;

namespace PeopleApp.Client.Services.Auth;

/// <summary>
/// Servicio de orquestación para autenticación
/// Combina AuthApiClient (HTTP) con TokenStore (persistencia)
/// </summary>
public class AuthService
{
    private readonly AuthApiClient _authApiClient;
    private readonly ITokenStore _tokenStore;

    public AuthService(AuthApiClient authApiClient, ITokenStore tokenStore)
    {
        _authApiClient = authApiClient;
        _tokenStore = tokenStore;
    }

    /// <summary>
    /// Registra un nuevo usuario y guarda el token automáticamente
    /// </summary>
    /// <param name="dto">Datos de registro</param>
    public async Task RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            // 1) Llamar al API de registro
            var authResponse = await _authApiClient.RegisterAsync(dto);

            // 2) Guardar el token en localStorage
            await _tokenStore.SetTokenAsync(authResponse.Token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en el registro: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Autentica un usuario y guarda el token automáticamente
    /// </summary>
    /// <param name="dto">Credenciales de login</param>
    public async Task LoginAsync(LoginRequestDto dto)
    {
        try
        {
            // 1) Llamar al API de login
            var authResponse = await _authApiClient.LoginAsync(dto);

            // 2) Guardar el token en localStorage
            await _tokenStore.SetTokenAsync(authResponse.Token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en el login: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cierra sesión eliminando el token
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            // Eliminar el token del localStorage
            await _tokenStore.ClearAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al cerrar sesión: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene el token actual del localStorage
    /// </summary>
    /// <returns>Token o null si no existe</returns>
    public async Task<string?> GetTokenAsync()
    {
        return await _tokenStore.GetTokenAsync();
    }

    /// <summary>
    /// Verifica si el usuario está autenticado (token existe)
    /// </summary>
    /// <returns>true si existe token, false en caso contrario</returns>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    /// <summary>
    /// Obtiene los datos del usuario autenticado
    /// </summary>
    /// <returns>Datos del usuario</returns>
    public async Task<UserMeDto> GetUserAsync()
    {
        return await _authApiClient.MeAsync();
    }
}
