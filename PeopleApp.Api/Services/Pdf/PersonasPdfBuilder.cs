using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using PeopleApp.Api.Models;
using iText.Kernel.Pdf.Canvas.Draw;



namespace PeopleApp.Api.Services.Pdf;

public static class PersonasPdfBuilder
{
    public static byte[] Build(List<Persona> personas)
    {
        using var ms = new MemoryStream();

        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);

        // Tamaño carta/letter. Si prefieres A4: PageSize.A4
        var doc = new Document(pdf, PageSize.LETTER);
        doc.SetMargins(36, 36, 42, 36); // top, right, bottom, left

        // Fuentes
        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        // ======== Header corporativo (logo + nombre + fecha) ========
        var header = BuildHeader(font, fontBold);
        doc.Add(header);

        // Separador
        doc.Add(new LineSeparator(new SolidLine())
            .SetMarginTop(8)
            .SetMarginBottom(14));


        // ======== Título ========
        doc.Add(new Paragraph("Catálogo de Personas")
            .SetFont(fontBold)
            .SetFontSize(16)
            .SetMarginBottom(10));

        // ======== Tabla ========
        var table = new Table(new float[] { 50, 160, 60, 75, 60, 205 })
            .UseAllAvailableWidth();

        // Estilos corporativos
        var headerBg = new DeviceRgb(240, 242, 245); // gris muy claro
        var borderColor = new DeviceRgb(210, 214, 220);

        AddHeader(table, "ID", fontBold, headerBg, borderColor);
        AddHeader(table, "Nombre", fontBold, headerBg, borderColor);
        AddHeader(table, "Edad", fontBold, headerBg, borderColor);
        AddHeader(table, "Estatura", fontBold, headerBg, borderColor);
        AddHeader(table, "Peso", fontBold, headerBg, borderColor);
        AddHeader(table, "Descripción", fontBold, headerBg, borderColor);

        // Zebra rows
        var zebra = new DeviceRgb(250, 250, 252);

        for (int i = 0; i < personas.Count; i++)
        {
            var p = personas[i];
            var rowBg = (i % 2 == 0) ? ColorConstants.WHITE : zebra;

            AddCell(table, p.Id.ToString(), font, rowBg, borderColor, TextAlignment.LEFT);
            AddCell(table, p.Nombre, font, rowBg, borderColor, TextAlignment.LEFT);
            AddCell(table, p.Edad.ToString(), font, rowBg, borderColor, TextAlignment.RIGHT);
            AddCell(table, p.Estatura.ToString("0.##"), font, rowBg, borderColor, TextAlignment.RIGHT);
            AddCell(table, p.Peso.ToString("0.##"), font, rowBg, borderColor, TextAlignment.RIGHT);
            AddCell(table, p.Descripcion, font, rowBg, borderColor, TextAlignment.LEFT);
        }

        doc.Add(table);

        // ======== Footer con paginado ========
        doc.Flush(); // importante para que ya existan las páginas
        StampPageNumbers(pdf, font);


        doc.Close();
        return ms.ToArray();
    }

    // ----------------------------
    // Header
    // ----------------------------
    private static Table BuildHeader(PdfFont font, PdfFont fontBold)
    {
        var headerTable = new Table(new float[] { 80, 1, 180 })
            .UseAllAvailableWidth()
            .SetBorder(Border.NO_BORDER);

        // Logo (si no existe, no truena)
        var logoCell = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);

        var logoPath = System.IO.Path.Combine(AppContext.BaseDirectory, "assets", "logo.png");

        if (File.Exists(logoPath))
        {
            var imgData = ImageDataFactory.Create(logoPath);
            var img = new Image(imgData).SetAutoScale(true).SetMaxHeight(42);
            logoCell.Add(img);
        }
        else
        {
            // fallback: texto si no hay logo
            logoCell.Add(new Paragraph("PeopleApp").SetFont(fontBold).SetFontSize(14));
        }

        headerTable.AddCell(logoCell);

        // Separador vertical “fino”
        headerTable.AddCell(new Cell()
            .SetBorder(Border.NO_BORDER)
            .SetWidth(1)
            .SetBackgroundColor(new DeviceRgb(230, 230, 230)));

        // Datos derecho
        var right = new Cell().SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE);

        right.Add(new Paragraph("PeopleApp • Reporte")
            .SetFont(fontBold)
            .SetFontSize(12)
            .SetMarginBottom(2));

        right.Add(new Paragraph($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}")
            .SetFont(font)
            .SetFontSize(9)
            .SetFontColor(new DeviceRgb(90, 90, 90)));

        headerTable.AddCell(right);

        return headerTable;
    }

    // ----------------------------
    // Header cells
    // ----------------------------
    private static void AddHeader(Table table, string text, PdfFont fontBold, Color bg, Color borderColor)
    {
        var cell = new Cell()
            .Add(new Paragraph(text).SetFont(fontBold).SetFontSize(10))
            .SetBackgroundColor(bg)
            .SetBorder(new SolidBorder(borderColor, 1))
            .SetPadding(6);

        table.AddHeaderCell(cell);
    }

    // ----------------------------
    // Body cells
    // ----------------------------
    private static void AddCell(Table table, string text, PdfFont font, Color bg, Color borderColor, TextAlignment align)
    {
        table.AddCell(new Cell()
            .Add(new Paragraph(text ?? "").SetFont(font).SetFontSize(10))
            .SetTextAlignment(align)
            .SetBackgroundColor(bg)
            .SetBorder(new SolidBorder(borderColor, 1))
            .SetPadding(6));
    }

    private static void StampPageNumbers(PdfDocument pdf, PdfFont font)
    {
        // Asegura que el documento ya haya “materializado” páginas
        var total = pdf.GetNumberOfPages();

        for (int i = 1; i <= total; i++)
        {
            var page = pdf.GetPage(i);
            var pageSize = page.GetPageSize();

            // Posición del footer
            float x = pageSize.GetRight() - 110;
            float y = pageSize.GetBottom() + 18;

            // Dibuja texto en la página
            var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdf);
            canvas.BeginText();
            canvas.SetFontAndSize(font, 9);
            canvas.SetFillColor(new DeviceRgb(110, 110, 110));
            canvas.MoveText(x, y);
            canvas.ShowText($"Página {i} de {total}");
            canvas.EndText();
            canvas.Release();
        }
    }


}
