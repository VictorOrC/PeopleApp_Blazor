using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleApp.Api.Data;
using PeopleApp.Api.Dtos.Auth;
using PeopleApp.Api.Models;
using PeopleApp.Api.Services;


namespace PeopleApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict("El email ya está registrado.");

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return BadRequest(createResult.Errors.Select(e => e.Description));

        var roleResult = await _userManager.AddToRoleAsync(user, RolesNames.User);
        if (!roleResult.Succeeded)
            return StatusCode(500, roleResult.Errors.Select(e => e.Description));

        var roles = await _userManager.GetRolesAsync(user);
        var auth = _tokenService.CreateToken(user, roles);

        return Ok(auth);
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Unauthorized("Credenciales inválidas.");

        var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!ok)
            return Unauthorized("Credenciales inválidas.");

        var roles = await _userManager.GetRolesAsync(user);
        var auth = _tokenService.CreateToken(user, roles);

        return Ok(auth);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok("Token válido 👍");
    }


}
