using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Models;

namespace PeopleApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PersonasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Persona>>> GetPersonas()
    {
        return await _context.Personas.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePersona(Persona persona)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Personas.Add(persona);
        await _context.SaveChangesAsync();

        return Ok(persona);
    }

}
