using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace PeopleApp.Api.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    public string FirstName { get; set; } = "";
    [Required]
    [MinLength(3)]
    [MaxLength(64)]
    public string LastName { get; set; } = "";
}