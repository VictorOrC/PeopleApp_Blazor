using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Models;

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

    // =========================
    // GET: api/personas/export-pdf
    // =========================
    [HttpGet("export-pdf")]
    public IActionResult ExportPdf()
    {
        var personas = _context.Personas.ToList();

        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // ===== TÍTULO =====
        document.Add(
            new Paragraph("Documentación de personas")
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
        );
        document.Add(new Paragraph("\n"));

        // ===== USUARIO (solo lo que realmente existe) =====
        string correo =
            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            ?? "correo@ejemplo.com";

        var headerTable = new Table(2).UseAllAvailableWidth();

        // Izquierda: correo
        headerTable.AddCell(
            new Cell()
                .Add(new Paragraph(correo))
                .SetBorder(Border.NO_BORDER)
        );

        // Derecha: lugar, fecha y hora
        headerTable.AddCell(
            new Cell()
                .Add(new Paragraph(
                    "Los Mochis, Sinaloa\n" +
                    $"Fecha: {DateTime.Now:dd/MM/yyyy}\n" +
                    $"Hora: {DateTime.Now:HH:mm}"
                ))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(Border.NO_BORDER)
        );

        document.Add(headerTable);
        document.Add(new Paragraph("\n"));

        // ===== TABLA DE PERSONAS =====
        var table = new Table(2).UseAllAvailableWidth();
        table.AddHeaderCell("Nombre");
        table.AddHeaderCell("Descripción");

        foreach (var p in personas)
        {
            table.AddCell(p.Nombre);
            table.AddCell(p.Descripcion);
        }

        document.Add(table);

        // ===== PIE DE PÁGINA =====
        document.Add(new Paragraph("\n"));
        document.Add(
            new Paragraph(
                "Todos los derechos reservados a S.A de C.V por cualquier situación que represente algún problema " +
                "denominante o el uso de otras palabras y nombres erróneos o mal representados en contextos aparente " +
                "pero intencionalmente semi formales de las que se desconoce su significado o propósito en el momento " +
                "y lugar presentes (20/01/2026, 10:24 a.m. Los Mochis, Sinaloa.)"
            )
            .SetFontSize(8)
            .SetTextAlignment(TextAlignment.CENTER)
        );

        document.Close();

        return File(ms.ToArray(), "application/pdf", "Personas.pdf");
    }
}
