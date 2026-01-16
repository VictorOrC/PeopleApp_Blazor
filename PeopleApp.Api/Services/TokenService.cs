using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PeopleApp.Api.Dtos.Auth;
using PeopleApp.Api.Models;

namespace PeopleApp.Api.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public AuthResponseDto CreateToken(ApplicationUser user, IList<string> roles)
    {
        // 1) Leer configuración JWT desde appsettings
        var jwtSection = _config.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key no está configurado");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer no está configurado");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience no está configurado");

        var expiresMinutesRaw = jwtSection["ExpiresMinutes"];
        if (!int.TryParse(expiresMinutesRaw, out var expiresMinutes))
            expiresMinutes = 60;

        // 2) Crear la clave de firma (la "firma" del token)
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // 3) Claims = datos dentro del token
        var claims = new List<Claim>
        {
            // ✅ Identity standard (clave)
            new(ClaimTypes.NameIdentifier, user.Id),

            // ✅ JWT standard (opcional pero bien)
            new(JwtRegisteredClaimNames.Sub, user.Id),

            // Mejor usar ClaimTypes.Email (más compatible)
            new(ClaimTypes.Email, user.Email ?? ""),

            new("firstName", user.FirstName ?? ""),
            new("lastName", user.LastName ?? ""),
        };


        // 4) Agregar roles como claims (esto permite [Authorize(Roles="Admin")])
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 5) Expiración
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes);

        // 6) Construir el token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials
        );

        // 7) Convertir a string (lo que el cliente va a guardar)
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            ExpiresAt = expiresAt
        };
    }
}
