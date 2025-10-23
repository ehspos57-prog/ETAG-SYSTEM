using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ETAG_ERP.Helpers
{
    public static class ExportHelper
    {
        /// <summary>
        /// Export data to Excel
        /// </summary>
        public static void ExportToExcel<T>(List<T> data, string fileName, string sheetName = "Sheet1")
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = fileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add(sheetName);

                        // Get properties of the type
                        var properties = typeof(T).GetProperties();

                        // Add headers
                        for (int i = 0; i < properties.Length; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = properties[i].Name;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                            worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                        }

                        // Add data
                        for (int i = 0; i < data.Count; i++)
                        {
                            var item = data[i];
                            for (int j = 0; j < properties.Length; j++)
                            {
                                var value = properties[j].GetValue(item);
                                worksheet.Cell(i + 2, j + 1).Value = value?.ToString() ?? "";

                                // Format numbers
                                if (value is decimal || value is double || value is float)
                                {
                                    worksheet.Cell(i + 2, j + 1).Style.NumberFormat.Format = "#,##0.00";
                                }
                            }

                            // Alternate row colors
                            if (i % 2 == 1)
                            {
                                worksheet.Range(i + 2, 1, i + 2, properties.Length).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 249, 250);
                            }
                        }

                        // Auto-fit columns
                        worksheet.Columns().AdjustToContents();

                        // Save workbook
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    ErrorHandler.ShowSuccess("تم تصدير البيانات إلى Excel بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export to Excel");
            }
        }

        /// <summary>
        /// Export data to PDF
        /// </summary>
        public static void ExportToPdf<T>(List<T> data, string fileName, string title)
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = fileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20))
                    {
                        using (var writer = Models.PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create)))
                        {
                            document.Open();

                            // Add title
                            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                            var titleParagraph = new Paragraph(title, titleFont)
                            {
                                Alignment = Element.ALIGN_CENTER,
                                SpacingAfter = 20
                            };
                            document.Add(titleParagraph);

                            // Add date
                            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                            var date = new Paragraph($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}", dateFont)
                            {
                                Alignment = Element.ALIGN_RIGHT,
                                SpacingAfter = 20
                            };
                            document.Add(date);

                            // Create table
                            var properties = typeof(T).GetProperties();
                            var table = new PdfPTable(properties.Length)
                            {
                                WidthPercentage = 100,
                                SpacingBefore = 10,
                                SpacingAfter = 10
                            };

                            // Add headers
                            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                            var headerBackground = new BaseColor(52, 152, 219);

                            foreach (var property in properties)
                            {
                                var cell = new PdfPCell(new Phrase(property.Name, headerFont))
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

                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                var isEven = i % 2 == 0;

                                foreach (var property in properties)
                                {
                                    var value = property.GetValue(item);
                                    var cell = new PdfPCell(new Phrase(value?.ToString() ?? "", dataFont))
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
                            var summary = new Paragraph($"إجمالي السجلات: {data.Count}", summaryFont)
                            {
                                Alignment = Element.ALIGN_CENTER,
                                SpacingBefore = 20
                            };
                            document.Add(summary);
                        }
                    }

                    ErrorHandler.ShowSuccess("تم تصدير البيانات إلى PDF بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export to PDF");
            }
        }

        /// <summary>
        /// Export items to Excel
        /// </summary>
        public static void ExportItemsToExcel()
        {
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems();
                ExportToExcel(items, $"الأصناف_{DateTime.Now:yyyyMMdd}.xlsx", "الأصناف");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Items to Excel");
            }
        }

        /// <summary>
        /// Export items to PDF
        /// </summary>
        public static void ExportItemsToPdf()
        {
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems();
                ExportToPdf(items, $"الأصناف_{DateTime.Now:yyyyMMdd}.pdf", "تقرير الأصناف");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Items to PDF");
            }
        }

        /// <summary>
        /// Export clients to Excel
        /// </summary>
        public static void ExportClientsToExcel()
        {
            try
            {
                var clients = DatabaseHelper_Extensions.GetAllClients();
                ExportToExcel(clients, $"العملاء_{DateTime.Now:yyyyMMdd}.xlsx", "العملاء");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Clients to Excel");
            }
        }

        /// <summary>
        /// Export clients to PDF
        /// </summary>
        public static void ExportClientsToPdf()
        {
            try
            {
                var clients = DatabaseHelper_Extensions.GetAllClients();
                ExportToPdf(clients, $"العملاء_{DateTime.Now:yyyyMMdd}.pdf", "تقرير العملاء");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Clients to PDF");
            }
        }

        /// <summary>
        /// Export invoices to Excel
        /// </summary>
        public static void ExportInvoicesToExcel()
        {
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                ExportToExcel(invoices, $"الفواتير_{DateTime.Now:yyyyMMdd}.xlsx", "الفواتير");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Invoices to Excel");
            }
        }

        /// <summary>
        /// Export invoices to PDF
        /// </summary>
        public static void ExportInvoicesToPdf()
        {
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                ExportToPdf(invoices, $"الفواتير_{DateTime.Now:yyyyMMdd}.pdf", "تقرير الفواتير");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Invoices to PDF");
            }
        }

        /// <summary>
        /// Export purchases to Excel
        /// </summary>
        public static void ExportPurchasesToExcel()
        {
            try
            {
                var purchases = DatabaseHelper_Extensions.GetAllPurchases();
                ExportToExcel(purchases, $"المشتريات_{DateTime.Now:yyyyMMdd}.xlsx", "المشتريات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Purchases to Excel");
            }
        }

        /// <summary>
        /// Export purchases to PDF
        /// </summary>
        public static void ExportPurchasesToPdf()
        {
            try
            {
                var purchases = DatabaseHelper_Extensions.GetAllPurchases();
                ExportToPdf(purchases, $"المشتريات_{DateTime.Now:yyyyMMdd}.pdf", "تقرير المشتريات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Purchases to PDF");
            }
        }

        /// <summary>
        /// Export expenses to Excel
        /// </summary>
        public static void ExportExpensesToExcel()
        {
            try
            {
                var expenses = DatabaseHelper_Extensions.GetAllExpenses();
                ExportToExcel(expenses, $"المصروفات_{DateTime.Now:yyyyMMdd}.xlsx", "المصروفات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Expenses to Excel");
            }
        }

        /// <summary>
        /// Export expenses to PDF
        /// </summary>
        public static void ExportExpensesToPdf()
        {
            try
            {
                var expenses = DatabaseHelper_Extensions.GetAllExpenses();
                ExportToPdf(expenses, $"المصروفات_{DateTime.Now:yyyyMMdd}.pdf", "تقرير المصروفات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Expenses to PDF");
            }
        }

        /// <summary>
        /// Export sales report to Excel
        /// </summary>
        public static void ExportSalesReportToExcel()
        {
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                var salesData = invoices.Select(i => new
                {
                    رقم_الفاتورة = i.InvoiceNumber,
                    اسم_العميل = i.ClientName,
                    التاريخ = i.InvoiceDate.ToString("yyyy/MM/dd"),
                    المبلغ_الإجمالي = i.TotalAmount,
                    المبلغ_المدفوع = i.PaidAmount,
                    المبلغ_المتبقي = i.TotalAmount - i.PaidAmount,
                    الحالة = i.Status
                }).ToList();

                ExportToExcel(salesData, $"تقرير_المبيعات_{DateTime.Now:yyyyMMdd}.xlsx", "تقرير المبيعات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Sales Report to Excel");
            }
        }

        /// <summary>
        /// Export sales report to PDF
        /// </summary>
        public static void ExportSalesReportToPdf()
        {
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                var salesData = invoices.Select(i => new
                {
                    رقم_الفاتورة = i.InvoiceNumber,
                    اسم_العميل = i.ClientName,
                    التاريخ = i.InvoiceDate.ToString("yyyy/MM/dd"),
                    المبلغ_الإجمالي = i.TotalAmount,
                    المبلغ_المدفوع = i.PaidAmount,
                    المبلغ_المتبقي = i.TotalAmount - i.PaidAmount,
                    الحالة = i.Status
                }).ToList();

                ExportToPdf(salesData, $"تقرير_المبيعات_{DateTime.Now:yyyyMMdd}.pdf", "تقرير المبيعات");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Sales Report to PDF");
            }
        }

        /// <summary>
        /// Export financial summary to Excel
        /// </summary>
        public static void ExportFinancialSummaryToExcel()
        {
            try
            {
                var totalSales = DatabaseHelper_Extensions.GetTotalSales();
                var totalPurchases = DatabaseHelper_Extensions.GetTotalPurchases();
                var totalExpenses = DatabaseHelper_Extensions.GetTotalExpenses();
                var netProfit = totalSales - totalPurchases - totalExpenses;

                var financialData = new List<object>
                {
                    new { البند = "إجمالي المبيعات", المبلغ = totalSales },
                    new { البند = "إجمالي المشتريات", المبلغ = totalPurchases },
                    new { البند = "إجمالي المصروفات", المبلغ = totalExpenses },
                    new { البند = "صافي الربح", المبلغ = netProfit }
                };

                ExportToExcel(financialData, $"الملخص_المالي_{DateTime.Now:yyyyMMdd}.xlsx", "الملخص المالي");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Financial Summary to Excel");
            }
        }

        /// <summary>
        /// Export financial summary to PDF
        /// </summary>
        public static void ExportFinancialSummaryToPdf()
        {
            try
            {
                var totalSales = DatabaseHelper_Extensions.GetTotalSales();
                var totalPurchases = DatabaseHelper_Extensions.GetTotalPurchases();
                var totalExpenses = DatabaseHelper_Extensions.GetTotalExpenses();
                var netProfit = totalSales - totalPurchases - totalExpenses;

                var financialData = new List<object>
                {
                    new { البند = "إجمالي المبيعات", المبلغ = totalSales },
                    new { البند = "إجمالي المشتريات", المبلغ = totalPurchases },
                    new { البند = "إجمالي المصروفات", المبلغ = totalExpenses },
                    new { البند = "صافي الربح", المبلغ = netProfit }
                };

                ExportToPdf(financialData, $"الملخص_المالي_{DateTime.Now:yyyyMMdd}.pdf", "الملخص المالي");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Financial Summary to PDF");
            }
        }
    }
}