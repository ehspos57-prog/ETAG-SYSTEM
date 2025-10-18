using ETAG_ERP.Models;
using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace ETAG_ERP.Helpers
{
    public static class InvoiceTemplateHelper
    {
        public static FlowDocument GenerateInvoiceTemplate(Invoice invoice, List<InvoiceItem> items)
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new FontFamily("Arial"),
                FontSize = 12
            };

            // Company Header
            AddCompanyHeader(doc);
            
            // Invoice Title
            AddInvoiceTitle(doc, invoice);
            
            // Invoice Details
            AddInvoiceDetails(doc, invoice);
            
            // Items Table
            AddItemsTable(doc, items);
            
            // Totals
            AddTotals(doc, invoice);
            
            // Footer
            AddFooter(doc);

            return doc;
        }

        private static void AddCompanyHeader(FlowDocument doc)
        {
            // Company Name
            var companyName = new Paragraph(new Run("شركة ETAG للمعدات الصناعية"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            doc.Blocks.Add(companyName);

            // Company Details
            var companyDetails = new Paragraph();
            companyDetails.Inlines.Add(new Run("العنوان: شارع الملك فهد، الرياض، المملكة العربية السعودية"));
            companyDetails.Inlines.Add(new LineBreak());
            companyDetails.Inlines.Add(new Run("الهاتف: +966 11 123 4567"));
            companyDetails.Inlines.Add(new LineBreak());
            companyDetails.Inlines.Add(new Run("البريد الإلكتروني: info@etag.com"));
            companyDetails.Inlines.Add(new LineBreak());
            companyDetails.Inlines.Add(new Run("الرقم الضريبي: 123456789"));
            companyDetails.TextAlignment = TextAlignment.Center;
            companyDetails.FontSize = 10;
            companyDetails.Margin = new Thickness(0, 0, 0, 20);
            doc.Blocks.Add(companyDetails);
        }

        private static void AddInvoiceTitle(FlowDocument doc, Invoice invoice)
        {
            var title = new Paragraph(new Run("فاتورة مبيعات"))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(title);
        }

        private static void AddInvoiceDetails(FlowDocument doc, Invoice invoice)
        {
            var detailsTable = new Table
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Add columns
            for (int i = 0; i < 2; i++)
                detailsTable.Columns.Add(new TableColumn());

            // Header row
            var headerRow = new TableRow();
            var headerCell1 = new TableCell(new Paragraph(new Run("تفاصيل الفاتورة")))
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                Background = Brushes.LightGray,
                FontWeight = FontWeights.Bold
            };
            var headerCell2 = new TableCell(new Paragraph(new Run("بيانات العميل")))
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                Background = Brushes.LightGray,
                FontWeight = FontWeights.Bold
            };
            headerRow.Cells.Add(headerCell1);
            headerRow.Cells.Add(headerCell2);

            // Data row
            var dataRow = new TableRow();
            var dataCell1 = new TableCell();
            var dataCell1Content = new Paragraph();
            dataCell1Content.Inlines.Add(new Run($"رقم الفاتورة: {invoice.InvoiceNumber}"));
            dataCell1Content.Inlines.Add(new LineBreak());
            dataCell1Content.Inlines.Add(new Run($"التاريخ: {invoice.Date:yyyy-MM-dd}"));
            dataCell1Content.Inlines.Add(new LineBreak());
            dataCell1Content.Inlines.Add(new Run($"الحالة: {invoice.Status}"));
            dataCell1Content.Inlines.Add(new LineBreak());
            dataCell1Content.Inlines.Add(new Run($"النوع: {invoice.Type}"));
            dataCell1.BorderBrush = Brushes.Black;
            dataCell1.BorderThickness = new Thickness(1);
            dataCell1.Padding = new Thickness(5);
            dataCell1.Blocks.Add(dataCell1Content);

            var dataCell2 = new TableCell();
            var dataCell2Content = new Paragraph();
            dataCell2Content.Inlines.Add(new Run($"العميل: {invoice.ClientName}"));
            dataCell2Content.Inlines.Add(new LineBreak());
            dataCell2Content.Inlines.Add(new Run($"المبلغ الإجمالي: {invoice.TotalAmount:N2} ريال"));
            dataCell2Content.Inlines.Add(new LineBreak());
            dataCell2Content.Inlines.Add(new Run($"المبلغ المدفوع: {invoice.PaidAmount:N2} ريال"));
            dataCell2Content.Inlines.Add(new LineBreak());
            dataCell2Content.Inlines.Add(new Run($"المبلغ المتبقي: {(invoice.TotalAmount - invoice.PaidAmount):N2} ريال"));
            dataCell2.BorderBrush = Brushes.Black;
            dataCell2.BorderThickness = new Thickness(1);
            dataCell2.Padding = new Thickness(5);
            dataCell2.Blocks.Add(dataCell2Content);

            dataRow.Cells.Add(dataCell1);
            dataRow.Cells.Add(dataCell2);

            var rowGroup = new TableRowGroup();
            rowGroup.Rows.Add(headerRow);
            rowGroup.Rows.Add(dataRow);
            detailsTable.RowGroups.Add(rowGroup);

            doc.Blocks.Add(detailsTable);

            // Add spacing
            var spacing = new Paragraph(new Run(""))
            {
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(spacing);
        }

        private static void AddItemsTable(FlowDocument doc, List<InvoiceItem> items)
        {
            var table = new Table
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Add columns
            for (int i = 0; i < 6; i++)
                table.Columns.Add(new TableColumn());

            // Header row
            var headerRow = new TableRow();
            string[] headers = { "رقم", "اسم الصنف", "الكمية", "سعر الوحدة", "الخصم", "المجموع" };
            
            foreach (string header in headers)
            {
                var cell = new TableCell(new Paragraph(new Run(header)))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(5),
                    Background = Brushes.LightGray,
                    FontWeight = FontWeights.Bold
                };
                headerRow.Cells.Add(cell);
            }

            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // Data rows
            var dataGroup = new TableRowGroup();
            int rowNumber = 1;
            foreach (var item in items)
            {
                var row = new TableRow();
                
                string[] values = {
                    rowNumber.ToString(),
                    item.ItemName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("N2"),
                    item.DiscountRate.ToString("N2") + "%",
                    item.Total.ToString("N2")
                };

                foreach (string value in values)
                {
                    var cell = new TableCell(new Paragraph(new Run(value)))
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(5)
                    };
                    row.Cells.Add(cell);
                }

                dataGroup.Rows.Add(row);
                rowNumber++;
            }
            table.RowGroups.Add(dataGroup);

            doc.Blocks.Add(table);
        }

        private static void AddTotals(FlowDocument doc, Invoice invoice)
        {
            var totalsTable = new Table
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 20, 0, 0)
            };

            // Add columns
            for (int i = 0; i < 2; i++)
                totalsTable.Columns.Add(new TableColumn());

            // Total rows
            var totalRows = new List<(string Label, decimal Amount)>
            {
                ("المجموع الفرعي:", invoice.TotalAmount),
                ("الضريبة (14%):", invoice.TotalAmount * 0.14m),
                ("المجموع الإجمالي:", invoice.TotalAmount * 1.14m),
                ("المبلغ المدفوع:", invoice.PaidAmount),
                ("المبلغ المتبقي:", (invoice.TotalAmount * 1.14m) - invoice.PaidAmount)
            };

            var rowGroup = new TableRowGroup();
            foreach (var (label, amount) in totalRows)
            {
                var row = new TableRow();
                
                var labelCell = new TableCell(new Paragraph(new Run(label)))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(5),
                    FontWeight = FontWeights.Bold
                };
                
                var amountCell = new TableCell(new Paragraph(new Run(amount.ToString("N2") + " ريال")))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(5),
                    TextAlignment = TextAlignment.Right
                };

                row.Cells.Add(labelCell);
                row.Cells.Add(amountCell);
                rowGroup.Rows.Add(row);
            }

            totalsTable.RowGroups.Add(rowGroup);
            doc.Blocks.Add(totalsTable);
        }

        private static void AddFooter(FlowDocument doc)
        {
            var footer = new Paragraph(new Run("شكراً لاختياركم شركة ETAG للمعدات الصناعية"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 30, 0, 10)
            };
            doc.Blocks.Add(footer);

            var footerDetails = new Paragraph(new Run("هذه الفاتورة صادرة إلكترونياً ولا تحتاج إلى توقيع"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Center,
                Foreground = Brushes.Gray
            };
            doc.Blocks.Add(footerDetails);
        }

        public static void PrintInvoice(Invoice invoice, List<InvoiceItem> items)
        {
            try
            {
                var document = GenerateInvoiceTemplate(invoice, items);
                var printDialog = new System.Windows.Controls.PrintDialog();
                
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, $"فاتورة {invoice.InvoiceNumber}");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Print Invoice");
            }
        }

        public static void SaveInvoiceAsPDF(Invoice invoice, List<InvoiceItem> items, string filePath)
        {
            try
            {
                var document = GenerateInvoiceTemplate(invoice, items);
                
                // Convert FlowDocument to PDF using iTextSharp
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var pdfWriter = new iTextSharp.text.pdf.PdfWriter(fileStream);
                    var pdfDoc = new iTextSharp.text.Document();
                    pdfWriter.Open();
                    pdfDoc.Open();
                    
                    // Add content to PDF
                    pdfDoc.Add(new iTextSharp.text.Paragraph($"فاتورة مبيعات - {invoice.InvoiceNumber}"));
                    pdfDoc.Add(new iTextSharp.text.Paragraph($"العميل: {invoice.ClientName}"));
                    pdfDoc.Add(new iTextSharp.text.Paragraph($"التاريخ: {invoice.Date:yyyy-MM-dd}"));
                    pdfDoc.Add(new iTextSharp.text.Paragraph($"المبلغ الإجمالي: {invoice.TotalAmount:N2} ريال"));
                    
                    pdfDoc.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save Invoice as PDF");
            }
        }
    }
}
