using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PeopleApp.Api.Models; // donde está ApplicationUser

namespace PeopleApp.Api.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        // 1) Servicios de Identity
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // 2) Leer configuración
        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        var firstName = config["SeedAdmin:FirstName"] ?? "Admin";
        var lastName = config["SeedAdmin:LastName"] ?? "User";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return; // si no configuras SeedAdmin, no hace nada

        // 3) Asegurar roles
        var roles = new[] { "Admin", "User" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole(r));
        }

        // 4) Crear admin si no existe
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,

                // Si tu ApplicationUser tiene estos campos:
                FirstName = firstName,
                LastName = lastName,

                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creando admin: {errors}");
            }
        }

        // 5) Asegurar rol Admin
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}
