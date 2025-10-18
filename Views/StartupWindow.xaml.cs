using ETAG_ERP.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ETAG_ERP.Views
{
    public partial class StartupWindow : Window
    {
        private int _loadingStep = 0;
        private readonly string[] _loadingSteps = {
            "جاري تحميل النظام...",
            "جاري تهيئة قاعدة البيانات...",
            "جاري تحميل البيانات...",
            "جاري تحضير واجهة المستخدم...",
            "جاري التحقق من الصلاحيات...",
            "جاري تحميل الإعدادات...",
            "جاري إنهاء التحميل..."
        };

        public StartupWindow()
        {
            InitializeComponent();
            InitializeStartup();
        }

        private async void InitializeStartup()
        {
            try
            {
                // بدء تحميل النظام
                await LoadSystemAsync();
                
                // إغلاق نافذة البداية وفتح النافذة الرئيسية
                var mainWindow = new MainWindow();
                mainWindow.Show();
                
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "System Startup");
                Application.Current.Shutdown();
            }
        }

        private async Task LoadSystemAsync()
        {
            // تحميل النظام خطوة بخطوة
            for (int i = 0; i < _loadingSteps.Length; i++)
            {
                await UpdateLoadingStep(i);
                await Task.Delay(500); // تأخير لمحاكاة التحميل
            }
        }

        private async Task UpdateLoadingStep(int step)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                _loadingStep = step;
                StatusText.Text = _loadingSteps[step];
                
                // تحديث شريط التقدم
                var progress = (double)(step + 1) / _loadingSteps.Length * 100;
                LoadingProgressBar.Value = progress;
                
                // تحميل البيانات الفعلية
                switch (step)
                {
                    case 1:
                        DatabaseHelper.InitializeDatabase();
                        break;
                    case 2:
                        LoadInitialData();
                        break;
                    case 3:
                        PrepareUI();
                        break;
                    case 4:
                        CheckPermissions();
                        break;
                    case 5:
                        LoadSettings();
                        break;
                }
            });
        }

        private void LoadInitialData()
        {
            try
            {
                // تحميل البيانات الأساسية
                var categories = DatabaseHelper.GetAllCategories();
                var items = DatabaseHelper.GetAllItems();
                var clients = DatabaseHelper.GetAllClients();
                
                // تسجيل نجاح التحميل
                ErrorHandler.LogInfo($"تم تحميل {categories.Count} تصنيف، {items.Count} صنف، {clients.Count} عميل", "Data Loading");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initial Data Loading");
            }
        }

        private void PrepareUI()
        {
            try
            {
                // تحضير واجهة المستخدم
                ThemeManager.InitializeTheme();
                
                // تحسين الأداء
                PerformanceOptimizer.OptimizeApplicationStartup();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "UI Preparation");
            }
        }

        private void CheckPermissions()
        {
            try
            {
                // التحقق من الصلاحيات
                var currentUser = SessionManager.CurrentUser;
                if (currentUser != null)
                {
                    var permissions = PermissionManager.GetCurrentUserPermissions();
                    ErrorHandler.LogInfo($"تم تحميل {permissions.Count} صلاحية للمستخدم {currentUser.Username}", "Permissions");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Permissions Check");
            }
        }

        private void LoadSettings()
        {
            try
            {
                // تحميل الإعدادات
                var companyName = DatabaseHelper.GetSetting("CompanyName") ?? "ETAG ERP";
                var theme = DatabaseHelper.GetSetting("Theme") ?? "Light";
                
                ErrorHandler.LogInfo($"تم تحميل إعدادات الشركة: {companyName}, الثيم: {theme}", "Settings");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Settings Loading");
            }
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ToggleButton toggleButton)
                {
                    if (toggleButton.IsChecked == true)
                    {
                        // تفعيل الوضع الداكن
                        ThemeManager.SetTheme(ThemeManager.Theme.Dark);
                        toggleButton.Content = "الوضع الفاتح";
                    }
                    else
                    {
                        // تفعيل الوضع الفاتح
                        ThemeManager.SetTheme(ThemeManager.Theme.Light);
                        toggleButton.Content = "الوضع الداكن";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Theme Toggle");
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // إضافة تأثيرات بصرية
            AddStartupAnimations();
        }

        private void AddStartupAnimations()
        {
            // تأثير الظهور
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            
            this.BeginAnimation(OpacityProperty, fadeIn);
            
            // تأثير التكبير
            var scaleTransform = new ScaleTransform(0.8, 0.8);
            this.RenderTransform = scaleTransform;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            
            var scaleAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }
    }
}
