using System.Windows;
using ETAG_ERP.Models;
using ETAG_ERP.Views;
using ETAG_ERP.Helpers;

namespace ETAG_ERP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // تهيئة نظام معالجة الأخطاء
            try
            {
                ErrorHandler.HandleApplicationStartup();
            }
            catch
            {
                // إذا فشل تسجيل بداية التطبيق، تجاهل
            }

            // تهيئة نظام الثيمات
            try
            {
                ThemeManager.InitializeTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة نظام الثيمات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // ✅ تهيئة قاعدة البيانات — يضمن إنشاء كل الجداول إذا لم تكن موجودة
            try
            {
                DatabaseHelper.InitializeDatabase();
                
                // إضافة البيانات التجريبية إذا لم تكن موجودة
                var hasData = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Items");
                if (hasData != null && Convert.ToInt32(hasData) == 0)
                {
                    SampleDataSeeder.SeedAllSampleData();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Database Initialization");
            }

#if DEBUG
            // فتح MainWindow مباشرة في وضع التطوير
            var mainWindow = new Views.MainWindow();
            mainWindow.Show();
            this.MainWindow = mainWindow;
#else
            // فتح نافذة تسجيل الدخول في الوضع العادي (الإصدار النهائي)
            var loginWindow = new ETAG_POS.LoginWindow();
            loginWindow.Show();
            this.MainWindow = loginWindow;
#endif
        }
    }

    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
    }
}
