using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PeopleApp.Api.Models;

namespace PeopleApp.Api.Services.Pdf;

public static class PersonasPdfBuilder
{
    public static byte[] Build(List<Persona> personas)
    {
        using var ms = new MemoryStream();

        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var doc = new Document(pdf);

        // Fuentes estándar
        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        doc.Add(new Paragraph("Catálogo de Personas")
            .SetFont(fontBold)
            .SetFontSize(18)
            .SetTextAlignment(TextAlignment.CENTER));

        doc.Add(new Paragraph($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}")
            .SetFont(font)
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.RIGHT));

        var table = new Table(6).UseAllAvailableWidth();

        AddHeader(table, "ID", fontBold);
        AddHeader(table, "Nombre", fontBold);
        AddHeader(table, "Edad", fontBold);
        AddHeader(table, "Estatura", fontBold);
        AddHeader(table, "Peso", fontBold);
        AddHeader(table, "Descripción", fontBold);

        foreach (var p in personas)
        {
            table.AddCell(new Cell().Add(new Paragraph(p.Id.ToString()).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(p.Nombre).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(p.Edad.ToString()).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(p.Estatura.ToString("0.##")).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(p.Peso.ToString("0.##")).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(p.Descripcion).SetFont(font)));
        }

        doc.Add(table);
        doc.Close();

        return ms.ToArray();
    }

    private static void AddHeader(Table table, string text, PdfFont fontBold)
    {
        table.AddHeaderCell(
            new Cell().Add(new Paragraph(text).SetFont(fontBold))
        );
    }
}
