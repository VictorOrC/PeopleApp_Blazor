namespace PeopleApp.Api.Dtos.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public DateTimeOffset ExpiresAt { get; set; }
}