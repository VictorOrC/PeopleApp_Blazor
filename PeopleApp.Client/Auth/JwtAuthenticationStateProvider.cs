using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using PeopleApp.Client.Services;

namespace PeopleApp.Client.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore _tokenStore;
    private readonly AuthenticationState _anonymous;

    private System.Threading.Timer? _expiryTimer;

    public JwtAuthenticationStateProvider(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _tokenStore.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            if (IsTokenExpired(token))
            {
                await _tokenStore.ClearAsync();
                CancelAutoLogout();
                return _anonymous;
            }

            // ✅ Si el token es válido, programa auto-logout (caso: refresh/reload de la app)
            ScheduleAutoLogout(token);

            var principal = ParseClaimsFromJwt(token);
            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener estado de autenticación: {ex.Message}");
            return _anonymous;
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        try
        {
            // Si llega un token expirado, mejor limpiar todo
            if (IsTokenExpired(token))
            {
                _ = ForceLogoutAsync();
                return;
            }

            // ✅ Programa auto-logout exactamente cuando expire
            ScheduleAutoLogout(token);

            var principal = ParseClaimsFromJwt(token);
            var authState = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al notificar autenticación: {ex.Message}");
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }
    }

    public void NotifyUserLogout()
    {
        CancelAutoLogout();
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private ClaimsPrincipal ParseClaimsFromJwt(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");

            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al parsear JWT: {ex.Message}");
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    // =========================
    // Expiración + Auto logout
    // =========================

    private DateTimeOffset? GetTokenExpiryUtc(string token)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expValue = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (string.IsNullOrWhiteSpace(expValue)) return null;
            if (!long.TryParse(expValue, out var expSeconds)) return null;

            return DateTimeOffset.FromUnixTimeSeconds(expSeconds);
        }
        catch
        {
            return null;
        }
    }

    private bool IsTokenExpired(string token, TimeSpan? clockSkew = null)
    {
        var exp = GetTokenExpiryUtc(token);
        if (exp is null) return true;

        // pequeña tolerancia por reloj del cliente
        var skew = clockSkew ?? TimeSpan.FromSeconds(10);
        return exp.Value <= DateTimeOffset.UtcNow.Add(skew);
    }

    private void ScheduleAutoLogout(string token)
    {
        CancelAutoLogout();

        var exp = GetTokenExpiryUtc(token);
        if (exp is null) return;

        var due = exp.Value - DateTimeOffset.UtcNow;

        // si ya expiró (o expira ya), logout inmediato
        if (due <= TimeSpan.FromSeconds(1))
        {
            _ = ForceLogoutAsync();
            return;
        }

        _expiryTimer = new System.Threading.Timer(_ =>
        {
            _ = ForceLogoutAsync();
        }, null, due, Timeout.InfiniteTimeSpan);
    }

    private void CancelAutoLogout()
    {
        try
        {
            _expiryTimer?.Dispose();
        }
        catch { /* ignore */ }
        finally
        {
            _expiryTimer = null;
        }
    }

    private async Task ForceLogoutAsync()
    {
        try
        {
            await _tokenStore.ClearAsync();
        }
        catch { /* ignore */ }

        NotifyUserLogout();
    }
}
