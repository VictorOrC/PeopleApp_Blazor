using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers (API real)
builder.Services.AddControllers();

// 2) OpenAPI / Swagger (tu template usa AddOpenApi; lo dejamos)
builder.Services.AddOpenApi();

// 3) DbContext (MySQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 4) Identity (usuarios + roles) usando EF Core
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Aquí después ajustas políticas de password si quieres
        // options.Password.RequiredLength = 8; etc.
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

// 5) CORS (para que el Client WASM consuma tu API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true); // DEV ONLY (luego lo restringes)
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 6) CORS antes de mapear controllers
app.UseCors("ClientPolicy");

// 7) (Más adelante) AuthN/AuthZ para JWT
// app.UseAuthentication();
app.UseAuthorization();

// 8) Endpoints de controllers
app.MapControllers();

app.Run();
