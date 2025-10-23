using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using ExcelDataReader;

namespace ETAG_ERP.Helpers
{
    public static class ExcelImportHelper
    {
        /// <summary>
        /// Import items from Excel file
        /// </summary>
        public static ImportResult ImportItemsFromExcel(string filePath)
        {
            var result = new ImportResult();
            
            try
            {
                var items = ReadItemsFromExcel(filePath);
                
                foreach (var item in items)
                {
                    var validationResult = ValidationHelper.ValidateItem(item);
                    if (validationResult.IsValid)
                    {
                        if (DatabaseHelper_Extensions.InsertItem(item))
                        {
                            result.SuccessCount++;
                        }
                        else
                        {
                            result.Errors.Add($"فشل في إدراج الصنف: {item.ItemName}");
                        }
                    }
                    else
                    {
                        result.Errors.AddRange(validationResult.Errors);
                    }
                }
                
                result.TotalCount = items.Count;
                result.IsSuccess = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"خطأ في استيراد الملف: {ex.Message}");
                result.IsSuccess = false;
            }
            
            return result;
        }

        /// <summary>
        /// Import clients from Excel file
        /// </summary>
        public static ImportResult ImportClientsFromExcel(string filePath)
        {
            var result = new ImportResult();
            
            try
            {
                var clients = ReadClientsFromExcel(filePath);
                
                foreach (var client in clients)
                {
                    var validationResult = ValidationHelper.ValidateClient(client);
                    if (validationResult.IsValid)
                    {
                        DatabaseHelper_Extensions.InsertClient(client);
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.Errors.AddRange(validationResult.Errors);
                    }
                }
                
                result.TotalCount = clients.Count;
                result.IsSuccess = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"خطأ في استيراد الملف: {ex.Message}");
                result.IsSuccess = false;
            }
            
