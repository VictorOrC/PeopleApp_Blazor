using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Models;
using PeopleApp.Api.Entities;

namespace PeopleApp.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Aquí luego tu compañero agregará entidades del CRUD, por ejemplo:
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        ProductSeed.Seed(builder);
    }



}
