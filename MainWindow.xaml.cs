using ETAG_ERP.Helpers;
using ETAG_ERP.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            // شاشة البداية
            LoadContent(new SalesView());
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

        // ============ الأزرار (UserControl) ============
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
    }
}
