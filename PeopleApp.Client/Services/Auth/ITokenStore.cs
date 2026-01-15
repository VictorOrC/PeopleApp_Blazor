namespace PeopleApp.Client.Services;

/// <summary>
/// Interfaz para manejar el almacenamiento del token JWT en el cliente
/// </summary>
public interface ITokenStore
{
    /// <summary>
    /// Guarda el token en el Local Storage
    /// </summary>
    /// <param name="token">Token JWT a guardar</param>
    Task SetTokenAsync(string token);

    /// <summary>
    /// Obtiene el token del Local Storage
    /// </summary>
    /// <returns>Token JWT o null si no existe</returns>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Elimina el token del Local Storage
    /// </summary>
    Task ClearAsync();
}