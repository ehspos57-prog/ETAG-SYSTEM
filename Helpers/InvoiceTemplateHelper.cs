using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.Data;

namespace ETAG_ERP.Helpers
{
    public static class InvoiceTemplateHelper
    {
        /// <summary>
        /// Generate PDF invoice
        /// </summary>
        public static void GeneratePdfInvoice(Invoice invoice, string filePath)
        {
            try
            {
                using (var document = new Document(PageSize.A4, 20, 20, 20, 20))
                {
                    using (var writer = Models.PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                    {
                        document.Open();

                        // Add company header
                        AddCompanyHeader(document);

                        // Add invoice header
                        AddInvoiceHeader(document, invoice);

                        // Add client information
                        AddClientInformation(document, invoice);

                        // Add items table
                        AddItemsTable(document, invoice);

                        // Add totals
                        AddTotals(document, invoice);

                        // Add footer
                        AddFooter(document);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Generate PDF Invoice");
            }
        }

        /// <summary>
        /// Generate Excel invoice
        /// </summary>
        public static void GenerateExcelInvoice(Invoice invoice, string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("فاتورة");

                    // Add company header
                    AddCompanyHeaderExcel(worksheet);

                    // Add invoice header
                    AddInvoiceHeaderExcel(worksheet, invoice);

                    // Add client information
                    AddClientInformationExcel(worksheet, invoice);

                    // Add items table
                    AddItemsTableExcel(worksheet, invoice);

                    // Add totals
                    AddTotalsExcel(worksheet, invoice);

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Save workbook
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Generate Excel Invoice");
            }
        }

        /// <summary>
        /// Add company header to PDF
        /// </summary>
        private static void AddCompanyHeader(Document document)
        {
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
            var title = new Paragraph("شركة إيتاج للمعدات الصناعية", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            document.Add(title);

            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
            var subtitle = new Paragraph("معدات هيدروليكية - هوائية - ميكانيكية", subtitleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(subtitle);
        }

        /// <summary>
        /// Add invoice header to PDF
        /// </summary>
        private static void AddInvoiceHeader(Document document, Invoice invoice)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
            var header = new Paragraph("فاتورة مبيعات", headerFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(header);

            var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            var info = new Paragraph($"رقم الفاتورة: {invoice.InvoiceNumber} | التاريخ: {invoice.InvoiceDate:yyyy/MM/dd}", infoFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(info);
        }

        /// <summary>
        /// Add client information to PDF
        /// </summary>
        private static void AddClientInformation(Document document, Invoice invoice)
        {
            var clientFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
            var clientTitle = new Paragraph("بيانات العميل:", clientFont)
            {
                SpacingAfter = 10
            };
            document.Add(clientTitle);

            var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            var clientInfo = new Paragraph($"الاسم: {invoice.ClientName}\nالعنوان: {invoice.Client?.Address ?? ""}\nالهاتف: {invoice.Client?.Phone ?? ""}", infoFont)
            {
                SpacingAfter = 20
            };
            document.Add(clientInfo);
        }

        /// <summary>
        /// Add items table to PDF
        /// </summary>
        private static void AddItemsTable(Document document, Invoice invoice)
        {
            var table = new PdfPTable(6)
            {
                WidthPercentage = 100,
                SpacingBefore = 10,
                SpacingAfter = 10
            };

            // Set column widths
            table.SetWidths(new float[] { 1, 3, 1, 1, 1, 1 });

            // Add headers
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var headerBackground = new BaseColor(52, 152, 219);

            var headers = new[] { "م", "اسم الصنف", "الكمية", "سعر الوحدة", "الخصم", "الإجمالي" };

            foreach (var header in headers)
            {
                var cell = new PdfPCell(new Phrase(header, headerFont))
                {
                    BackgroundColor = headerBackground,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 8
                };
                table.AddCell(cell);
            }

            // Add data rows
            var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
            var alternateBackground = new BaseColor(248, 249, 250);

            for (int i = 0; i < invoice.Items.Count; i++)
            {
                var item = invoice.Items[i];
                var isEven = i % 2 == 0;
                var total = (item.UnitPrice * item.Quantity) - item.Discount;

                var cells = new[]
                {
                    (i + 1).ToString(),
                    item.ItemName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("N2"),
                    item.Discount.ToString("N2"),
                    total.ToString("N2")
                };

                foreach (var cellText in cells)
                {
                    var cell = new PdfPCell(new Phrase(cellText, dataFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 6,
                        BackgroundColor = isEven ? BaseColor.WHITE : alternateBackground
                    };
                    table.AddCell(cell);
                }
            }

            document.Add(table);
        }

        /// <summary>
        /// Add totals to PDF
        /// </summary>
        private static void AddTotals(Document document, Invoice invoice)
        {
            var totalsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
            var subtotal = invoice.Items.Sum(i => i.UnitPrice * i.Quantity);
            var totalDiscount = invoice.Items.Sum(i => i.Discount);
            var total = subtotal - totalDiscount;

            var totals = new Paragraph($"المجموع الفرعي: {subtotal:N2} جنيه\nالخصم: {totalDiscount:N2} جنيه\nالإجمالي: {total:N2} جنيه", totalsFont)
            {
                Alignment = Element.ALIGN_LEFT,
                SpacingBefore = 20
            };
            document.Add(totals);
        }

        /// <summary>
        /// Add footer to PDF
        /// </summary>
        private static void AddFooter(Document document)
        {
            var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            var footer = new Paragraph("شكراً لاختياركم شركة إيتاج للمعدات الصناعية\nهاتف: 01234567890 | البريد الإلكتروني: info@etag.com", footerFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 40
            };
            document.Add(footer);
        }

        /// <summary>
        /// Add company header to Excel
        /// </summary>
        private static void AddCompanyHeaderExcel(ClosedXML.Excel.IXLWorksheet worksheet)
        {
            worksheet.Cell("A1").Value = "شركة إيتاج للمعدات الصناعية";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 18;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:F1").Merge();

            worksheet.Cell("A2").Value = "معدات هيدروليكية - هوائية - ميكانيكية";
            worksheet.Cell("A2").Style.Font.FontSize = 12;
            worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A2:F2").Merge();
        }

        /// <summary>
        /// Add invoice header to Excel
        /// </summary>
        private static void AddInvoiceHeaderExcel(ClosedXML.Excel.IXLWorksheet worksheet, Invoice invoice)
        {
            worksheet.Cell("A4").Value = "فاتورة مبيعات";
            worksheet.Cell("A4").Style.Font.Bold = true;
            worksheet.Cell("A4").Style.Font.FontSize = 16;
            worksheet.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A4:F4").Merge();

            worksheet.Cell("A5").Value = $"رقم الفاتورة: {invoice.InvoiceNumber} | التاريخ: {invoice.InvoiceDate:yyyy/MM/dd}";
            worksheet.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A5:F5").Merge();
        }

        /// <summary>
        /// Add client information to Excel
        /// </summary>
        private static void AddClientInformationExcel(ClosedXML.Excel.IXLWorksheet worksheet, Invoice invoice)
        {
            worksheet.Cell("A7").Value = "بيانات العميل:";
            worksheet.Cell("A7").Style.Font.Bold = true;
            worksheet.Cell("A7").Style.Font.FontSize = 12;

            worksheet.Cell("A8").Value = $"الاسم: {invoice.ClientName}";
            worksheet.Cell("A9").Value = $"العنوان: {invoice.Client?.Address ?? ""}";
            worksheet.Cell("A10").Value = $"الهاتف: {invoice.Client?.Phone ?? ""}";
        }

        /// <summary>
        /// Add items table to Excel
        /// </summary>
        private static void AddItemsTableExcel(ClosedXML.Excel.IXLWorksheet worksheet, Invoice invoice)
        {
            var startRow = 12;
            var headers = new[] { "م", "اسم الصنف", "الكمية", "سعر الوحدة", "الخصم", "الإجمالي" };

            // Add headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(startRow, i + 1).Value = headers[i];
                worksheet.Cell(startRow, i + 1).Style.Font.Bold = true;
                worksheet.Cell(startRow, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                worksheet.Cell(startRow, i + 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(startRow, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Add data
            for (int i = 0; i < invoice.Items.Count; i++)
            {
                var item = invoice.Items[i];
                var row = startRow + i + 1;
                var total = (item.UnitPrice * item.Quantity) - item.Discount;

                worksheet.Cell(row, 1).Value = i + 1;
                worksheet.Cell(row, 2).Value = item.ItemName;
                worksheet.Cell(row, 3).Value = item.Quantity;
                worksheet.Cell(row, 4).Value = item.UnitPrice;
                worksheet.Cell(row, 5).Value = item.Discount;
                worksheet.Cell(row, 6).Value = total;

                // Format numbers
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                // Alternate row colors
                if (i % 2 == 1)
                {
                    worksheet.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 249, 250);
                }
            }
        }

        /// <summary>
        /// Add totals to Excel
        /// </summary>
        private static void AddTotalsExcel(ClosedXML.Excel.IXLWorksheet worksheet, Invoice invoice)
        {
            var startRow = 12 + invoice.Items.Count + 2;
            var subtotal = invoice.Items.Sum(i => i.UnitPrice * i.Quantity);
            var totalDiscount = invoice.Items.Sum(i => i.Discount);
            var total = subtotal - totalDiscount;

            worksheet.Cell(startRow, 1).Value = "المجموع الفرعي:";
            worksheet.Cell(startRow, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow, 2).Value = subtotal;
            worksheet.Cell(startRow, 2).Style.NumberFormat.Format = "#,##0.00";

            worksheet.Cell(startRow + 1, 1).Value = "الخصم:";
            worksheet.Cell(startRow + 1, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow + 1, 2).Value = totalDiscount;
            worksheet.Cell(startRow + 1, 2).Style.NumberFormat.Format = "#,##0.00";

            worksheet.Cell(startRow + 2, 1).Value = "الإجمالي:";
            worksheet.Cell(startRow + 2, 1).Style.Font.Bold = true;
            worksheet.Cell(startRow + 2, 2).Value = total;
            worksheet.Cell(startRow + 2, 2).Style.NumberFormat.Format = "#,##0.00";
        }

        /// <summary>
        /// Print invoice
        /// </summary>
        public static void PrintInvoice(Invoice invoice)
        {
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"invoice_{invoice.InvoiceNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                GeneratePdfInvoice(invoice, tempPath);

                // Print the PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    Verb = "print",
                    UseShellExecute = true
                });

                // Clean up after a delay
                System.Threading.Tasks.Task.Delay(5000).ContinueWith(_ =>
                {
                    try
                    {
                        if (File.Exists(tempPath))
                        {
                            File.Delete(tempPath);
                        }
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Print Invoice");
            }
        }

        /// <summary>
        /// Save invoice template
        /// </summary>
        public static void SaveInvoiceTemplate(string templateName, string templatePath, string templateType)
        {
            try
            {
                DatabaseHelper.SavePrintTemplate(templateName, templatePath, templateType);
                ErrorHandler.ShowSuccess("تم حفظ قالب الفاتورة بنجاح");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save Invoice Template");
            }
        }

        /// <summary>
        /// Load invoice template
        /// </summary>
        public static string LoadInvoiceTemplate(string templateName)
        {
            try
            {
                var templates = DatabaseHelper.GetPrintTemplates();
                var template = templates.AsEnumerable()
                    .FirstOrDefault(t => t.Field<string>("TemplateName") == templateName);

                return template?.Field<string>("TemplatePath") ?? "";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Load Invoice Template");
                return "";
            }
        }
    }
}