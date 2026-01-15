using Blazored.LocalStorage;

namespace PeopleApp.Client.Services;

/// <summary>
/// Servicio para gestionar el token JWT en el Local Storage del navegador
/// </summary>
public class TokenStore : ITokenStore
{
    private readonly ILocalStorageService _localStorageService;
    private const string TokenKey = "authToken"; // Clave para guardar el token

    public TokenStore(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    /// <summary>
    /// Guarda el token en el Local Storage del navegador
    /// </summary>
    /// <param name="token">Token JWT a guardar</param>
    public async Task SetTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("El token no puede estar vacío", nameof(token));

        // Guardar el token en el Local Storage con la clave "authToken"
        await _localStorageService.SetItemAsync(TokenKey, token);
    }

    /// <summary>
    /// Obtiene el token del Local Storage
    /// </summary>
    /// <returns>Token JWT o null si no existe</returns>
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            // Intentar obtener el token del Local Storage
            var token = await _localStorageService.GetItemAsync<string>(TokenKey);

            // Retornar el token o null si no existe
            return string.IsNullOrWhiteSpace(token) ? null : token;
        }
        catch
        {
            // Si hay error al acceder al Local Storage, retornar null
            return null;
        }
    }

    /// <summary>
    /// Elimina el token del Local Storage
    /// </summary>
    public async Task ClearAsync()
    {
        try
        {
            // Eliminar el token del Local Storage
            await _localStorageService.RemoveItemAsync(TokenKey);
        }
        catch
        {
            // Si hay error, simplemente ignorar
        }
    }
}