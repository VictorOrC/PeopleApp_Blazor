using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PeopleApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "personas",
                keyColumn: "Descripcion",
                keyValue: null,
                column: "Descripcion",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "personas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sku = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "IsActive", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { 1, true, "Coca-Cola 600ml", 22m, "BEB-COCA-600" },
                    { 2, true, "Agua Natural 1L", 15m, "BEB-AGUA-1L" },
                    { 3, true, "Jugo Naranja 500ml", 20m, "BEB-JUG-NAR-500" },
                    { 4, true, "Powerade 500ml", 28m, "BEB-POW-500" },
                    { 5, true, "Café Americano 355ml", 25m, "BEB-CAFE-AME-355" },
                    { 6, true, "Té Helado Limón 600ml", 24m, "BEB-TE-LIM-600" },
                    { 7, true, "Papas Sabritas Original 45g", 18m, "BOT-SAB-ORI-45" },
                    { 8, true, "Doritos Nacho 58g", 20m, "BOT-DOR-NAC-58" },
                    { 9, true, "Cheetos Flamin Hot 52g", 20m, "BOT-CHE-FH-52" },
                    { 10, true, "Ruffles Queso 50g", 20m, "BOT-RUF-QUE-50" },
                    { 11, true, "Takis Fuego 56g", 22m, "BOT-TAK-FUE-56" },
                    { 12, true, "Palomitas Mantequilla 90g", 26m, "BOT-PAL-MAN-90" },
                    { 13, true, "Galletas Marías 170g", 18m, "GAL-MAR-170" },
                    { 14, true, "Galletas Oreo 154g", 30m, "GAL-ORE-154" },
                    { 15, true, "Gansito 50g", 20m, "PAN-GAN-50" },
                    { 16, true, "Chocorroles 2 pzas", 26m, "PAN-CHO-2" },
                    { 17, true, "Barra de Chocolate 45g", 15m, "DUL-CHO-45" },
                    { 18, true, "Gomitas Enchiladas 80g", 18m, "DUL-GOM-ENC-80" },
                    { 19, true, "Sándwich Jamón y Queso", 45m, "ALI-SAN-JQ" },
                    { 20, true, "Ensalada César", 55m, "ALI-ENS-CES" },
                    { 21, true, "Yogurt Fresa 125g", 14m, "LAC-YOG-FRE-125" },
                    { 22, true, "Leche Entera 1L", 28m, "LAC-LEC-ENT-1L" },
                    { 23, true, "Queso Panela 200g", 48m, "LAC-QUE-PAN-200" },
                    { 24, true, "Jamón de Pavo 250g", 52m, "CAR-JAM-PAV-250" },
                    { 25, true, "Pan de Caja Blanco", 38m, "PAN-CAJ-BLA" },
                    { 26, true, "Tortillas de Harina 10 pzas", 32m, "ALI-TOR-HAR-10" },
                    { 27, true, "Arroz 1kg", 30m, "DES-ARR-1K" },
                    { 28, true, "Frijol Negro 1kg", 34m, "DES-FRI-NEG-1K" },
                    { 29, true, "Atún en Agua 140g", 22m, "DES-ATU-AGU-140" },
                    { 30, true, "Aceite Vegetal 900ml", 55m, "DES-ACE-VEG-900" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "personas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
