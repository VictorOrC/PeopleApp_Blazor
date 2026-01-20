using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Dtos.Purchases;
using PeopleApp.Api.Entities;

namespace PeopleApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/purchases")]
public class PurchasesController : ControllerBase
{
    private readonly AppDbContext _db;

    public PurchasesController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> Create([FromBody] PurchaseCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // 1) Cargar productos usados (para validar y obtener precio)
        var productIds = dto.Lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => p.IsActive && productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != productIds.Count)
            return BadRequest("Uno o más productos no existen o están inactivos.");

        // 2) Crear Purchase + Lines congelando UnitPrice
        var purchase = new Purchase
        {
            CustomerName = dto.CustomerName.Trim(),
            Date = dto.Date == default ? DateTime.UtcNow : dto.Date,
            Lines = dto.Lines.Select(l =>
            {
                var product = products.Single(p => p.Id == l.ProductId);
                return new PurchaseLine
                {
                    ProductId = product.Id,
                    Quantity = l.Quantity,
                    Description = l.Description,
                    UnitPrice = product.Price
                };
            }).ToList()
        };

        purchase.Total = purchase.Lines.Sum(x => x.UnitPrice * x.Quantity);

        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();

        // devolver el detalle creado
        return CreatedAtAction(nameof(GetById), new { id = purchase.Id }, await BuildDto(purchase.Id));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseDto>> GetById(int id)
    {
        var dto = await BuildDto(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    private async Task<PurchaseDto?> BuildDto(int id)
    {
        var purchase = await _db.Purchases
            .AsNoTracking()
            .Include(p => p.Lines)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (purchase is null) return null;

        return new PurchaseDto
        {
            Id = purchase.Id,
            Date = purchase.Date,
            CustomerName = purchase.CustomerName,
            Total = purchase.Total,
            Lines = purchase.Lines.Select(l => new PurchaseLineDto
            {
                Id = l.Id,
                ProductId = l.ProductId,
                ProductName = l.Product.Name,
                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity,
                Description = l.Description,
                LineTotal = l.UnitPrice * l.Quantity
            }).ToList()
        };
    }
}
