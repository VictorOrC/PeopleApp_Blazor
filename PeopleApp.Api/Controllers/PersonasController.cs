using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Models;
using PeopleApp.Api.Services.Pdf;


// iText
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.Layout.Properties;

// Claims
using System.Security.Claims;

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

    // =========================
    // GET: api/personas
    // =========================
    [HttpGet]
    public async Task<ActionResult<List<Persona>>> GetPersonas()
    {
        return await _context.Personas.ToListAsync();
    }

    // =========================
    // POST: api/personas
    // =========================
    [HttpPost]
    public async Task<IActionResult> CreatePersona(Persona persona)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Personas.Add(persona);
        await _context.SaveChangesAsync();

        return Ok(persona);
    }

    // GET: api/personas/export-pdf
    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var personas = await _context.Personas
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        var pdfBytes = PersonasPdfBuilder.Build(personas);

        Response.Headers["Content-Disposition"] = "inline; filename=personas.pdf";
        return File(pdfBytes, "application/pdf");
    }




}
