using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Tests
{
    public static class UnitTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== بدء تشغيل اختبارات الوحدة ===");
            
            var testResults = new List<TestResult>();
            
            // اختبارات قاعدة البيانات
            testResults.Add(TestDatabaseConnection());
            testResults.Add(TestDatabaseInitialization());
            testResults.Add(TestCategoryOperations());
            testResults.Add(TestItemOperations());
            testResults.Add(TestClientOperations());
            testResults.Add(TestInvoiceOperations());
            
            // اختبارات التحقق من صحة البيانات
            testResults.Add(TestValidationHelper());
            
            // اختبارات نظام الأخطاء
            testResults.Add(TestErrorHandler());
            
            // اختبارات نظام الثيمات
            testResults.Add(TestThemeManager());
            
            // اختبارات نظام الصلاحيات
            testResults.Add(TestPermissionManager());
            
            // عرض النتائج
            DisplayTestResults(testResults);
        }

        private static TestResult TestDatabaseConnection()
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar("SELECT 1");
                return new TestResult("Database Connection", true, "تم الاتصال بقاعدة البيانات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Database Connection", false, $"فشل الاتصال بقاعدة البيانات: {ex.Message}");
            }
        }

        private static TestResult TestDatabaseInitialization()
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
                return new TestResult("Database Initialization", true, "تم تهيئة قاعدة البيانات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Database Initialization", false, $"فشل تهيئة قاعدة البيانات: {ex.Message}");
            }
        }

        private static TestResult TestCategoryOperations()
        {
            try
            {
                // اختبار إضافة تصنيف
                var category = new Category
                {
                    Name = "Test Category",
                    Type = "Test",
                    Description = "Test Description",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                DatabaseHelper.InsertCategory(category);
                
                // اختبار جلب التصنيفات
                var categories = DatabaseHelper.GetAllCategories();
                
                // اختبار تحديث التصنيف
                category.Name = "Updated Test Category";
                DatabaseHelper.UpdateCategory(category);
                
                // اختبار حذف التصنيف
                DatabaseHelper.DeleteCategory(category.Id);
                
                return new TestResult("Category Operations", true, "تم اختبار عمليات التصنيفات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Category Operations", false, $"فشل اختبار عمليات التصنيفات: {ex.Message}");
            }
        }

        private static TestResult TestItemOperations()
        {
            try
            {
                // اختبار إضافة صنف
                var item = new Item
                {
                    ItemName = "Test Item",
                    ItemCode = "TEST001",
                    SellingPrice1 = 100,
                    StockQuantity = 50,
                    Unit = "قطعة",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                DatabaseHelper.InsertItem(item);
                
                // اختبار جلب الأصناف
                var items = DatabaseHelper.GetAllItems();
                
                // اختبار تحديث الصنف
                item.ItemName = "Updated Test Item";
                DatabaseHelper.UpdateItem(item);
                
                // اختبار حذف الصنف
                DatabaseHelper.DeleteItem(item.Id);
                
                return new TestResult("Item Operations", true, "تم اختبار عمليات الأصناف بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Item Operations", false, $"فشل اختبار عمليات الأصناف: {ex.Message}");
            }
        }

        private static TestResult TestClientOperations()
        {
            try
            {
                // اختبار إضافة عميل
                var client = new Client
                {
                    Name = "Test Client",
                    Phone = "1234567890",
                    Address = "Test Address",
                    Balance = 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                DatabaseHelper.InsertClient(client);
                
                // اختبار جلب العملاء
                var clients = DatabaseHelper.GetAllClients();
                
                // اختبار تحديث العميل
                client.Name = "Updated Test Client";
                DatabaseHelper.UpdateClient(client);
                
                // اختبار حذف العميل
                DatabaseHelper.DeleteClient(client.Id);
                
                return new TestResult("Client Operations", true, "تم اختبار عمليات العملاء بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Client Operations", false, $"فشل اختبار عمليات العملاء: {ex.Message}");
            }
        }

        private static TestResult TestInvoiceOperations()
        {
            try
            {
                // اختبار إضافة فاتورة
                var invoice = new Invoice
                {
                    InvoiceNumber = "TEST001",
                    ClientName = "Test Client",
                    Date = DateTime.Now,
                    TotalAmount = 1000,
                    PaidAmount = 500,
                    Status = "Pending",
                    Type = "Sale",
                    Notes = "Test Invoice",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                DatabaseHelper.InsertInvoice(invoice);
                
                // اختبار جلب الفواتير
                var invoices = DatabaseHelper.GetAllInvoices();
                
                // اختبار تحديث الفاتورة
                invoice.Status = "Paid";
                DatabaseHelper.UpdateInvoice(invoice);
                
                // اختبار حذف الفاتورة
                DatabaseHelper.DeleteInvoice(invoice.Id);
                
                return new TestResult("Invoice Operations", true, "تم اختبار عمليات الفواتير بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Invoice Operations", false, $"فشل اختبار عمليات الفواتير: {ex.Message}");
            }
        }

        private static TestResult TestValidationHelper()
        {
            try
            {
                // اختبار التحقق من البريد الإلكتروني
                var validEmail = ValidationHelper.ValidateEmail("test@example.com");
                var invalidEmail = ValidationHelper.ValidateEmail("invalid-email");
                
                if (!validEmail || invalidEmail)
                {
                    return new TestResult("Validation Helper", false, "فشل اختبار التحقق من البريد الإلكتروني");
                }
                
                // اختبار التحقق من رقم الهاتف
                var validPhone = ValidationHelper.ValidatePhone("1234567890");
                var invalidPhone = ValidationHelper.ValidatePhone("abc");
                
                if (!validPhone || invalidPhone)
                {
                    return new TestResult("Validation Helper", false, "فشل اختبار التحقق من رقم الهاتف");
                }
                
                // اختبار التحقق من البيانات المطلوبة
                var validRequired = ValidationHelper.ValidateRequired("test");
                var invalidRequired = ValidationHelper.ValidateRequired("");
                
                if (!validRequired || invalidRequired)
                {
                    return new TestResult("Validation Helper", false, "فشل اختبار التحقق من البيانات المطلوبة");
                }
                
                return new TestResult("Validation Helper", true, "تم اختبار نظام التحقق بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Validation Helper", false, $"فشل اختبار نظام التحقق: {ex.Message}");
            }
        }

        private static TestResult TestErrorHandler()
        {
            try
            {
                // اختبار معالجة الأخطاء
                try
                {
                    throw new Exception("Test Exception");
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleException(ex, "Test Context");
                }
                
                return new TestResult("Error Handler", true, "تم اختبار نظام معالجة الأخطاء بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Error Handler", false, $"فشل اختبار نظام معالجة الأخطاء: {ex.Message}");
            }
        }

        private static TestResult TestThemeManager()
        {
            try
            {
                // اختبار تغيير الثيم
                var originalTheme = ThemeManager.CurrentTheme;
                
                ThemeManager.SetTheme(ThemeManager.Theme.Dark);
                if (ThemeManager.CurrentTheme != ThemeManager.Theme.Dark)
                {
                    return new TestResult("Theme Manager", false, "فشل في تغيير الثيم إلى الداكن");
                }
                
                ThemeManager.SetTheme(ThemeManager.Theme.Light);
                if (ThemeManager.CurrentTheme != ThemeManager.Theme.Light)
                {
                    return new TestResult("Theme Manager", false, "فشل في تغيير الثيم إلى الفاتح");
                }
                
                // استعادة الثيم الأصلي
                ThemeManager.SetTheme(originalTheme);
                
                return new TestResult("Theme Manager", true, "تم اختبار نظام الثيمات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Theme Manager", false, $"فشل اختبار نظام الثيمات: {ex.Message}");
            }
        }

        private static TestResult TestPermissionManager()
        {
            try
            {
                // اختبار إنشاء مستخدم تجريبي
                var testUser = new User
                {
                    Username = "testuser",
                    FullName = "Test User",
                    Role = "Admin",
                    IsAdmin = true,
                    IsActive = true
                };
                
                // اختبار الصلاحيات
                var hasPermission = PermissionManager.HasPermission(testUser, PermissionManager.Permission.ViewSales);
                if (!hasPermission)
                {
                    return new TestResult("Permission Manager", false, "فشل في التحقق من الصلاحيات");
                }
                
                // اختبار الأدوار
                var userRole = PermissionManager.GetUserRole(testUser);
                if (userRole != PermissionManager.Role.Admin)
                {
                    return new TestResult("Permission Manager", false, "فشل في تحديد دور المستخدم");
                }
                
                return new TestResult("Permission Manager", true, "تم اختبار نظام الصلاحيات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Permission Manager", false, $"فشل اختبار نظام الصلاحيات: {ex.Message}");
            }
        }

        private static void DisplayTestResults(List<TestResult> results)
        {
            Console.WriteLine("\n=== نتائج الاختبارات ===");
            
            var passedTests = results.Count(r => r.Passed);
            var failedTests = results.Count(r => !r.Passed);
            
            Console.WriteLine($"إجمالي الاختبارات: {results.Count}");
            Console.WriteLine($"نجحت: {passedTests}");
            Console.WriteLine($"فشلت: {failedTests}");
            Console.WriteLine($"نسبة النجاح: {(double)passedTests / results.Count * 100:F1}%");
            
            Console.WriteLine("\n=== تفاصيل النتائج ===");
            foreach (var result in results)
            {
                var status = result.Passed ? "✅ نجح" : "❌ فشل";
                Console.WriteLine($"{status} - {result.TestName}: {result.Message}");
            }
            
            if (failedTests > 0)
            {
                Console.WriteLine("\n⚠️ يرجى مراجعة الاختبارات الفاشلة وإصلاحها قبل النشر");
            }
            else
            {
                Console.WriteLine("\n🎉 جميع الاختبارات نجحت! النظام جاهز للنشر");
            }
        }
    }

    public class TestResult
    {
        public string TestName { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }

        public TestResult(string testName, bool passed, string message)
        {
            TestName = testName;
            Passed = passed;
            Message = message;
        }
    }
}
