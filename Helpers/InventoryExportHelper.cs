using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;

namespace ETAG_ERP.Helpers
{
    public static class InventoryExportHelper
    {
        /// <summary>
        /// Export inventory to PDF
        /// </summary>
        public static void ExportInventoryToPdf()
        {
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems();
                var categories = DatabaseHelper_Extensions.GetAllCategories();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"جرد_المخزون_{DateTime.Now:yyyyMMdd}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    CreateInventoryPdf(items, categories, saveFileDialog.FileName);
                    ErrorHandler.ShowSuccess("تم تصدير جرد المخزون إلى PDF بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Inventory to PDF");
            }
        }

        /// <summary>
        /// Export inventory to Excel
        /// </summary>
        public static void ExportInventoryToExcel()
        {
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems();
                var categories = DatabaseHelper_Extensions.GetAllCategories();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"جرد_المخزون_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    CreateInventoryExcel(items, categories, saveFileDialog.FileName);
                    ErrorHandler.ShowSuccess("تم تصدير جرد المخزون إلى Excel بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Inventory to Excel");
            }
        }

        /// <summary>
        /// Create PDF document for inventory
        /// </summary>
        private static void CreateInventoryPdf(List<Item> items, List<Category> categories, string filePath)
        {
            using (var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20))
            {
                using (var writer = Models.PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                {
                    document.Open();

                    // Add title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                    var title = new Paragraph("جرد المخزون", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    document.Add(title);

                    // Add date
                    var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                    var date = new Paragraph($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}", dateFont)
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingAfter = 20
                    };
                    document.Add(date);

                    // Create table
                    var table = new PdfPTable(8)
                    {
                        WidthPercentage = 100,
                        SpacingBefore = 10,
                        SpacingAfter = 10
                    };

                    // Set column widths
                    table.SetWidths(new float[] { 1, 2, 2, 1, 1, 1, 1, 1 });

                    // Add headers
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    var headerBackground = new BaseColor(52, 152, 219);

                    var headers = new[] { "الكود", "اسم الصنف", "الوصف", "الكمية", "سعر البيع", "سعر الشراء", "الحد الأدنى", "الحالة" };

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

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        var isEven = i % 2 == 0;

                        var cells = new[]
                        {
                            item.Code ?? "",
                            item.ItemName ?? "",
                            item.Description ?? "",
                            item.Quantity.ToString(),
                            item.SellingPrice.ToString("N2"),
                            item.PurchasePrice.ToString("N2"),
                            item.MinStock.ToString(),
                            item.Quantity <= item.MinStock ? "منخفض" : "عادي"
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

                    // Add summary
                    var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                    var totalItems = items.Count;
                    var lowStockItems = items.Count(i => i.Quantity <= i.MinStock);
                    var totalValue = items.Sum(i => i.Quantity * i.SellingPrice);

                    var summary = new Paragraph($"إجمالي الأصناف: {totalItems} | أصناف منخفضة المخزون: {lowStockItems} | إجمالي القيمة: {totalValue:N2} جنيه", summaryFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20
                    };
                    document.Add(summary);
                }
            }
        }

        /// <summary>
        /// Create Excel document for inventory
        /// </summary>
        private static void CreateInventoryExcel(List<Item> items, List<Category> categories, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("جرد المخزون");

                // Add title
                worksheet.Cell("A1").Value = "جرد المخزون";
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Cell("A1").Style.Font.FontSize = 16;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:H1").Merge();

                // Add date
                worksheet.Cell("A2").Value = $"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}";
                worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range("A2:H2").Merge();

                // Add headers
                var headers = new[] { "الكود", "اسم الصنف", "الوصف", "الكمية", "سعر البيع", "سعر الشراء", "الحد الأدنى", "الحالة" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(4, i + 1).Value = headers[i];
                    worksheet.Cell(4, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                    worksheet.Cell(4, i + 1).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(4, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Add data
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var row = i + 5;

                    worksheet.Cell(row, 1).Value = item.Code ?? "";
                    worksheet.Cell(row, 2).Value = item.ItemName ?? "";
                    worksheet.Cell(row, 3).Value = item.Description ?? "";
                    worksheet.Cell(row, 4).Value = item.Quantity;
                    worksheet.Cell(row, 5).Value = item.SellingPrice;
                    worksheet.Cell(row, 6).Value = item.PurchasePrice;
                    worksheet.Cell(row, 7).Value = item.MinStock;
                    worksheet.Cell(row, 8).Value = item.Quantity <= item.MinStock ? "منخفض" : "عادي";

                    // Format numbers
                    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

                    // Alternate row colors
                    if (i % 2 == 1)
                    {
                        worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 249, 250);
                    }
                }

                // Add summary
                var summaryRow = items.Count + 7;
                var totalItems = items.Count;
                var lowStockItems = items.Count(i => i.Quantity <= i.MinStock);
                var totalValue = items.Sum(i => i.Quantity * i.SellingPrice);

                worksheet.Cell(summaryRow, 1).Value = $"إجمالي الأصناف: {totalItems}";
                worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
                worksheet.Cell(summaryRow, 3).Value = $"أصناف منخفضة المخزون: {lowStockItems}";
                worksheet.Cell(summaryRow, 3).Style.Font.Bold = true;
                worksheet.Cell(summaryRow, 5).Value = $"إجمالي القيمة: {totalValue:N2} جنيه";
                worksheet.Cell(summaryRow, 5).Style.Font.Bold = true;
                worksheet.Cell(summaryRow, 5).Style.NumberFormat.Format = "#,##0.00";

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save workbook
                workbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Export low stock items to PDF
        /// </summary>
        public static void ExportLowStockItemsToPdf()
        {
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems()
                    .Where(i => i.Quantity <= i.MinStock)
                    .ToList();

                if (!items.Any())
                {
                    ErrorHandler.ShowInfo("لا توجد أصناف منخفضة المخزون");
                    return;
                }

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"الأصناف_منخفضة_المخزون_{DateTime.Now:yyyyMMdd}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    CreateLowStockPdf(items, saveFileDialog.FileName);
                    ErrorHandler.ShowSuccess("تم تصدير الأصناف منخفضة المخزون إلى PDF بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Low Stock Items to PDF");
            }
        }

        /// <summary>
        /// Create PDF document for low stock items
        /// </summary>
        private static void CreateLowStockPdf(List<Item> items, string filePath)
        {
            using (var document = new Document(PageSize.A4, 20, 20, 20, 20))
            {
                using (var writer = Models.PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                {
                    document.Open();

                    // Add title
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.RED);
                    var title = new Paragraph("الأصناف منخفضة المخزون", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingAfter = 20
                    };
                    document.Add(title);

                    // Add date
                    var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                    var date = new Paragraph($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}", dateFont)
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingAfter = 20
                    };
                    document.Add(date);

                    // Create table
                    var table = new PdfPTable(6)
                    {
                        WidthPercentage = 100,
                        SpacingBefore = 10,
                        SpacingAfter = 10
                    };

                    // Set column widths
                    table.SetWidths(new float[] { 1, 2, 1, 1, 1, 1 });

                    // Add headers
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    var headerBackground = new BaseColor(231, 76, 60);

                    var headers = new[] { "الكود", "اسم الصنف", "الكمية الحالية", "الحد الأدنى", "النقص", "الحالة" };

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

                    foreach (var item in items)
                    {
                        var shortage = item.MinStock - item.Quantity;
                        var status = shortage > 0 ? "ناقص" : "عند الحد الأدنى";

                        var cells = new[]
                        {
                            item.Code ?? "",
                            item.ItemName ?? "",
                            item.Quantity.ToString(),
                            item.MinStock.ToString(),
                            shortage.ToString(),
                            status
                        };

                        foreach (var cellText in cells)
                        {
                            var cell = new PdfPCell(new Phrase(cellText, dataFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 6
                            };
                            table.AddCell(cell);
                        }
                    }

                    document.Add(table);

                    // Add summary
                    var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.RED);
                    var totalItems = items.Count;
                    var totalShortage = items.Sum(i => Math.Max(0, i.MinStock - i.Quantity));

                    var summary = new Paragraph($"إجمالي الأصناف منخفضة المخزون: {totalItems} | إجمالي النقص: {totalShortage} وحدة", summaryFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 20
                    };
                    document.Add(summary);
                }
            }
        }
    }
}