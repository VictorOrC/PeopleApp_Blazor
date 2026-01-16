using PeopleApp.Client.Auth;
using PeopleApp.Client.Dtos;

namespace PeopleApp.Client.Services.Auth;

/// <summary>
/// Servicio de orquestación para autenticación
/// Combina AuthApiClient (HTTP) + TokenStore (persistencia) + JwtAuthenticationStateProvider (estado)
/// </summary>
public class AuthService
{
    private readonly AuthApiClient _authApiClient;
    private readonly ITokenStore _tokenStore;
    private readonly JwtAuthenticationStateProvider _authStateProvider;

    public AuthService(AuthApiClient authApiClient, ITokenStore tokenStore, JwtAuthenticationStateProvider authStateProvider)
    {
        _authApiClient = authApiClient;
        _tokenStore = tokenStore;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Registra un nuevo usuario y guarda el token automáticamente
    /// Notifica al AuthenticationStateProvider que el usuario está autenticado
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

            // 3) Notificar al AuthenticationStateProvider que el usuario está autenticado
            _authStateProvider.NotifyUserAuthentication(authResponse.Token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en el registro: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Autentica un usuario y guarda el token automáticamente
    /// Notifica al AuthenticationStateProvider que el usuario está autenticado
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

            // 3) Notificar al AuthenticationStateProvider que el usuario está autenticado
            _authStateProvider.NotifyUserAuthentication(authResponse.Token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error en el login: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Cierra sesión eliminando el token
    /// Notifica al AuthenticationStateProvider que el usuario se desautenticó
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            // 1) Eliminar el token del localStorage
            await _tokenStore.ClearAsync();

            // 2) Notificar al AuthenticationStateProvider que el usuario se desautenticó
            _authStateProvider.NotifyUserLogout();
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
