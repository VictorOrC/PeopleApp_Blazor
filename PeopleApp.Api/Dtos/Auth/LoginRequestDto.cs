using System.ComponentModel.DataAnnotations;

namespace PeopleApp.Api.Dtos.Auth;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = "";
    [Required]
    [MinLength(8)]
    [MaxLength(64)]
    public string Password { get; set; } = "";
}