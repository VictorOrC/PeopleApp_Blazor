namespace PeopleApp.Client.Dtos;

/// <summary>
/// DTO para respuesta de autenticación (contiene el token JWT)
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}