            return result;
        }

        /// <summary>
        /// Import categories from Excel file
        /// </summary>
        public static ImportResult ImportCategoriesFromExcel(string filePath)
        {
            var result = new ImportResult();
            
            try
            {
                var categories = ReadCategoriesFromExcel(filePath);
                
                foreach (var category in categories)
                {
                    var validationResult = ValidationHelper.ValidateCategory(category);
                    if (validationResult.IsValid)
                    {
                        DatabaseHelper_Extensions.InsertCategory(category);
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.Errors.AddRange(validationResult.Errors);
                    }
                }
                
                result.TotalCount = categories.Count;
                result.IsSuccess = result.Errors.Count == 0;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"خطأ في استيراد الملف: {ex.Message}");
                result.IsSuccess = false;
            }
            
            return result;
        }

        /// <summary>
        /// Read items from Excel file
        /// </summary>
        private static List<Item> ReadItemsFromExcel(string filePath)
        {
            var items = new List<Item>();
            
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().Skip(1); // Skip header row
                    
                    foreach (var row in rows)
                    {
                        try
                        {
                            var item = new Item
                            {
                                ItemName = row.Cell(1).GetString(),
                                Code = row.Cell(2).GetString(),
                                Description = row.Cell(3).GetString(),
                                Quantity = Convert.ToInt32(row.Cell(4).GetDouble()),
                                SellingPrice = Convert.ToDecimal(row.Cell(5).GetDouble()),
                                PurchasePrice = Convert.ToDecimal(row.Cell(6).GetDouble()),
                                MinStock = Convert.ToInt32(row.Cell(7).GetDouble()),
                                Unit = row.Cell(8).GetString(),
                                Barcode = row.Cell(9).GetString(),
                                Tax = Convert.ToDecimal(row.Cell(10).GetDouble()),
                                Discount = Convert.ToDecimal(row.Cell(11).GetDouble()),
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            
                            items.Add(item);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.LogWarning($"خطأ في قراءة صف {row.RowNumber()}: {ex.Message}", "Excel Import");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Read Items From Excel");
            }
            
            return items;
        }

        /// <summary>
        /// Read clients from Excel file
        /// </summary>
        private static List<Client> ReadClientsFromExcel(string filePath)
        {
            var clients = new List<Client>();
            
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().Skip(1); // Skip header row
                    
                    foreach (var row in rows)
                    {
                        try
                        {
                            var client = new Client
                            {
                                Name = row.Cell(1).GetString(),
                                Phone = row.Cell(2).GetString(),
                                Email = row.Cell(3).GetString(),
                                Address = row.Cell(4).GetString(),
                                Notes = row.Cell(5).GetString(),
                                Balance = Convert.ToDecimal(row.Cell(6).GetDouble()),
                                TaxNumber = row.Cell(7).GetString(),
                                CommercialRecord = row.Cell(8).GetString(),
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            
                            clients.Add(client);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.LogWarning($"خطأ في قراءة صف {row.RowNumber()}: {ex.Message}", "Excel Import");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Read Clients From Excel");
            }
            
            return clients;
        }

        /// <summary>
        /// Read categories from Excel file
        /// </summary>
        private static List<Category> ReadCategoriesFromExcel(string filePath)
        {
            var categories = new List<Category>();
            
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    var rows = worksheet.RowsUsed().Skip(1); // Skip header row
                    
                    foreach (var row in rows)
                    {
                        try
                        {
                            var category = new Category
                            {
                                Name = row.Cell(1).GetString(),
                                Type = row.Cell(2).GetString(),
                                Description = row.Cell(3).GetString(),
                                Level1 = row.Cell(4).GetString(),
                                Level2 = row.Cell(5).GetString(),
                                Level3 = row.Cell(6).GetString(),
                                Level4 = row.Cell(7).GetString(),
                                Level5 = row.Cell(8).GetString(),
                                Code = row.Cell(9).GetString(),
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            
                            categories.Add(category);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.LogWarning($"خطأ في قراءة صف {row.RowNumber()}: {ex.Message}", "Excel Import");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Read Categories From Excel");
            }
            
            return categories;
        }

        /// <summary>
        /// Create Excel template for items
        /// </summary>
        public static void CreateItemsTemplate(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("الأصناف");
                    
                    // Add headers
                    var headers = new[] { "اسم الصنف", "الكود", "الوصف", "الكمية", "سعر البيع", "سعر الشراء", "الحد الأدنى", "الوحدة", "الباركود", "الضريبة", "الخصم" };
                    
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                        worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                    }
                    
                    // Add sample data
                    worksheet.Cell(2, 1).Value = "مضخة هيدروليكية";
                    worksheet.Cell(2, 2).Value = "HYD001";
                    worksheet.Cell(2, 3).Value = "مضخة هيدروليكية عالية الضغط";
                    worksheet.Cell(2, 4).Value = 10;
                    worksheet.Cell(2, 5).Value = 2500.00;
                    worksheet.Cell(2, 6).Value = 1800.00;
                    worksheet.Cell(2, 7).Value = 5;
                    worksheet.Cell(2, 8).Value = "قطعة";
                    worksheet.Cell(2, 9).Value = "1234567890123";
                    worksheet.Cell(2, 10).Value = 14.00;
                    worksheet.Cell(2, 11).Value = 5.00;
                    
                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                    
                    // Save workbook
                    workbook.SaveAs(filePath);
                }
                
                ErrorHandler.ShowSuccess("تم إنشاء قالب الأصناف بنجاح");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Items Template");
            }
        }

        /// <summary>
        /// Create Excel template for clients
        /// </summary>
        public static void CreateClientsTemplate(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("العملاء");
                    
                    // Add headers
                    var headers = new[] { "الاسم", "الهاتف", "البريد الإلكتروني", "العنوان", "ملاحظات", "الرصيد", "الرقم الضريبي", "السجل التجاري" };
                    
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                        worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                    }
                    
                    // Add sample data
                    worksheet.Cell(2, 1).Value = "شركة البناء الحديث";
                    worksheet.Cell(2, 2).Value = "01234567890";
                    worksheet.Cell(2, 3).Value = "info@modernconstruction.com";
                    worksheet.Cell(2, 4).Value = "شارع الملك فهد، الرياض";
                    worksheet.Cell(2, 5).Value = "عميل مميز";
                    worksheet.Cell(2, 6).Value = 15000.00;
                    worksheet.Cell(2, 7).Value = "123456789";
                    worksheet.Cell(2, 8).Value = "CR123456";
                    
                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                    
                    // Save workbook
                    workbook.SaveAs(filePath);
                }
                
                ErrorHandler.ShowSuccess("تم إنشاء قالب العملاء بنجاح");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Clients Template");
            }
        }

        /// <summary>
        /// Create Excel template for categories
        /// </summary>
        public static void CreateCategoriesTemplate(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("التصنيفات");
                    
                    // Add headers
                    var headers = new[] { "الاسم", "النوع", "الوصف", "المستوى 1", "المستوى 2", "المستوى 3", "المستوى 4", "المستوى 5", "الكود" };
                    
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 152, 219);
                        worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                    }
                    
                    // Add sample data
                    worksheet.Cell(2, 1).Value = "معدات هيدروليكية";
                    worksheet.Cell(2, 2).Value = "Hydraulic";
                    worksheet.Cell(2, 3).Value = "معدات الهيدروليك";
                    worksheet.Cell(2, 4).Value = "معدات";
                    worksheet.Cell(2, 5).Value = "هيدروليك";
                    worksheet.Cell(2, 6).Value = "";
                    worksheet.Cell(2, 7).Value = "";
                    worksheet.Cell(2, 8).Value = "";
                    worksheet.Cell(2, 9).Value = "HYD001";
                    
                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();
                    
                    // Save workbook
                    workbook.SaveAs(filePath);
                }
                
                ErrorHandler.ShowSuccess("تم إنشاء قالب التصنيفات بنجاح");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Categories Template");
            }
        }

        /// <summary>
        /// Validate Excel file format
        /// </summary>
        public static bool ValidateExcelFile(string filePath, string expectedSheetName = null)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    ErrorHandler.ShowWarning("الملف غير موجود");
                    return false;
                }
                
                var extension = Path.GetExtension(filePath).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    ErrorHandler.ShowWarning("نوع الملف غير مدعوم. يرجى استخدام ملف Excel (.xlsx أو .xls)");
                    return false;
                }
                
                using (var workbook = new XLWorkbook(filePath))
                {
                    if (expectedSheetName != null)
                    {
                        if (!workbook.Worksheets.Any(ws => ws.Name == expectedSheetName))
                        {
                            ErrorHandler.ShowWarning($"ورقة العمل '{expectedSheetName}' غير موجودة في الملف");
                            return false;
                        }
                    }
                    
                    var worksheet = workbook.Worksheets.First();
                    if (worksheet.RowsUsed().Count() < 2)
                    {
                        ErrorHandler.ShowWarning("الملف لا يحتوي على بيانات كافية");
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Validate Excel File");
                return false;
            }
        }

        /// <summary>
        /// Get Excel file information
        /// </summary>
        public static ExcelFileInfo GetExcelFileInfo(string filePath)
        {
            var info = new ExcelFileInfo();
            
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    info.SheetCount = workbook.Worksheets.Count;
                    info.SheetNames = workbook.Worksheets.Select(ws => ws.Name).ToList();
                    
                    var firstSheet = workbook.Worksheets.First();
                    info.RowCount = firstSheet.RowsUsed().Count();
                    info.ColumnCount = firstSheet.ColumnsUsed().Count();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Excel File Info");
            }
            
            return info;
        }

        /// <summary>
        /// Show import result dialog
        /// </summary>
        public static void ShowImportResult(ImportResult result)
        {
            var message = $"تم استيراد {result.SuccessCount} من أصل {result.TotalCount} سجل بنجاح";
            
            if (result.Errors.Count > 0)
            {
                message += $"\n\nالأخطاء:\n{string.Join("\n", result.Errors.Take(10))}";
                if (result.Errors.Count > 10)
                {
                    message += $"\n... و {result.Errors.Count - 10} خطأ آخر";
                }
            }
            
            if (result.IsSuccess)
            {
                ErrorHandler.ShowSuccess(message);
            }
            else
            {
                ErrorHandler.ShowWarning(message);
            }
        }
    }

    /// <summary>
    /// Import result class
    /// </summary>
    public class ImportResult
    {
        public bool IsSuccess { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Excel file info class
    /// </summary>
    public class ExcelFileInfo
    {
        public int SheetCount { get; set; }
        public List<string> SheetNames { get; set; } = new List<string>();
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }
}
