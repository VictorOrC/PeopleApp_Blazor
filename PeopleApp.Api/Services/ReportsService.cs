using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Data;
using PeopleApp.Api.Dtos.Reports;

namespace PeopleApp.Api.Services;

public class ReportsService
{
    private readonly AppDbContext _db;

    public ReportsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MonthlyPurchasesDto>> GetMonthlyPurchasesAsync(int months)
    {
        if (months <= 0) months = 12;
        if (months > 36) months = 36; // límite sano para dashboard

        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1).AddMonths(-(months - 1));

        return await _db.Purchases
            .AsNoTracking()
            .Where(p => p.Date >= start)
            .GroupBy(p => new { p.Date.Year, p.Date.Month })
            .Select(g => new MonthlyPurchasesDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                PurchasesCount = g.Count(),
                TotalAmount = g.Sum(x => x.Total)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();
    }

    public async Task<List<DailySalesDto>> GetDailySalesAsync(DateTime from, DateTime to)
    {
        // Normalizamos a fechas (sin hora) y hacemos rango inclusivo
        var start = from.Date;
        var end = to.Date;

        if (end < start)
            (start, end) = (end, start);

        // límite sano para evitar ranges enormes (dashboard)
        if ((end - start).TotalDays > 366)
            end = start.AddDays(366);

        // Query: agrupar por día (Date.Date)
        var grouped = await _db.Purchases
            .AsNoTracking()
            .Where(p => p.Date >= start && p.Date < end.AddDays(1)) // end inclusivo
            .GroupBy(p => p.Date.Date)
            .Select(g => new DailySalesDto
            {
                Date = g.Key,
                PurchasesCount = g.Count(),
                TotalAmount = g.Sum(x => x.Total)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        // Rellenar días faltantes para que el eje X sea continuo
        var map = grouped.ToDictionary(x => x.Date.Date);
        var result = new List<DailySalesDto>();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (map.TryGetValue(d, out var item))
            {
                result.Add(item);
            }
            else
            {
                result.Add(new DailySalesDto
                {
                    Date = d,
                    PurchasesCount = 0,
                    TotalAmount = 0m
                });
            }
        }

        return result;
    }

    public async Task<SalesKpisDto> GetSalesKpisAsync(DateTime from, DateTime to)
    {
        var start = from.Date;
        var end = to.Date;
        if (end < start) (start, end) = (end, start);

        // end inclusivo
        var purchasesQuery = _db.Purchases
            .AsNoTracking()
            .Where(p => p.Date >= start && p.Date < end.AddDays(1));

        var purchasesCount = await purchasesQuery.CountAsync();
        var totalSales = await purchasesQuery.SumAsync(p => (decimal?)p.Total) ?? 0m;

        // TotalItems viene de PurchaseLines en el mismo rango (join por Purchase.Date)
        var totalItems = await _db.PurchaseLines
            .AsNoTracking()
            .Where(pl => pl.Purchase.Date >= start && pl.Purchase.Date < end.AddDays(1))
            .SumAsync(pl => (int?)pl.Quantity) ?? 0;

        return new SalesKpisDto
        {
            PurchasesCount = purchasesCount,
            TotalSales = totalSales,
            AvgTicket = purchasesCount == 0 ? 0m : totalSales / purchasesCount,
            TotalItems = totalItems
        };
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int top)
    {
        var start = from.Date;
        var end = to.Date;
        if (end < start) (start, end) = (end, start);

        if (top <= 0) top = 10;
        if (top > 50) top = 50;

        // Query por líneas dentro del rango (por fecha de la compra)
        // Unimos con Products para obtener ProductName (no dependemos de ProductName en PurchaseLine)
        var query =
            from pl in _db.PurchaseLines.AsNoTracking()
            join p in _db.Products.AsNoTracking() on pl.ProductId equals p.Id
            where pl.Purchase.Date >= start && pl.Purchase.Date < end.AddDays(1)
            group new { pl, p } by new { pl.ProductId, p.Name } into g
            select new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                Quantity = g.Sum(x => x.pl.Quantity),
                TotalAmount = g.Sum(x => x.pl.UnitPrice * x.pl.Quantity)
            };

        return await query
            .OrderByDescending(x => x.TotalAmount)
            .Take(top)
            .ToListAsync();
    }
}
