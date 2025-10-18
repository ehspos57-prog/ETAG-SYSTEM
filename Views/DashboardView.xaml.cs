using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class DashboardView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<DashboardKPI> KPIs { get; set; } = new ObservableCollection<DashboardKPI>();
        public ObservableCollection<RecentTransaction> RecentTransactions { get; set; } = new ObservableCollection<RecentTransaction>();
        public ObservableCollection<TopClient> TopClients { get; set; } = new ObservableCollection<TopClient>();

        private decimal _totalSales = 0;
        public decimal TotalSales
        {
            get => _totalSales;
            set
            {
                _totalSales = value;
                OnPropertyChanged(nameof(TotalSales));
            }
        }

        private decimal _totalPurchases = 0;
        public decimal TotalPurchases
        {
            get => _totalPurchases;
            set
            {
                _totalPurchases = value;
                OnPropertyChanged(nameof(TotalPurchases));
            }
        }

        private decimal _totalExpenses = 0;
        public decimal TotalExpenses
        {
            get => _totalExpenses;
            set
            {
                _totalExpenses = value;
                OnPropertyChanged(nameof(TotalExpenses));
            }
        }

        private decimal _netProfit = 0;
        public decimal NetProfit
        {
            get => _netProfit;
            set
            {
                _netProfit = value;
                OnPropertyChanged(nameof(NetProfit));
            }
        }

        private int _totalClients = 0;
        public int TotalClients
        {
            get => _totalClients;
            set
            {
                _totalClients = value;
                OnPropertyChanged(nameof(TotalClients));
            }
        }

        private int _totalItems = 0;
        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged(nameof(TotalItems));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardView()
        {
            InitializeComponent();
            DataContext = this;
            LoadDashboardData();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadKPIs();
                LoadRecentTransactions();
                LoadTopClients();
                UpdateKPIDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات لوحة التحكم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadKPIs()
        {
            try
            {
                // Total Sales
                var salesResult = DatabaseHelper.ExecuteScalar(@"
                    SELECT COALESCE(SUM(TotalAmount), 0) 
                    FROM Invoices 
                    WHERE InvoiceDate >= date('now', '-30 days')");

                TotalSales = salesResult != null && salesResult != DBNull.Value ? Convert.ToDecimal(salesResult) : 0;

                // Total Purchases
                var purchasesResult = DatabaseHelper.ExecuteScalar(@"
                    SELECT COALESCE(SUM(TotalAmount), 0) 
                    FROM Purchases 
                    WHERE PurchaseDate >= date('now', '-30 days')");

                TotalPurchases = purchasesResult != null && purchasesResult != DBNull.Value ? Convert.ToDecimal(purchasesResult) : 0;

                // Total Expenses
                var expensesResult = DatabaseHelper.ExecuteScalar(@"
                    SELECT COALESCE(SUM(Amount), 0) 
                    FROM Expenses 
                    WHERE ExpenseDate >= date('now', '-30 days')");

                TotalExpenses = expensesResult != null && expensesResult != DBNull.Value ? Convert.ToDecimal(expensesResult) : 0;

                // Net Profit
                NetProfit = TotalSales - TotalPurchases - TotalExpenses;

                // Total Clients
                var clientsResult = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Clients");
                TotalClients = clientsResult != null ? Convert.ToInt32(clientsResult) : 0;

                // Total Items
                var itemsResult = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Items");
                TotalItems = itemsResult != null ? Convert.ToInt32(itemsResult) : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل مؤشرات الأداء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentTransactions()
        {
            try
            {
                RecentTransactions.Clear();
                
                var query = @"
                    SELECT 'فاتورة' as Type, InvoiceNumber as Number, TotalAmount as Amount, InvoiceDate as Date, ClientName as Client
                    FROM Invoices 
                    WHERE InvoiceDate >= date('now', '-7 days')
                    UNION ALL
                    SELECT 'مصروف' as Type, Description as Number, Amount, ExpenseDate as Date, '' as Client
                    FROM Expenses 
                    WHERE ExpenseDate >= date('now', '-7 days')
                    ORDER BY Date DESC
                    LIMIT 10";

                var dt = DatabaseHelper.GetDataTable(query);
                
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    RecentTransactions.Add(new RecentTransaction
                    {
                        Type = row["Type"]?.ToString() ?? "",
                        Number = row["Number"]?.ToString() ?? "",
                        Amount = Convert.ToDecimal(row["Amount"]),
                        Date = Convert.ToDateTime(row["Date"]),
                        Client = row["Client"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المعاملات الأخيرة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTopClients()
        {
            try
            {
                TopClients.Clear();
                
                var query = @"
                    SELECT c.Name, COALESCE(SUM(i.TotalAmount), 0) as TotalAmount, COUNT(i.Id) as TransactionCount
                    FROM Clients c
                    LEFT JOIN Invoices i ON c.Id = i.ClientId
                    WHERE i.InvoiceDate >= date('now', '-30 days')
                    GROUP BY c.Id, c.Name
                    ORDER BY TotalAmount DESC
                    LIMIT 5";

                var dt = DatabaseHelper.GetDataTable(query);
                
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    TopClients.Add(new TopClient
                    {
                        Name = row["Name"]?.ToString() ?? "",
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        TransactionCount = Convert.ToInt32(row["TransactionCount"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل أفضل العملاء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateKPIDisplay()
        {
            try
            {
                // Update KPI text blocks
                TotalSalesValue.Text = TotalSales.ToString("N2");
                TotalPurchasesValue.Text = TotalPurchases.ToString("N2");
                TotalExpensesValue.Text = TotalExpenses.ToString("N2");
                NetProfitValue.Text = NetProfit.ToString("N2");
                TotalClientsValue.Text = TotalClients.ToString();
                TotalItemsValue.Text = TotalItems.ToString();

                // Color coding for profit
                if (NetProfit > 0)
                    NetProfitValue.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                else if (NetProfit < 0)
                    NetProfitValue.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                else
                    NetProfitValue.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث عرض المؤشرات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private void ExportReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_لوحة_التحكم_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportDashboardToExcel(saveDialog.FileName);
                    MessageBox.Show("تم تصدير التقرير بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير التقرير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportDashboardToExcel(string filePath)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                // Dashboard Summary Sheet
                var summarySheet = package.Workbook.Worksheets.Add("ملخص لوحة التحكم");
                
                summarySheet.Cells[1, 1].Value = "مؤشرات الأداء الرئيسية";
                summarySheet.Cells[1, 1].Style.Font.Bold = true;
                summarySheet.Cells[1, 1].Style.Font.Size = 16;

                summarySheet.Cells[3, 1].Value = "إجمالي المبيعات";
                summarySheet.Cells[3, 2].Value = TotalSales;

                summarySheet.Cells[4, 1].Value = "إجمالي المشتريات";
                summarySheet.Cells[4, 2].Value = TotalPurchases;

                summarySheet.Cells[5, 1].Value = "إجمالي المصروفات";
                summarySheet.Cells[5, 2].Value = TotalExpenses;

                summarySheet.Cells[6, 1].Value = "صافي الربح";
                summarySheet.Cells[6, 2].Value = NetProfit;

                summarySheet.Cells[7, 1].Value = "إجمالي العملاء";
                summarySheet.Cells[7, 2].Value = TotalClients;

                summarySheet.Cells[8, 1].Value = "إجمالي الأصناف";
                summarySheet.Cells[8, 2].Value = TotalItems;

                // Recent Transactions Sheet
                var transactionsSheet = package.Workbook.Worksheets.Add("المعاملات الأخيرة");
                
                transactionsSheet.Cells[1, 1].Value = "النوع";
                transactionsSheet.Cells[1, 2].Value = "الرقم";
                transactionsSheet.Cells[1, 3].Value = "المبلغ";
                transactionsSheet.Cells[1, 4].Value = "التاريخ";
                transactionsSheet.Cells[1, 5].Value = "العميل";

                int row = 2;
                foreach (var transaction in RecentTransactions)
                {
                    transactionsSheet.Cells[row, 1].Value = transaction.Type;
                    transactionsSheet.Cells[row, 2].Value = transaction.Number;
                    transactionsSheet.Cells[row, 3].Value = transaction.Amount;
                    transactionsSheet.Cells[row, 4].Value = transaction.Date.ToString("yyyy-MM-dd");
                    transactionsSheet.Cells[row, 5].Value = transaction.Client;
                    row++;
                }

                // Top Clients Sheet
                var clientsSheet = package.Workbook.Worksheets.Add("أفضل العملاء");
                
                clientsSheet.Cells[1, 1].Value = "اسم العميل";
                clientsSheet.Cells[1, 2].Value = "إجمالي المبيعات";
                clientsSheet.Cells[1, 3].Value = "عدد المعاملات";

                row = 2;
                foreach (var client in TopClients)
                {
                    clientsSheet.Cells[row, 1].Value = client.Name;
                    clientsSheet.Cells[row, 2].Value = client.TotalAmount;
                    clientsSheet.Cells[row, 3].Value = client.TransactionCount;
                    row++;
                }

                // Auto-fit columns
                summarySheet.Cells.AutoFitColumns();
                transactionsSheet.Cells.AutoFitColumns();
                clientsSheet.Cells.AutoFitColumns();

                package.SaveAs(new System.IO.FileInfo(filePath));
            }
        }
    }

    public class DashboardKPI
    {
        public string Title { get; set; } = "";
        public decimal Value { get; set; }
        public string Color { get; set; } = "#3498DB";
    }

    public class RecentTransaction
    {
        public string Type { get; set; } = "";
        public string Number { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Client { get; set; } = "";
    }

    public class TopClient
    {
        public string Name { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}