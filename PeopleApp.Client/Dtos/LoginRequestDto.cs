namespace PeopleApp.Client.Dtos;

/// <summary>
/// DTO para solicitud de login
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
