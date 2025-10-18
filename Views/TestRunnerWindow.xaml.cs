using ETAG_ERP.Helpers;
using ETAG_ERP.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ETAG_ERP.Views
{
    public partial class TestRunnerWindow : Window
    {
        private List<TestResultViewModel> _testResults = new List<TestResultViewModel>();
        private bool _isRunning = false;

        public TestRunnerWindow()
        {
            InitializeComponent();
            InitializeTestRunner();
        }

        private void InitializeTestRunner()
        {
            TestResultsList.ItemsSource = _testResults;
            UpdateSummary();
        }

        private async void RunTestsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning) return;

            _isRunning = true;
            _testResults.Clear();
            TestResultsList.ItemsSource = null;
            TestResultsList.ItemsSource = _testResults;

            try
            {
                await RunAllTestsAsync();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Test Runner");
            }
            finally
            {
                _isRunning = false;
            }
        }

        private async Task RunAllTestsAsync()
        {
            var testMethods = new List<Func<TestResult>>
            {
                () => RunTest("Database Connection", () => TestDatabaseConnection()),
                () => RunTest("Database Initialization", () => TestDatabaseInitialization()),
                () => RunTest("Category Operations", () => TestCategoryOperations()),
                () => RunTest("Item Operations", () => TestItemOperations()),
                () => RunTest("Client Operations", () => TestClientOperations()),
                () => RunTest("Invoice Operations", () => TestInvoiceOperations()),
                () => RunTest("Validation Helper", () => TestValidationHelper()),
                () => RunTest("Error Handler", () => TestErrorHandler()),
                () => RunTest("Theme Manager", () => TestThemeManager()),
                () => RunTest("Permission Manager", () => TestPermissionManager())
            };

            var totalTests = testMethods.Count;
            var completedTests = 0;

            foreach (var testMethod in testMethods)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    ProgressText.Text = $"جاري تشغيل الاختبار {completedTests + 1} من {totalTests}";
                    TestProgressBar.Value = (double)completedTests / totalTests * 100;
                });

                var result = await Task.Run(testMethod);
                _testResults.Add(new TestResultViewModel(result));
                
                await Dispatcher.InvokeAsync(() =>
                {
                    TestResultsList.ItemsSource = null;
                    TestResultsList.ItemsSource = _testResults;
                    UpdateSummary();
                });

                completedTests++;
                await Task.Delay(100); // تأخير صغير لعرض التقدم
            }

            await Dispatcher.InvokeAsync(() =>
            {
                ProgressText.Text = "تم الانتهاء من جميع الاختبارات";
                TestProgressBar.Value = 100;
            });
        }

        private TestResult RunTest(string testName, Func<TestResult> testFunc)
        {
            var startTime = DateTime.Now;
            try
            {
                var result = testFunc();
                var endTime = DateTime.Now;
                result.ExecutionTime = (endTime - startTime).TotalMilliseconds;
                return result;
            }
            catch (Exception ex)
            {
                return new TestResult(testName, false, $"خطأ في تشغيل الاختبار: {ex.Message}");
            }
        }

        private TestResult TestDatabaseConnection()
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

        private TestResult TestDatabaseInitialization()
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

        private TestResult TestCategoryOperations()
        {
            try
            {
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
                var categories = DatabaseHelper.GetAllCategories();
                category.Name = "Updated Test Category";
                DatabaseHelper.UpdateCategory(category);
                DatabaseHelper.DeleteCategory(category.Id);

                return new TestResult("Category Operations", true, "تم اختبار عمليات التصنيفات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Category Operations", false, $"فشل اختبار عمليات التصنيفات: {ex.Message}");
            }
        }

        private TestResult TestItemOperations()
        {
            try
            {
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
                var items = DatabaseHelper.GetAllItems();
                item.ItemName = "Updated Test Item";
                DatabaseHelper.UpdateItem(item);
                DatabaseHelper.DeleteItem(item.Id);

                return new TestResult("Item Operations", true, "تم اختبار عمليات الأصناف بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Item Operations", false, $"فشل اختبار عمليات الأصناف: {ex.Message}");
            }
        }

        private TestResult TestClientOperations()
        {
            try
            {
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
                var clients = DatabaseHelper.GetAllClients();
                client.Name = "Updated Test Client";
                DatabaseHelper.UpdateClient(client);
                DatabaseHelper.DeleteClient(client.Id);

                return new TestResult("Client Operations", true, "تم اختبار عمليات العملاء بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Client Operations", false, $"فشل اختبار عمليات العملاء: {ex.Message}");
            }
        }

        private TestResult TestInvoiceOperations()
        {
            try
            {
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
                var invoices = DatabaseHelper.GetAllInvoices();
                invoice.Status = "Paid";
                DatabaseHelper.UpdateInvoice(invoice);
                DatabaseHelper.DeleteInvoice(invoice.Id);

                return new TestResult("Invoice Operations", true, "تم اختبار عمليات الفواتير بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Invoice Operations", false, $"فشل اختبار عمليات الفواتير: {ex.Message}");
            }
        }

        private TestResult TestValidationHelper()
        {
            try
            {
                var validEmail = ValidationHelper.ValidateEmail("test@example.com");
                var invalidEmail = ValidationHelper.ValidateEmail("invalid-email");
                var validPhone = ValidationHelper.ValidatePhone("1234567890");
                var invalidPhone = ValidationHelper.ValidatePhone("abc");

                if (!validEmail || invalidEmail || !validPhone || invalidPhone)
                {
                    return new TestResult("Validation Helper", false, "فشل اختبار نظام التحقق");
                }

                return new TestResult("Validation Helper", true, "تم اختبار نظام التحقق بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Validation Helper", false, $"فشل اختبار نظام التحقق: {ex.Message}");
            }
        }

        private TestResult TestErrorHandler()
        {
            try
            {
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

        private TestResult TestThemeManager()
        {
            try
            {
                var originalTheme = ThemeManager.CurrentTheme;
                ThemeManager.SetTheme(ThemeManager.Theme.Dark);
                ThemeManager.SetTheme(ThemeManager.Theme.Light);
                ThemeManager.SetTheme(originalTheme);

                return new TestResult("Theme Manager", true, "تم اختبار نظام الثيمات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Theme Manager", false, $"فشل اختبار نظام الثيمات: {ex.Message}");
            }
        }

        private TestResult TestPermissionManager()
        {
            try
            {
                var testUser = new User
                {
                    Username = "testuser",
                    FullName = "Test User",
                    Role = "Admin",
                    IsAdmin = true,
                    IsActive = true
                };

                var hasPermission = PermissionManager.HasPermission(testUser, PermissionManager.Permission.ViewSales);
                var userRole = PermissionManager.GetUserRole(testUser);

                if (!hasPermission || userRole != PermissionManager.Role.Admin)
                {
                    return new TestResult("Permission Manager", false, "فشل في اختبار نظام الصلاحيات");
                }

                return new TestResult("Permission Manager", true, "تم اختبار نظام الصلاحيات بنجاح");
            }
            catch (Exception ex)
            {
                return new TestResult("Permission Manager", false, $"فشل اختبار نظام الصلاحيات: {ex.Message}");
            }
        }

        private void UpdateSummary()
        {
            var totalTests = _testResults.Count;
            var passedTests = _testResults.Count(r => r.Passed);
            var failedTests = _testResults.Count(r => !r.Passed);
            var successRate = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0;

            TotalTestsText.Text = totalTests.ToString();
            PassedTestsText.Text = passedTests.ToString();
            FailedTestsText.Text = failedTests.ToString();
            SuccessRateText.Text = $"{successRate:F1}%";
        }

        private void RestartTestsButton_Click(object sender, RoutedEventArgs e)
        {
            _testResults.Clear();
            TestResultsList.ItemsSource = null;
            TestResultsList.ItemsSource = _testResults;
            UpdateSummary();
            TestProgressBar.Value = 0;
            ProgressText.Text = "جاهز للتشغيل";
        }

        private void PerformanceReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PerformanceOptimizer.GeneratePerformanceReport();
                MessageBox.Show("تم إنشاء تقرير الأداء بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Performance Report");
            }
        }
    }

    public class TestResultViewModel
    {
        public string TestName { get; set; } = "";
        public bool Passed { get; set; }
        public string Message { get; set; } = "";
        public double ExecutionTime { get; set; }
        public string StatusIcon => Passed ? "✅" : "❌";
        public string BackgroundColor => Passed ? "#E8F5E8" : "#FFE8E8";
        public string BorderColor => Passed ? "#27AE60" : "#E74C3C";
        public string TextColor => Passed ? "#27AE60" : "#E74C3C";

        public TestResultViewModel(TestResult result)
        {
            TestName = result.TestName;
            Passed = result.Passed;
            Message = result.Message;
            ExecutionTime = result.ExecutionTime;
        }
    }
}
