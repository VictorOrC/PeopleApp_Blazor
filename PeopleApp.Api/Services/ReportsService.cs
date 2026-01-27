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
}
