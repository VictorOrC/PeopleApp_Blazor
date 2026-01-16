using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PeopleApp.Api.Data;
using PeopleApp.Api.Dtos.Auth;
using PeopleApp.Api.Models;
using PeopleApp.Api.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace PeopleApp.Api.Controllers;

/// <summary>
/// Controlador de autenticación que maneja el registro, login y validación de tokens JWT
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Servicios inyectados para gestionar usuarios e identidad
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    /// <summary>
    /// Constructor que inyecta las dependencias necesarias
    /// </summary>
    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Endpoint para registrar un nuevo usuario
    /// POST: /api/auth/register
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        // 1) Verificar si el email ya existe en la base de datos
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return Conflict("El email ya está registrado.");

        // 2) Crear una nueva instancia de usuario con los datos del DTO
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email,  // El username será el email
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        };

        // 3) Crear el usuario en la base de datos con la contraseña hasheada
        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return BadRequest(createResult.Errors.Select(e => e.Description));

        // 4) Asignar el rol "User" al nuevo usuario
        var roleResult = await _userManager.AddToRoleAsync(user, RolesNames.User);
        if (!roleResult.Succeeded)
            return StatusCode(500, roleResult.Errors.Select(e => e.Description));

        // 5) Obtener los roles del usuario para incluirlos en el token JWT
        var roles = await _userManager.GetRolesAsync(user);

        // 6) Generar el token JWT con los datos del usuario y sus roles
        var auth = _tokenService.CreateToken(user, roles);

        // 7) Retornar el token al cliente
        return Ok(auth);
    }

    /// <summary>
    /// Endpoint para autenticar un usuario existente
    /// POST: /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        // 1) Buscar el usuario por email en la base de datos
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Unauthorized("Credenciales inválidas.");

        // 2) Verificar que la contraseña sea correcta (Identity se encarga del hash)
        var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!ok)
            return Unauthorized("Credenciales inválidas.");

        // 3) Obtener los roles asignados al usuario
        var roles = await _userManager.GetRolesAsync(user);

        // 4) Generar un nuevo token JWT con los datos del usuario autenticado
        var auth = _tokenService.CreateToken(user, roles);

        // 5) Retornar el token al cliente
        return Ok(auth);
    }

    /// <summary>
    /// Endpoint protegido que retorna los datos del usuario autenticado
    /// GET: /api/auth/me
    /// Requiere: Header "Authorization: Bearer {token}"
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserMeDto>> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized("No se pudo identificar al usuario.");

        return Ok(new UserMeDto
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            PhoneNumber = user.PhoneNumber ?? ""
        });
    }

}