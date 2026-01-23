using Microsoft.AspNetCore.Authorization;       // Para [Authorize] (proteger endpoints con JWT)
using Microsoft.AspNetCore.Mvc;                 // Para ControllerBase, ActionResult, atributos [HttpGet], etc.
using Microsoft.EntityFrameworkCore;            // Para Include, AsNoTracking, ToListAsync, etc.
using PeopleApp.Api.Data;                       // AppDbContext (EF Core)
using PeopleApp.Api.Dtos.Purchases;             // DTOs: PurchaseDto, PurchaseCreateDto, PurchaseLineDto...
using PeopleApp.Api.Entities;                   // Entidades EF: Purchase, PurchaseLine, Product
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Borders;
using iText.Layout.Properties;
using System.Security.Claims;
using iText.IO.Image;


namespace PeopleApp.Api.Controllers;

// [Authorize] = exige JWT válido. Si no mandas token -> 401 Unauthorized.
// Esto es clave porque compras no deben ser públicas.
[Authorize]

// [ApiController] habilita validaciones automáticas y comportamientos típicos de API.
// Ejemplo: binding de [FromBody], respuestas 400 más consistentes, etc.
[ApiController]

// Define la ruta base: /api/purchases
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    // AppDbContext = la "puerta" a la base de datos por EF Core.
    private readonly AppDbContext _db;

    // Inyección de dependencias (DI):
    // ASP.NET crea el controller y le pasa el DbContext ya configurado.
    public PurchasesController(AppDbContext db) => _db = db;

    // POST /api/purchases
    // Se usa para crear una compra nueva con sus líneas.
    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> Create([FromBody] PurchaseCreateDto dto)
    {
        // Si el DTO no cumple validaciones (Required, Range, etc.), regresamos 400 con detalles.
        // Nota: Con [ApiController], muchas veces esto ya se hace automático,
        // pero así queda explícito.
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        // Normaliza el customer name
        dto.CustomerName = dto.CustomerName.Trim();

        // Reglas de negocio adicionales:
        // Regla: CustomerName no puede estar vacío (solo espacios) 
        if (dto.CustomerName.Length == 0)
            return BadRequest("CustomerName is required.");

        // Normaliza fecha: si viene default usa UtcNow
        var date = dto.Date == default ? DateTime.UtcNow : dto.Date;

        // Regla: no permitir futuro (más de 1 día adelante, por si hay diferencia de hora)
        if (date > DateTime.UtcNow.AddDays(1))
            return BadRequest("Date cannot be in the future.");

        // Regla: no permitir demasiado pasado (ej. más de 30 días atrás)
        if (date < DateTime.UtcNow.AddDays(-30))
            return BadRequest("Date is too old.");


        // 1) Validar productos + obtener precio real desde DB
        // Extraemos los ProductId de las líneas, quitando repetidos.
        var productIds = dto.Lines.Select(l => l.ProductId).Distinct().ToList();

        // Traemos de la DB SOLO los productos activos que coincidan con esos IDs.
        // ¿Por qué? Para:
        // - validar que existen
        // - obtener el Price real para congelarlo en UnitPrice
        var products = await _db.Products
            .Where(p => p.IsActive && productIds.Contains(p.Id))
            .ToListAsync();

        // Si la cantidad que encontramos en DB no coincide con los IDs solicitados,
        // significa que algún ProductId no existe o está inactivo.
        if (products.Count != productIds.Count)
            return BadRequest("Uno o más productos no existen o están inactivos.");

        // 2) Construir la entidad Purchase (encabezado) y sus PurchaseLine (detalle)
        // Nota: aquí NO confiamos en que el cliente nos mande el precio:
        // el precio lo tomamos de la DB (product.Price) para evitar manipulación.
        var purchase = new Purchase
        {
            // Trim() para limpiar espacios al inicio/final
            CustomerName = dto.CustomerName.Trim(),

            Date = date,

            // Convertimos cada línea del DTO a una entidad PurchaseLine
            Lines = dto.Lines.Select(l =>
            {
                // Buscamos el producto correspondiente en la lista ya cargada.
                // Single() asegura que haya exactamente 1.
                var product = products.Single(p => p.Id == l.ProductId);

                return new PurchaseLine
                {
                    // FK del producto
                    ProductId = product.Id,

                    // Cantidad
                    Quantity = l.Quantity,

                    // Descripción opcional
                    Description = l.Description,

                    // "Congelar" precio del producto en este momento.
                    // Esto es clave para que si mañana cambia el precio del producto,
                    // las compras pasadas NO cambien.
                    UnitPrice = product.Price
                };
            }).ToList()
        };

        // Calcular total en el backend (no confiar en el cliente).
        // Total = suma de (UnitPrice * Quantity) de cada línea
        purchase.Total = purchase.Lines.Sum(x => x.UnitPrice * x.Quantity);

        // Guardar en DB:
        // - _db.Purchases.Add() agrega el Purchase
        // - y EF detecta también Lines y las inserta por relación
        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();

        // Devolver el recurso creado (201 Created) y además el DTO completo.
        // CreatedAtAction:
        // - coloca el header "Location" apuntando a GET /api/purchases/{id}
        // - y te regresa el body con el DTO (más útil para el frontend)
        return CreatedAtAction(nameof(GetById), new { id = purchase.Id }, await BuildDto(purchase.Id));
    }

    // GET /api/purchases/{id}
    // Se usa para consultar una compra por Id, incluyendo sus líneas y productos.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseDto>> GetById(int id)
    {
        // Construimos el DTO desde DB
        var dto = await BuildDto(id);

        // Si no existe -> 404
        if (dto is null) return NotFound();

        // Si existe -> 200 OK con el DTO
        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<List<PurchaseListItemDto>>> GetAll()
    {
        var items = await _db.Purchases
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .Select(p => new PurchaseListItemDto
            {
                Id = p.Id,
                Date = p.Date,
                CustomerName = p.CustomerName,
                Total = p.Total
            })
            .ToListAsync();

        return Ok(items);
    }


    // Método privado para armar el DTO de una compra completa.
    // ¿Por qué existe? Para no repetir el mismo mapping en Create y GetById.
    private async Task<PurchaseDto?> BuildDto(int id)
    {
        // Traemos la compra desde DB:
        // - AsNoTracking() => más rápido, porque solo estamos leyendo (no editaremos)
        // - Include(p => p.Lines) => incluye las líneas (detalle)
        // - ThenInclude(l => l.Product) => incluye el producto de cada línea (para obtener Name)
        var purchase = await _db.Purchases
            .AsNoTracking()
            .Include(p => p.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        // Si no existe en DB, regresamos null
        if (purchase is null) return null;

        // Convertimos entidad -> DTO (lo que se envía al cliente)
        // Importante: aquí controlas exactamente qué campos expones.
        return new PurchaseDto
        {
            Id = purchase.Id,
            Date = purchase.Date,
            CustomerName = purchase.CustomerName,
            Total = purchase.Total,

            // Mapping de cada línea a DTO
            Lines = purchase.Lines.Select(l => new PurchaseLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,

                // Product.Name lo tenemos gracias a ThenInclude
                ProductName = l.Product.Name,

                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity,
                Description = l.Description,

                // LineTotal lo calculamos (no dependemos del cliente)
                LineTotal = l.UnitPrice * l.Quantity
            }).ToList()
        };
    }

    // GET /api/purchases/{id}/export-pdf
    [HttpGet("{id:int}/export-pdf")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        // Reutilizamos el mismo método que ya existe
        var purchase = await BuildDto(id);
        if (purchase is null) return NotFound();

        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        // ===== LOGO =====
        var logoPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Assets",
            "logo.png"
        );

        var logoData = ImageDataFactory.Create(logoPath);
        var logo = new Image(logoData)
        .ScaleToFit(140, 140)
        .SetFixedPosition(36, pdf.GetDefaultPageSize().GetTop() - 80);

        document.Add(new Paragraph("\n"));
        document.Add(logo);

        // ===== FECHA =====

        var header = new Table(2).UseAllAvailableWidth();

        header.AddCell(
            new Cell()
                .Add(new Paragraph(
                    $"Lugar: Los Mochis, Sinaloa\n" +
                    $"Fecha: {DateTime.Now:dd/MM/yyyy}\n" +
                    $"Hora: {DateTime.Now:HH:mm}"
                ))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(Border.NO_BORDER)
        );

        document.Add(header);

        // ===== TÍTULO =====
        document.Add(
            new Paragraph("Documentación de compra")
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER)
        );

        document.Add(new Paragraph("\n"));

        // ===== DATOS DE COMPRA =====
        document.Add(new Paragraph($"Compra #{purchase.Id}"));
        document.Add(new Paragraph($"Cliente: {purchase.CustomerName}"));
        document.Add(new Paragraph($"Total: {purchase.Total:C}"));
        document.Add(new Paragraph("\n"));

        // ===== TABLA =====
        var table = new Table(5).UseAllAvailableWidth();
        table.AddHeaderCell("Producto");
        table.AddHeaderCell("Cant.");
        table.AddHeaderCell("Precio");
        table.AddHeaderCell("Subtotal");
        table.AddHeaderCell("Descripción");

        foreach (var l in purchase.Lines)
        {
            table.AddCell(l.ProductName);
            table.AddCell(l.Quantity.ToString());
            table.AddCell(l.UnitPrice.ToString("C"));
            table.AddCell(l.LineTotal.ToString("C"));
            table.AddCell(l.Description ?? "");
        }

        document.Add(table);

        // ===== PIE DE PÁGINA =====
        document.Add(new Paragraph("\n"));
        document.Add(
            new Paragraph(
                "Todos los derechos reservados a S.A de C.V por cualquier situación que represente " +
                "algún problema denominante o el uso de otras palabras y nombres erróneos o mal " +
                "representados en contextos aparentes pero intencionalmente semi formales. " +
                "(20/01/2026, 10:24 a.m. Los Mochis, Sinaloa.)"
            )
            .SetFontSize(8)
            .SetTextAlignment(TextAlignment.CENTER)
        );

        document.Close();

        return File(ms.ToArray(), "application/pdf", $"Compra_{purchase.Id}.pdf");
    }

}
