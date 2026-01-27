using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleApp.Api.Dtos.Reports;
using PeopleApp.Api.Services;

namespace PeopleApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // si quieres solo Admin, cambia a [Authorize(Roles="Admin")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reports;

    public ReportsController(ReportsService reports)
    {
        _reports = reports;
    }

    [HttpGet("purchases/monthly")]
    public async Task<ActionResult<List<MonthlyPurchasesDto>>> GetMonthly([FromQuery] int months = 12)
    {
        var data = await _reports.GetMonthlyPurchasesAsync(months);
        return Ok(data);
    }

    [HttpGet("purchases/daily")]
    public async Task<ActionResult<List<DailySalesDto>>> GetDaily(
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null)
    {
        // default: últimos 7 días
        var end = (to ?? DateTime.UtcNow).Date;
        var start = (from ?? end.AddDays(-6)).Date;

        var data = await _reports.GetDailySalesAsync(start, end);
        return Ok(data);
    }


    [HttpGet("purchases/kpis")]
    public async Task<ActionResult<SalesKpisDto>> GetKpis(
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null)
    {
        var end = (to ?? DateTime.UtcNow).Date;
        var start = (from ?? end.AddDays(-6)).Date;

        var data = await _reports.GetSalesKpisAsync(start, end);
        return Ok(data);
    }

    [HttpGet("products/top")]
    public async Task<ActionResult<List<TopProductDto>>> GetTopProducts(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int top = 10)
    {
        var end = (to ?? DateTime.UtcNow).Date;
        var start = (from ?? end.AddDays(-6)).Date;

        var data = await _reports.GetTopProductsAsync(start, end, top);
        return Ok(data);
    }

}
