using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using PeopleApp.Client.Services;

namespace PeopleApp.Client.Auth;

/// <summary>
/// AuthenticationStateProvider personalizado para manejar autenticación con JWT
/// Se encarga de:
/// - Leer el token del localStorage
/// - Parsear los claims del JWT
/// - Notificar cambios de autenticación a la aplicación
/// </summary>
public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore _tokenStore;
    private readonly AuthenticationState _anonymous;

    public JwtAuthenticationStateProvider(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
        // Estado anónimo por defecto
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    /// <summary>
    /// Obtiene el estado de autenticación actual
    /// Se llama automáticamente cuando el componente CascadingAuthenticationState se renderiza
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // 1) Obtener el token del localStorage
            var token = await _tokenStore.GetTokenAsync();

            // 2) Si no hay token, retornar usuario anónimo
            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            // 3) Parsear el JWT para extraer los claims
            var principal = ParseClaimsFromJwt(token);

            // 4) Retornar AuthenticationState con el usuario autenticado
            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener estado de autenticación: {ex.Message}");
            return _anonymous;
        }
    }

    /// <summary>
    /// Notifica a la aplicación que el usuario se ha autenticado
    /// </summary>
    /// <param name="token">Token JWT</param>
    public void NotifyUserAuthentication(string token)
    {
        try
        {
            // 1) Parsear los claims del JWT
            var principal = ParseClaimsFromJwt(token);

            // 2) Crear el nuevo estado de autenticación
            var authState = new AuthenticationState(principal);

            // 3) Notificar a todos los listeners del cambio de estado
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al notificar autenticación: {ex.Message}");
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }
    }

    /// <summary>
    /// Notifica a la aplicación que el usuario se ha desautenticado
    /// </summary>
    public void NotifyUserLogout()
    {
        // Notificar que el usuario es anónimo
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    /// <summary>
    /// Parsea el JWT y extrae los claims
    /// NOTA: No valida la firma del token (eso lo hace el servidor)
    /// Solo extrae los claims para mostrar en la UI
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ClaimsPrincipal con los claims extraídos</returns>
    private ClaimsPrincipal ParseClaimsFromJwt(string token)
    {
        try
        {
            // 1) Crear un tokenHandler (sin validación)
            var handler = new JwtSecurityTokenHandler();

            // 2) Leer el JWT sin validar firma (ReadToken sin validación)
            var jwtToken = handler.ReadJwtToken(token);

            // 3) Crear una ClaimsIdentity con los claims del JWT
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");

            // 4) Crear y retornar el ClaimsPrincipal
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al parsear JWT: {ex.Message}");
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
