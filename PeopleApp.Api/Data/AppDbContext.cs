using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Models;

namespace PeopleApp.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Aquí luego tu compañero agregará entidades del CRUD, por ejemplo:
    // public DbSet<Person> Persons => Set<Person>();
}
