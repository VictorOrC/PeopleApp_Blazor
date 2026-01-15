using PeopleApp.Client.Services;

namespace PeopleApp.Client.Services.Http;

/// <summary>
/// DelegatingHandler que automáticamente agrega el token JWT a cada request HTTP
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AuthHeaderHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    /// <summary>
    /// Intercepta cada request HTTP y agrega el header Authorization si existe un token
    /// </summary>
    /// <param name="request">El mensaje HTTP a enviar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>La respuesta del servidor</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1) Obtener el token del Local Storage
        var token = await _tokenStore.GetTokenAsync();

        // 2) Si existe un token y el header Authorization no está ya configurado
        if (!string.IsNullOrWhiteSpace(token))
        {
            // Verificar que no haya ya un header Authorization
            if (request.Headers.Authorization == null)
            {
                // 3) Agregar el header "Authorization: Bearer {token}"
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 4) Enviar el request con el header agregado
        return await base.SendAsync(request, cancellationToken);
    }
}
