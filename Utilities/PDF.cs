using System;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using ERP_Fix.Models;

namespace ERP_Fix.Utilities
{
    public class PDF
    {
        private static void Settings()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static void Bill(Company company, Bill bill)
        {
            Settings();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));
                    page.Header().Row(header =>
                    {
                        // Left: Invoice title and company
                        header.RelativeItem().Column(col =>
                        {
                            col.Item().Text("RECHNUNG").FontSize(24).Bold().FontColor(Colors.Blue.Medium);
                            col.Item().Text(company.Name).Bold();
                            col.Item().Text($"{company.Address.Street}\n{company.Address.PostalCode} {company.Address.City}, {company.Address.Country}").FontSize(10);
                            col.Item().Text($"{company.Email}\n{company.PhoneNumber}");
                        });

                        // Right: Invoice metadata
                        header.ConstantItem(200).Column(col =>
                        {
                            col.Item().Text("Rechnungsnr: 0001").AlignRight();
                            col.Item().Text($"Datum: {bill.BillDate}").AlignRight();
                            col.Item().Text($"Kunde: {bill.Customer.Name}").AlignRight();
                            var addr = bill.Customer?.Information?.Address;
                            var addrText = addr != null
                                ? $"{addr.Street}\n{addr.PostalCode} {addr.City}, {addr.Country}"
                                : "";
                            col.Item().Text(addrText).AlignRight();
                        });
                    });

                    page.Content().PaddingVertical(20).Column(content =>
                    {
                        // Table header
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header row
                            table.Cell().Element(CellStyleHeader).Text("Produkt").SemiBold();
                            table.Cell().Element(CellStyleHeader).Text("Anzahl").SemiBold();
                            table.Cell().Element(CellStyleHeader).Text("Stückpreis").SemiBold();
                            table.Cell().Element(CellStyleHeader).Text("Preis").SemiBold();

                            bill.Order.Articles.ForEach(article =>
                            {
                                var typeName = article.Type.Name;
                                var qty = article.Stock;
                                // Lookup unit price; fall back to 0.00 if not present
                                double unitPrice = 0.0;
                                if (bill.Prices?.PriceList != null && bill.Prices.PriceList.TryGetValue(article.Type, out var found))
                                    unitPrice = found;
                                var lineTotal = unitPrice * qty;

                                table.Cell().Element(CellStyle).Text(typeName);
                                table.Cell().Element(CellStyle).Text(qty.ToString());
                                table.Cell().Element(CellStyle).Text(unitPrice.ToString("C2", CultureInfo.CurrentCulture));
                                table.Cell().Element(CellStyle).Text(lineTotal.ToString("C2", CultureInfo.CurrentCulture));
                            });

                            // Total row
                            table.Cell().ColumnSpan(3).Element(CellStyle).AlignRight().Text("Gesamt:").SemiBold();
                            table.Cell().Element(CellStyle).Text(bill.TotalPrice.ToString("C2", CultureInfo.CurrentCulture)).SemiBold();
                        });

                        // Payment info & thanks
                        content.Item().PaddingTop(25).Text($"Bitte überweisen Sie den Betrag innerhalb der nächsten {bill.PaymentTerms.DaysUntilDue} Tage an uns:\nIBAN: {company.BankInfo.IBAN}\nBIC: {company.BankInfo.BIC}\nBank: {company.BankInfo.BankName}").FontSize(10);

                        content.Item().PaddingTop(10).Text("Vielen Dank für Ihren Auftrag!").Italic().FontColor(Colors.Grey.Darken2);
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span($"{company.Name}\nErstellt mit ERP").FontSize(8);
                    });
                });
            })
            .GeneratePdf($"Bill-{((bill.Customer?.Name ?? "Customer").Replace(" ", ""))}-{bill.Id}.pdf");
        }

        static IContainer CellStyle(IContainer container) =>
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(2);

        static IContainer CellStyleHeader(IContainer container) =>
        container.Background(Colors.Grey.Lighten4).PaddingVertical(5).PaddingHorizontal(2);
    }
}
