using JewelryBillingSoft.Application.DTOs;
using JewelryBillingSoft.Domain.Entities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace JewelryBillingSoft.Infrastructure.Printing;

public class InvoicePrinter
{
    public byte[] GenerateInvoicePdf(InvoiceDto invoice, ShopSettings? shopSettings = null)
    {
        using var document = new PdfDocument();
        document.Info.Title = $"Invoice - {invoice.InvoiceNumber}";

        var page = document.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        var gfx = XGraphics.FromPdfPage(page);

        double margin = 40;
        double y = margin;
        double pageWidth = page.Width.Point;
        double contentWidth = pageWidth - 2 * margin;

        // Fonts
        var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
        var headerFont = new XFont("Arial", 11, XFontStyle.Bold);
        var normalFont = new XFont("Arial", 9, XFontStyle.Regular);
        var smallFont = new XFont("Arial", 8, XFontStyle.Regular);
        var boldFont = new XFont("Arial", 10, XFontStyle.Bold);

        // Colors
        var primaryColor = XColor.FromArgb(180, 130, 50); // Gold color
        var darkColor = XColors.DarkSlateGray;
        var lightGray = XColor.FromArgb(245, 245, 245);

        // Header - Shop Name
        var shopName = shopSettings?.ShopName ?? "Jewelry Shop";
        gfx.DrawString(shopName, titleFont, new XSolidBrush(primaryColor),
            new XRect(margin, y, contentWidth, 30), XStringFormats.TopCenter);
        y += 28;

        // Shop details
        if (shopSettings != null)
        {
            gfx.DrawString(shopSettings.ShopAddress ?? "", normalFont, XBrushes.Black,
                new XRect(margin, y, contentWidth, 15), XStringFormats.TopCenter);
            y += 14;
            gfx.DrawString($"Mobile: {shopSettings.Mobile}  |  Email: {shopSettings.Email}  |  GST: {shopSettings.GSTNumber}",
                smallFont, XBrushes.Black,
                new XRect(margin, y, contentWidth, 15), XStringFormats.TopCenter);
            y += 18;
        }

        // Divider
        gfx.DrawLine(new XPen(primaryColor, 2), margin, y, pageWidth - margin, y);
        y += 10;

        // Invoice title
        gfx.DrawString("TAX INVOICE", headerFont, new XSolidBrush(darkColor),
            new XRect(margin, y, contentWidth, 20), XStringFormats.TopCenter);
        y += 22;

        // Invoice details - two columns
        gfx.DrawString($"Invoice No: {invoice.InvoiceNumber}", normalFont, XBrushes.Black, margin, y);
        gfx.DrawString($"Date: {invoice.InvoiceDate:dd/MM/yyyy}", normalFont, XBrushes.Black, margin + contentWidth / 2, y);
        y += 16;

        // Customer details
        gfx.DrawRectangle(new XPen(XColors.LightGray), new XSolidBrush(lightGray),
            new XRect(margin, y, contentWidth, 60));
        y += 5;
        gfx.DrawString("BILL TO:", boldFont, XBrushes.Black, margin + 5, y);
        y += 15;
        gfx.DrawString(invoice.CustomerName, normalFont, XBrushes.Black, margin + 5, y);
        y += 14;
        gfx.DrawString($"Mobile: {invoice.CustomerMobile}", normalFont, XBrushes.Black, margin + 5, y);
        y += 22;

        // Table header
        double[] colWidths = { 30, 150, 50, 50, 50, 60, 60, 70 };
        string[] colHeaders = { "#", "Item Description", "Gr.Wt", "Net Wt", "Purity", "Rate/g", "Making", "Amount" };

        gfx.DrawRectangle(new XSolidBrush(primaryColor), new XRect(margin, y, contentWidth, 20));

        double x = margin;
        for (int i = 0; i < colHeaders.Length; i++)
        {
            gfx.DrawString(colHeaders[i], smallFont, XBrushes.White,
                new XRect(x + 2, y + 3, colWidths[i] - 4, 14), XStringFormats.CenterLeft);
            x += colWidths[i];
        }
        y += 20;

        // Table rows
        for (int idx = 0; idx < invoice.Items.Count; idx++)
        {
            var item = invoice.Items[idx];
            var rowBrush = idx % 2 == 0 ? XBrushes.White : new XSolidBrush(lightGray);
            gfx.DrawRectangle(rowBrush, new XRect(margin, y, contentWidth, 18));

            x = margin;
            var rowData = new string[]
            {
                (idx + 1).ToString(),
                item.ItemDescription,
                $"{item.GrossWeight:F2}g",
                $"{item.NetWeight:F2}g",
                item.Purity,
                $"₹{item.RatePerGram:N0}",
                $"₹{item.MakingCharges:N0}",
                $"₹{item.TotalAmount:N0}"
            };

            for (int c = 0; c < rowData.Length; c++)
            {
                gfx.DrawString(rowData[c], smallFont, XBrushes.Black,
                    new XRect(x + 2, y + 3, colWidths[c] - 4, 12), XStringFormats.CenterLeft);
                x += colWidths[c];
            }
            y += 18;
        }

        // Total section
        gfx.DrawLine(new XPen(XColors.LightGray), margin, y, pageWidth - margin, y);
        y += 8;

        double totalCol = pageWidth - margin - 120;
        var totals = new[]
        {
            ("Sub Total:", $"₹{invoice.SubTotal:N2}"),
            ("Making Charges:", $"₹{invoice.TotalMakingCharges:N2}"),
            ("Discount:", $"-₹{invoice.TotalDiscount:N2}"),
            ("GST (CGST+SGST):", $"₹{invoice.TotalGST:N2}"),
        };

        foreach (var (label, value) in totals)
        {
            gfx.DrawString(label, normalFont, XBrushes.Black, totalCol, y);
            gfx.DrawString(value, normalFont, XBrushes.Black, totalCol + 80, y);
            y += 16;
        }

        // Grand total
        gfx.DrawRectangle(new XSolidBrush(primaryColor), new XRect(totalCol - 5, y, 145, 22));
        gfx.DrawString("GRAND TOTAL:", boldFont, XBrushes.White, totalCol, y + 5);
        gfx.DrawString($"₹{invoice.TotalAmount:N2}", boldFont, XBrushes.White, totalCol + 80, y + 5);
        y += 30;

        // Payment details
        if (invoice.Payments.Any())
        {
            gfx.DrawString("Payment Details:", boldFont, XBrushes.Black, margin, y);
            y += 16;
            foreach (var payment in invoice.Payments)
            {
                gfx.DrawString($"{payment.PaymentMethodName}: ₹{payment.Amount:N2}", normalFont, XBrushes.Black, margin + 10, y);
                y += 14;
            }
        }

        if (invoice.PendingAmount > 0)
        {
            gfx.DrawString($"Pending Amount: ₹{invoice.PendingAmount:N2}", boldFont,
                new XSolidBrush(XColors.Red), margin, y);
            y += 16;
        }

        // Terms
        y += 10;
        gfx.DrawLine(new XPen(XColors.LightGray), margin, y, pageWidth - margin, y);
        y += 8;
        gfx.DrawString("Terms & Conditions:", smallFont, XBrushes.DarkGray, margin, y);
        y += 12;
        gfx.DrawString(shopSettings?.TermsAndConditions ?? "All sales are final. Exchange within 30 days with original bill.",
            smallFont, XBrushes.DarkGray, margin, y);

        // Signature
        y += 30;
        gfx.DrawLine(new XPen(XColors.Black), pageWidth - margin - 120, y, pageWidth - margin, y);
        gfx.DrawString("Authorized Signature", smallFont, XBrushes.Black,
            new XRect(pageWidth - margin - 120, y + 2, 120, 15), XStringFormats.TopCenter);

        // Save to memory stream
        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }

    public void PrintInvoice(InvoiceDto invoice, ShopSettings? shopSettings = null)
    {
        var pdfBytes = GenerateInvoicePdf(invoice, shopSettings);
        var tempFile = Path.GetTempFileName() + ".pdf";
        File.WriteAllBytes(tempFile, pdfBytes);
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo(tempFile) { UseShellExecute = true }
        };
        process.Start();
    }
}

