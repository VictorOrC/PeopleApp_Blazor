using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;

namespace PeopleApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        term = (term ?? "").Trim();

        if (term.Length < 2)
            return Ok(Array.Empty<object>());

        var items = await _db.Products
            .Where(p => p.IsActive && EF.Functions.Like(p.Name, $"%{term}%"))
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Price
            })
            .Take(10)
            .ToListAsync();

        return Ok(items);
    }
}
