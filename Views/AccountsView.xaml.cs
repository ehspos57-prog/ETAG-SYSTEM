using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AccountsView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Invoice> Sales { get; set; } = new ObservableCollection<Invoice>();
        public ObservableCollection<Purchase> Purchases { get; set; } = new ObservableCollection<Purchase>();
        public ObservableCollection<Return> Returns { get; set; } = new ObservableCollection<Return>();
        public ObservableCollection<PriceOffer> Quotes { get; set; } = new ObservableCollection<PriceOffer>();
        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();
        public ObservableCollection<LedgerEntry> LedgerEntries { get; set; } = new ObservableCollection<LedgerEntry>();
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AccountsView()
        {
            InitializeComponent();
            DataContext = this;
            LoadAllData();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadAllData()
        {
            try
            {
                LoadSales();
                LoadPurchases();
                LoadReturns();
                LoadQuotes();
                LoadExpenses();
                LoadLedgerEntries();
                LoadClients();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSales()
        {
            try
            {
                Sales.Clear();
                var sales = DatabaseHelper.GetAllInvoices();
                foreach (var sale in sales)
                {
                    Sales.Add(sale);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المبيعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPurchases()
        {
            try
            {
                Purchases.Clear();
                var purchases = DatabaseHelper.GetAllPurchases();
                foreach (var purchase in purchases)
                {
                    Purchases.Add(purchase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المشتريات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadReturns()
        {
            try
            {
                Returns.Clear();
                // Note: You may need to implement GetAllReturns in DatabaseHelper
                // var returns = DatabaseHelper.GetAllReturns();
                // foreach (var ret in returns)
                // {
                //     Returns.Add(ret);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المرتجعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadQuotes()
        {
            try
            {
                Quotes.Clear();
                // Note: You may need to implement GetAllQuotes in DatabaseHelper
                // var quotes = DatabaseHelper.GetAllQuotes();
                // foreach (var quote in quotes)
                // {
                //     Quotes.Add(quote);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل عروض الأسعار: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadExpenses()
        {
            try
            {
                Expenses.Clear();
                var expenses = DatabaseHelper.GetAllExpenses();
                foreach (var expense in expenses)
                {
                    Expenses.Add(expense);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المصروفات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLedgerEntries()
        {
            try
            {
                LedgerEntries.Clear();
                // Note: You may need to implement GetAllLedgerEntries in DatabaseHelper
                // var entries = DatabaseHelper.GetAllLedgerEntries();
                // foreach (var entry in entries)
                // {
                //     LedgerEntries.Add(entry);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل دفتر الأستاذ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadClients()
        {
            try
            {
                Clients.Clear();
                var clients = DatabaseHelper.GetAllClients();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل العملاء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sales Events
        private void NewInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            var salesView = new SalesView();
            var window = new Window
            {
                Content = salesView,
                Title = "فاتورة جديدة",
                Width = 1000,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            LoadSales();
        }

        private void PrintSalesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportHelper.ExportDataTableToExcel(GetSalesDataTable(), "المبيعات.xlsx");
                MessageBox.Show("تم تصدير تقرير المبيعات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المبيعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportSalesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_المبيعات_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetSalesDataTable(), saveDialog.FileName, "المبيعات");
                    MessageBox.Show("تم تصدير تقرير المبيعات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المبيعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Purchases Events
        private void NewPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            var purchaseView = new PurchaseView();
            var window = new Window
            {
                Content = purchaseView,
                Title = "أمر شراء جديد",
                Width = 1000,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            LoadPurchases();
        }

        private void PrintPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetPurchasesDataTable(), "المشتريات.xlsx", "المشتريات");
                MessageBox.Show("تم تصدير تقرير المشتريات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المشتريات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_المشتريات_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetPurchasesDataTable(), saveDialog.FileName, "المشتريات");
                    MessageBox.Show("تم تصدير تقرير المشتريات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المشتريات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Returns Events
        private void NewReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var returnView = new ReturnsView();
            var window = new Window
            {
                Content = returnView,
                Title = "إرجاع جديد",
                Width = 1000,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            LoadReturns();
        }

        private void PrintReturnsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetReturnsDataTable(), "المرتجعات.xlsx", "المرتجعات");
                MessageBox.Show("تم تصدير تقرير المرتجعات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المرتجعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportReturnsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_المرتجعات_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetReturnsDataTable(), saveDialog.FileName, "المرتجعات");
                    MessageBox.Show("تم تصدير تقرير المرتجعات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المرتجعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Quotes Events
        private void NewQuoteButton_Click(object sender, RoutedEventArgs e)
        {
            var quoteView = new PriceQuoteView();
            quoteView.ShowDialog();
            LoadQuotes();
        }

        private void PrintQuotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetQuotesDataTable(), "عروض_الأسعار.xlsx", "عروض الأسعار");
                MessageBox.Show("تم تصدير تقرير عروض الأسعار بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير عروض الأسعار: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportQuotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_عروض_الأسعار_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetQuotesDataTable(), saveDialog.FileName, "عروض الأسعار");
                    MessageBox.Show("تم تصدير تقرير عروض الأسعار بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير عروض الأسعار: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Expenses Events
        private void NewExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            var expenseView = new AddEditExpenseView();
            var window = new Window
            {
                Content = expenseView,
                Title = "مصروف جديد",
                Width = 600,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
            LoadExpenses();
        }

        private void PrintExpensesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetExpensesDataTable(), "المصروفات.xlsx", "المصروفات");
                MessageBox.Show("تم تصدير تقرير المصروفات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المصروفات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExpensesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_المصروفات_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetExpensesDataTable(), saveDialog.FileName, "المصروفات");
                    MessageBox.Show("تم تصدير تقرير المصروفات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير المصروفات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Ledger Events
        private void NewLedgerEntryButton_Click(object sender, RoutedEventArgs e)
        {
            var ledgerEntryWindow = new AddLedgerEntryWindow();
            if (ledgerEntryWindow.ShowDialog() == true)
            {
                LoadLedgerEntries();
            }
        }

        private void PrintLedgerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetLedgerDataTable(), "دفتر_الأستاذ.xlsx", "دفتر الأستاذ");
                MessageBox.Show("تم تصدير دفتر الأستاذ بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير دفتر الأستاذ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportLedgerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"دفتر_الأستاذ_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetLedgerDataTable(), saveDialog.FileName, "دفتر الأستاذ");
                    MessageBox.Show("تم تصدير دفتر الأستاذ بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير دفتر الأستاذ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clients Events
        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new AddClientWindow();
            if (clientWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void PrintClientsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportToExcel(GetClientsDataTable(), "العملاء.xlsx", "العملاء");
                MessageBox.Show("تم تصدير تقرير العملاء بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير العملاء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportClientsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_العملاء_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(GetClientsDataTable(), saveDialog.FileName, "العملاء");
                    MessageBox.Show("تم تصدير تقرير العملاء بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير العملاء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper methods for data conversion
        private System.Data.DataTable GetSalesDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("رقم الفاتورة", typeof(string));
            dt.Columns.Add("العميل", typeof(string));
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("المبلغ", typeof(decimal));
            dt.Columns.Add("الحالة", typeof(string));

            foreach (var sale in Sales)
            {
                dt.Rows.Add(sale.InvoiceNumber, sale.ClientName, sale.Date, sale.TotalAmount, sale.Status);
            }

            return dt;
        }

        private System.Data.DataTable GetPurchasesDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("رقم الأمر", typeof(string));
            dt.Columns.Add("المورد", typeof(string));
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("المبلغ", typeof(decimal));
            dt.Columns.Add("الملاحظات", typeof(string));

            foreach (var purchase in Purchases)
            {
                dt.Rows.Add(purchase.PurchaseNumber, purchase.Supplier, purchase.Date, purchase.Total, purchase.Notes);
            }

            return dt;
        }

        private System.Data.DataTable GetReturnsDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("رقم الإرجاع", typeof(string));
            dt.Columns.Add("العميل", typeof(string));
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("المبلغ", typeof(decimal));
            dt.Columns.Add("الملاحظات", typeof(string));

            foreach (var ret in Returns)
            {
                dt.Rows.Add(ret.ReturnNumber, ret.Client?.Name, ret.Date, ret.Total, ret.Notes);
            }

            return dt;
        }

        private System.Data.DataTable GetQuotesDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("رقم العرض", typeof(string));
            dt.Columns.Add("العميل", typeof(string));
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("المبلغ", typeof(decimal));
            dt.Columns.Add("الملاحظات", typeof(string));

            foreach (var quote in Quotes)
            {
                dt.Rows.Add(quote.OfferNumber, quote.Client?.Name, quote.OfferDate, quote.TotalAmount, quote.Notes);
            }

            return dt;
        }

        private System.Data.DataTable GetExpensesDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("النوع", typeof(string));
            dt.Columns.Add("الوصف", typeof(string));
            dt.Columns.Add("المبلغ", typeof(decimal));
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("الفئة", typeof(string));

            foreach (var expense in Expenses)
            {
                dt.Rows.Add(expense.ExpenseType, expense.Description, expense.Amount, expense.ExpenseDate ?? DateTime.Now, expense.Category);
            }

            return dt;
        }

        private System.Data.DataTable GetLedgerDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("التاريخ", typeof(DateTime));
            dt.Columns.Add("الحساب", typeof(string));
            dt.Columns.Add("البيان", typeof(string));
            dt.Columns.Add("مدين", typeof(decimal));
            dt.Columns.Add("دائن", typeof(decimal));
            dt.Columns.Add("الرصيد", typeof(decimal));

            foreach (var entry in LedgerEntries)
            {
                dt.Rows.Add(entry.EntryDate, entry.AccountName, entry.Description, entry.Debit, entry.Credit, entry.Balance);
            }

            return dt;
        }

        private System.Data.DataTable GetClientsDataTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("الاسم", typeof(string));
            dt.Columns.Add("الهاتف", typeof(string));
            dt.Columns.Add("العنوان", typeof(string));
            dt.Columns.Add("الرصيد", typeof(decimal));

            foreach (var client in Clients)
            {
                dt.Rows.Add(client.Name, client.Phone, client.Address, client.Balance);
            }

            return dt;
        }

        private void ExportToExcel(System.Data.DataTable dataTable, string filePath, string sheetName)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                worksheet.Cells.AutoFitColumns();
                package.SaveAs(new System.IO.FileInfo(filePath));
            }
        }
    }
}