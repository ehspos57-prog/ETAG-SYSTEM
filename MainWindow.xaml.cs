using ETAG_ERP.Helpers;
using ETAG_ERP.ViewModels;
using ETAG_ERP.Infrastructure.Network;
using ETAG_ERP.Infrastructure.Database;
using ETAG_ERP.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Threading.Tasks;

namespace ETAG_ERP.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private NetworkManager? _networkManager;
        private IConfiguration _configuration;
        private ILogger<MainWindow> _logger;
        private bool _isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // شاشة البداية
            LoadContent(new DashboardView());
            InitializeNetwork();
        }

        private void InitializeServices()
        {
            var services = new ServiceCollection();
            
            // إضافة التكوين
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Configuration/AppSettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            services.AddSingleton<IConfiguration>(config);
            
            // إضافة خدمات ETAG ERP
            services.AddETAGERPServices(config);
            
            var serviceProvider = services.BuildServiceProvider();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<MainWindow>>();
        }

        private async void InitializeNetwork()
        {
            try
            {
                _networkManager = new NetworkManager(_configuration, _logger);
                _networkManager.ConnectionStatusChanged += OnConnectionStatusChanged;
                _networkManager.UserConnected += OnUserConnected;
                _networkManager.UserDisconnected += OnUserDisconnected;

                // Try to start as server first, if fails, try to connect to existing server
                var serverStarted = await _networkManager.StartServerAsync();
                if (!serverStarted)
                {
                    var serverUrl = $"http://{_configuration["NetworkSettings:ServerIP"]}:{_configuration["NetworkSettings:ServerPort"]}";
                    await _networkManager.ConnectToServerAsync(serverUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تهيئة الشبكة");
                UpdateStatus("غير متصل - وضع محلي");
            }
        }

        // تحميل أي UserControl في ContentArea
        private void LoadContent(UserControl control)
        {
            try
            {
                ContentArea.Content = control;
                _viewModel.CurrentViewTitle = control.GetType().Name; // اسم الشاشة الحالي
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحميل الشاشة:\n{ex.Message}",
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // فتح نافذة جديدة
        private void OpenWindow(Window window)
        {
            try
            {
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"تعذر فتح النافذة:\n{ex.Message}",
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ============ Network Event Handlers ============
        private void OnConnectionStatusChanged(object? sender, NetworkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStatus(e.IsConnected ? "متصل" : "غير متصل");
                _logger.LogInformation($"حالة الاتصال: {e.Status}");
            });
        }

        private void OnUserConnected(object? sender, NetworkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"مستخدم متصل: {e.UserName}");
                _logger.LogInformation($"مستخدم متصل: {e.UserName}");
            });
        }

        private void OnUserDisconnected(object? sender, NetworkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStatus($"مستخدم منقطع: {e.SenderId}");
                _logger.LogInformation($"مستخدم منقطع: {e.SenderId}");
            });
        }

        private void UpdateStatus(string status)
        {
            StatusText.Text = status;
        }

        // ============ Window Controls ============
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            إغلاق_Click(sender, e);
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkTheme = !_isDarkTheme;
            // Implement theme switching logic here
            _logger.LogInformation($"تبديل الثيم إلى: {(_isDarkTheme ? "داكن" : "فاتح")}");
        }

        // ============ Navigation Handlers ============
        private void Dashboard_Click(object sender, RoutedEventArgs e) => LoadContent(new DashboardView());
        private void الأصناف_Click(object sender, RoutedEventArgs e) => LoadContent(new CategoryView());
        private void العملاء_Click(object sender, RoutedEventArgs e) => LoadContent(new CustomersView());
        private void فاتورة_Click(object sender, RoutedEventArgs e) => LoadContent(new SalesView());
        private void الحسابات_Click(object sender, RoutedEventArgs e) => LoadContent(new AccountsView());
        private void المرتجعات_Click(object sender, RoutedEventArgs e) => LoadContent(new ReturnsView());
        private void كشف_حساب_Click(object sender, RoutedEventArgs e) => LoadContent(new LedgerView());
        private void Expenses_Click(object sender, RoutedEventArgs e) => LoadContent(new AddEditExpenseView());
        private void تقارير_Click(object sender, RoutedEventArgs e) => LoadContent(new DashboardView());
        private void مندوبين_Click(object sender, RoutedEventArgs e) => LoadContent(new EmployeesSchedulerView());
        private void اختبار_النظام_Click(object sender, RoutedEventArgs e) => OpenWindow(new TestRunnerWindow());

        // ============ Advanced Module Handlers ============
        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            // Load project management module
            UpdateStatus("تحميل وحدة إدارة المشاريع...");
            LoadContent(new ProjectManagementView());
        }

        private void Manufacturing_Click(object sender, RoutedEventArgs e)
        {
            // Load manufacturing module
            UpdateStatus("تحميل وحدة التصنيع...");
            LoadContent(new ManufacturingView());
        }

        private void HR_Click(object sender, RoutedEventArgs e)
        {
            // Load HR module
            UpdateStatus("تحميل وحدة الموارد البشرية...");
            LoadContent(new HRView());
        }

        private void CRM_Click(object sender, RoutedEventArgs e)
        {
            // Load CRM module
            UpdateStatus("تحميل وحدة إدارة العملاء...");
            LoadContent(new CRMView());
        }

        private void TaxPortal_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentViewTitle = "البوابة الإلكترونية للضرائب المصرية";
            UpdateStatus("تحميل البوابة الضريبية...");
            LoadContent(new TaxPortalView());
        }

        private void CompanySettings_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentViewTitle = "إعدادات الشركة";
            UpdateStatus("تحميل إعدادات الشركة...");
            LoadContent(new CompanySettingsView());
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentViewTitle = "حول النظام";
            UpdateStatus("تحميل معلومات النظام...");
            LoadContent(new AboutView());
        }

        // ============ الأزرار (Windows) ============
        private void عروض_الأسعار_Click(object sender, RoutedEventArgs e) => OpenWindow(new PriceQuoteView());
        private void إذن_الاستلام_Click(object sender, RoutedEventArgs e) => OpenWindow(new ReceivingNoteView());
        private void الإعدادات_Click(object sender, RoutedEventArgs e) => OpenWindow(new SettingsView());

        private void البوابة_Click(object sender, RoutedEventArgs e)
        {
            var webWindow = new Window
            {
                Title = "بوابة الضرائب المصرية",
                Width = 1100,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new WebBrowser { Source = new Uri("https://invoicing.eta.gov.eg/") }
            };
            webWindow.Show();
        }

        // ============ وظائف خاصة ============
        private void فاتورة_جديدة_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "تأكد من حفظ الفواتير الحالية قبل الفتح.\nهل تريد فتح فاتورة جديدة؟",
                "تأكيد",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                LoadContent(new SalesView());
        }

        private void جرد_المخزون_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "هل تريد تصدير جرد المخزون؟\n(نعم → PDF، إلغاء → Excel، لا → إغلاق)",
                "جرد المخزون",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                InventoryExportHelper.ExportInventoryToPdf();
            else if (result == MessageBoxResult.Cancel)
                InventoryExportHelper.ExportInventoryToExcel();
        }

        // تسجيل الخروج مع حفظ
        private async void إغلاق_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("هل أنت متأكد من تسجيل الخروج؟", "تأكيد",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await SaveAllChangesAsync();
                    await DisconnectNetworkAsync();
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"فشل الحفظ:\n{ex.Message}",
                                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // دالة موحدة للحفظ
        private async Task SaveAllChangesAsync()
        {
            if (ContentArea.Content is FrameworkElement element &&
                element.DataContext is ISavable savable)
            {
                await savable.SaveAllChangesAsync();
                MessageBox.Show("تم حفظ البيانات بنجاح", "حفظ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task DisconnectNetworkAsync()
        {
            if (_networkManager != null)
            {
                await _networkManager.DisconnectAsync();
                _networkManager.Dispose();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            DisconnectNetworkAsync().Wait();
            base.OnClosed(e);
        }
    }
}
