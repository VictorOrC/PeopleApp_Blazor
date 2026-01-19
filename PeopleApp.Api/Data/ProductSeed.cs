using Microsoft.EntityFrameworkCore;
using PeopleApp.Api.Entities;

namespace PeopleApp.Api.Data;

public static class ProductSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Coca-Cola 600ml", Sku = "BEB-COCA-600", Price = 22m, IsActive = true },
            new Product { Id = 2, Name = "Agua Natural 1L", Sku = "BEB-AGUA-1L", Price = 15m, IsActive = true },
            new Product { Id = 3, Name = "Jugo Naranja 500ml", Sku = "BEB-JUG-NAR-500", Price = 20m, IsActive = true },
            new Product { Id = 4, Name = "Powerade 500ml", Sku = "BEB-POW-500", Price = 28m, IsActive = true },
            new Product { Id = 5, Name = "Café Americano 355ml", Sku = "BEB-CAFE-AME-355", Price = 25m, IsActive = true },
            new Product { Id = 6, Name = "Té Helado Limón 600ml", Sku = "BEB-TE-LIM-600", Price = 24m, IsActive = true },

            new Product { Id = 7, Name = "Papas Sabritas Original 45g", Sku = "BOT-SAB-ORI-45", Price = 18m, IsActive = true },
            new Product { Id = 8, Name = "Doritos Nacho 58g", Sku = "BOT-DOR-NAC-58", Price = 20m, IsActive = true },
            new Product { Id = 9, Name = "Cheetos Flamin Hot 52g", Sku = "BOT-CHE-FH-52", Price = 20m, IsActive = true },
            new Product { Id = 10, Name = "Ruffles Queso 50g", Sku = "BOT-RUF-QUE-50", Price = 20m, IsActive = true },
            new Product { Id = 11, Name = "Takis Fuego 56g", Sku = "BOT-TAK-FUE-56", Price = 22m, IsActive = true },
            new Product { Id = 12, Name = "Palomitas Mantequilla 90g", Sku = "BOT-PAL-MAN-90", Price = 26m, IsActive = true },

            new Product { Id = 13, Name = "Galletas Marías 170g", Sku = "GAL-MAR-170", Price = 18m, IsActive = true },
            new Product { Id = 14, Name = "Galletas Oreo 154g", Sku = "GAL-ORE-154", Price = 30m, IsActive = true },
            new Product { Id = 15, Name = "Gansito 50g", Sku = "PAN-GAN-50", Price = 20m, IsActive = true },
            new Product { Id = 16, Name = "Chocorroles 2 pzas", Sku = "PAN-CHO-2", Price = 26m, IsActive = true },
            new Product { Id = 17, Name = "Barra de Chocolate 45g", Sku = "DUL-CHO-45", Price = 15m, IsActive = true },
            new Product { Id = 18, Name = "Gomitas Enchiladas 80g", Sku = "DUL-GOM-ENC-80", Price = 18m, IsActive = true },

            new Product { Id = 19, Name = "Sándwich Jamón y Queso", Sku = "ALI-SAN-JQ", Price = 45m, IsActive = true },
            new Product { Id = 20, Name = "Ensalada César", Sku = "ALI-ENS-CES", Price = 55m, IsActive = true },
            new Product { Id = 21, Name = "Yogurt Fresa 125g", Sku = "LAC-YOG-FRE-125", Price = 14m, IsActive = true },
            new Product { Id = 22, Name = "Leche Entera 1L", Sku = "LAC-LEC-ENT-1L", Price = 28m, IsActive = true },
            new Product { Id = 23, Name = "Queso Panela 200g", Sku = "LAC-QUE-PAN-200", Price = 48m, IsActive = true },
            new Product { Id = 24, Name = "Jamón de Pavo 250g", Sku = "CAR-JAM-PAV-250", Price = 52m, IsActive = true },

            new Product { Id = 25, Name = "Pan de Caja Blanco", Sku = "PAN-CAJ-BLA", Price = 38m, IsActive = true },
            new Product { Id = 26, Name = "Tortillas de Harina 10 pzas", Sku = "ALI-TOR-HAR-10", Price = 32m, IsActive = true },
            new Product { Id = 27, Name = "Arroz 1kg", Sku = "DES-ARR-1K", Price = 30m, IsActive = true },
            new Product { Id = 28, Name = "Frijol Negro 1kg", Sku = "DES-FRI-NEG-1K", Price = 34m, IsActive = true },
            new Product { Id = 29, Name = "Atún en Agua 140g", Sku = "DES-ATU-AGU-140", Price = 22m, IsActive = true },
            new Product { Id = 30, Name = "Aceite Vegetal 900ml", Sku = "DES-ACE-VEG-900", Price = 55m, IsActive = true }
        );
    }
}
