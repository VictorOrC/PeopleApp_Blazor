using Microsoft.EntityFrameworkCore;            // EF Core: consultas async (CountAsync, ToListAsync), AsNoTracking, etc.
using Microsoft.Extensions.Options;             // Para leer opciones tipadas desde appsettings (IOptions<T>)
using PeopleApp.Api.Entities;                   // Tus entidades: Purchase, PurchaseLine, Product
using PeopleApp.Api.Options;                    // DemoSeedOptions (configuración del seeding)

namespace PeopleApp.Api.Data;

public class DemoPurchaseSeeder
{
    // DbContext de la app: acceso a tablas (DbSet) Purchases, Products, etc.
    private readonly AppDbContext _db;

    // Opciones de seeding leídas desde appsettings.Development.json (DemoSeed section)
    private readonly DemoSeedOptions _opt;

    // Constructor con DI: se inyecta el DbContext y las opciones configuradas en Program.cs
    public DemoPurchaseSeeder(AppDbContext db, IOptions<DemoSeedOptions> opt)
    {
        _db = db;

        // opt.Value contiene el objeto ya mapeado desde configuración:
        // DemoSeed: { Enabled, Purchases, DaysBack, MaxLinesPerPurchase }
        _opt = opt.Value;
    }

    // Método principal: siembra compras demo (solo si está habilitado)
    public async Task SeedAsync()
    {
        // 1) Feature flag: si está apagado, no hacemos nada
        if (!_opt.Enabled) return;

        // 2) Idempotencia: revisamos cuántas compras existen para no sembrar infinito
        var existing = await _db.Purchases.CountAsync();

        // Si ya tenemos al menos la cantidad objetivo, terminamos
        if (existing >= _opt.Purchases) return;

        // 3) Necesitamos productos existentes para crear líneas de compra
        // AsNoTracking = mejor rendimiento: solo lectura, EF no "trackea" los objetos
        var products = await _db.Products.AsNoTracking().ToListAsync();

        // Si no hay productos, no podemos crear líneas coherentes
        if (products.Count == 0) return;

        // 4) Random con seed fijo => datos reproducibles (siempre genera el mismo patrón)
        // Útil para demos y para que no cambie cada vez que corres la app
        var rnd = new Random(12345);

        // Cantidad de compras que faltan para llegar al objetivo (Purchases)
        var purchasesToCreate = _opt.Purchases - existing;

        // Fecha de inicio: hoy - DaysBack (en UTC) y normalizado a medianoche (Date)
        // Esto hace que las compras queden distribuidas en el rango de últimos N días
        var start = DateTime.UtcNow.Date.AddDays(-_opt.DaysBack);

        // 5) Creamos N compras (las faltantes)
        for (int i = 0; i < purchasesToCreate; i++)
        {
            // dayOffset en el rango [0..DaysBack] (con protección para DaysBack <= 0)
            // Math.Max(1, DaysBack + 1) evita rnd.Next(0,0) que explota
            var dayOffset = rnd.Next(0, Math.Max(1, _opt.DaysBack + 1));

            // Fecha final de la compra dentro del rango (start + offset)
            var date = start.AddDays(dayOffset);

            // Creamos la entidad Purchase
            var purchase = new Purchase
            {
                Date = date,
                // Nombre "fake" para identificar que es data demo
                CustomerName = $"Cliente Demo {rnd.Next(1, 9999)}",
                // Inicializamos lista de líneas (relación 1..N)
                Lines = new List<PurchaseLine>()
            };

            // 6) Determinamos cuántas líneas tendrá esta compra
            // rnd.Next(min, maxExclusive) => el +1 es para hacerlo inclusivo al máximo deseado
            // Math.Max(2, ...) asegura que maxExclusive sea >= 2 (si no, Next truena)
            var linesCount = rnd.Next(1, Math.Max(2, _opt.MaxLinesPerPurchase + 1));

            // Para evitar productos duplicados dentro de la misma compra
            var used = new HashSet<int>();

            // 7) Creamos cada línea (PurchaseLine)
            for (int l = 0; l < linesCount; l++)
            {
                // Elegimos un producto aleatorio de la lista existente
                var product = products[rnd.Next(products.Count)];

                // Si ya usamos este producto en esta compra, saltamos (evita duplicados)
                if (!used.Add(product.Id)) continue;

                // Cantidad aleatoria (1..5)
                var qty = rnd.Next(1, 6);

                // Tomamos el precio del producto de la tabla Products (consistencia)
                var unitPrice = product.Price;

                // Agregamos la línea a la compra
                purchase.Lines.Add(new PurchaseLine
                {
                    ProductId = product.Id,
                    UnitPrice = unitPrice,
                    Quantity = qty,
                    Description = "Compra demo"
                    // No asignamos LineTotal porque en tu entidad es calculado o read-only
                });
            }

            // 8) Calculamos el total de la compra (coherente con sus líneas)
            // Sumamos UnitPrice * Quantity de cada línea
            purchase.Total = purchase.Lines.Sum(x => x.UnitPrice * x.Quantity);

            // 9) Evitamos insertar compras vacías (por si todas las líneas se saltaron por duplicados)
            if (purchase.Lines.Count > 0)
                _db.Purchases.Add(purchase); // EF rastrea la Purchase y sus Lines (por la relación)
        }

        // 10) Un solo SaveChanges para todo: más eficiente que guardar dentro del loop
        await _db.SaveChangesAsync();
    }
}
