using System.ComponentModel.DataAnnotations;

namespace PeopleApp.Api.Dtos.Auth;

public class RegisterRequestDto
{
    [Required]
    [MinLength(2)]
    [MaxLength(64)]
    public string FirstName { get; set; } = "";
    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    public string LastName { get; set; } = "";
    [Required]
    [MinLength(10)]
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = "";
}
