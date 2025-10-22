using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ETAG_ERP.Views
{
    public partial class LedgerView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<LedgerEntry> LedgerEntries { get; set; } = new ObservableCollection<LedgerEntry>();
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                _fromDate = value;
                OnPropertyChanged(nameof(FromDate));
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                _toDate = value;
                OnPropertyChanged(nameof(ToDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LedgerView()
        {
            InitializeComponent();
            DataContext = this;
            LoadAccounts();
            LoadLedgerEntries();
            SetDefaultDates();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetDefaultDates()
        {
            ToDate = DateTime.Now;
            FromDate = DateTime.Now.AddMonths(-1);
        }

        private void LoadAccounts()
        {
            try
            {
                Accounts.Clear();
                var accounts = DatabaseHelper.GetAllAccounts();
                
                foreach (var account in accounts)
                {
                    Accounts.Add(account);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الحسابات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLedgerEntries()
        {
            try
            {
                LedgerEntries.Clear();
                
                string query = @"
                    SELECT le.*, a.Name as AccountName 
                    FROM LedgerEntries le 
                    LEFT JOIN Accounts a ON le.AccountId = a.Id 
                    WHERE 1=1";
                
                var parameters = new List<System.Data.SQLite.SQLiteParameter>();

                if (SelectedAccount != null && SelectedAccount.Id > 0)
                {
                    query += " AND le.AccountId = @AccountId";
                    parameters.Add(new System.Data.SQLite.SQLiteParameter("@AccountId", SelectedAccount.Id));
                }

                if (FromDate.HasValue)
                {
                    query += " AND le.EntryDate >= @FromDate";
                    parameters.Add(new System.Data.SQLite.SQLiteParameter("@FromDate", FromDate.Value.ToString("yyyy-MM-dd")));
                }

                if (ToDate.HasValue)
                {
                    query += " AND le.EntryDate <= @ToDate";
                    parameters.Add(new System.Data.SQLite.SQLiteParameter("@ToDate", ToDate.Value.ToString("yyyy-MM-dd")));
                }

                query += " ORDER BY le.EntryDate DESC, le.Id DESC";

                var dt = DatabaseHelper.GetDataTable(query, parameters.ToArray());
                
                decimal runningBalance = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    var entry = new LedgerEntry
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        EntryDate = Convert.ToDateTime(row["EntryDate"]),
                        AccountName = row["AccountName"]?.ToString() ?? "",
                        Debit = Convert.ToDecimal(row["Debit"]),
                        Credit = Convert.ToDecimal(row["Credit"]),
                        Description = row["Description"]?.ToString() ?? "",
                        Type = row["Type"]?.ToString() ?? "",
                        RefNumber = row["RefNumber"]?.ToString() ?? "",
                        AccountId = row["AccountId"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccountId"])
                    };

                    // Calculate running balance
                    runningBalance += entry.Debit - entry.Credit;
                    entry.Balance = runningBalance;

                    LedgerEntries.Add(entry);
                }

                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل قيود دفتر الأستاذ: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSummary()
        {
            var totalDebit = LedgerEntries.Sum(e => e.Debit);
            var totalCredit = LedgerEntries.Sum(e => e.Credit);
            var balance = totalDebit - totalCredit;
            var entriesCount = LedgerEntries.Count;

            TotalDebitText.Text = totalDebit.ToString("N2");
            TotalCreditText.Text = totalCredit.ToString("N2");
            BalanceText.Text = balance.ToString("N2");
            EntriesCountText.Text = entriesCount.ToString();

            // Color coding for balance
            if (balance > 0)
                BalanceText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            else if (balance < 0)
                BalanceText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            else
                BalanceText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadLedgerEntries();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var document = GeneratePrintDocument();
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "طباعة دفتر الأستاذ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
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
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show("تم تصدير البيانات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            var addEntryWindow = new AddLedgerEntryWindow();
            if (addEntryWindow.ShowDialog() == true)
            {
                LoadLedgerEntries();
            }
        }

        private System.Windows.Documents.FlowDocument GeneratePrintDocument()
        {
            var doc = new System.Windows.Documents.FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new System.Windows.Media.FontFamily("Arial")
            };

            // Title
            var title = new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("دفتر الأستاذ"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(title);

            // Date range
            var dateRange = new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run($"من: {FromDate?.ToString("yyyy-MM-dd")} إلى: {ToDate?.ToString("yyyy-MM-dd")}"))
            {
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(dateRange);

            // Table
            var table = new System.Windows.Documents.Table
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Add columns
            for (int i = 0; i < 7; i++)
                table.Columns.Add(new System.Windows.Documents.TableColumn());

            // Header row
            var headerRow = new System.Windows.Documents.TableRow();
            string[] headers = { "التاريخ", "رقم القيد", "الحساب", "البيان", "مدين", "دائن", "الرصيد" };
            
            foreach (string header in headers)
            {
                var cell = new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(header)))
                {
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(5),
                    Background = System.Windows.Media.Brushes.LightGray
                };
                headerRow.Cells.Add(cell);
            }

            var headerGroup = new System.Windows.Documents.TableRowGroup();
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // Data rows
            var dataGroup = new System.Windows.Documents.TableRowGroup();
            foreach (var entry in LedgerEntries)
            {
                var row = new System.Windows.Documents.TableRow();
                
                string[] values = {
                    entry.EntryDate.ToString("yyyy-MM-dd"),
                    entry.RefNumber,
                    entry.AccountName,
                    entry.Description,
                    entry.Debit.ToString("N2"),
                    entry.Credit.ToString("N2"),
                    entry.Balance.ToString("N2")
                };

                foreach (string value in values)
                {
                    var cell = new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(value)))
                    {
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Padding = new Thickness(5)
                    };
                    row.Cells.Add(cell);
                }

                dataGroup.Rows.Add(row);
            }
            table.RowGroups.Add(dataGroup);

            doc.Blocks.Add(table);

            return doc;
        }

        private void ExportToExcel(string filePath)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("دفتر الأستاذ");

                // Headers
                worksheet.Cells[1, 1].Value = "التاريخ";
                worksheet.Cells[1, 2].Value = "رقم القيد";
                worksheet.Cells[1, 3].Value = "الحساب";
                worksheet.Cells[1, 4].Value = "البيان";
                worksheet.Cells[1, 5].Value = "مدين";
                worksheet.Cells[1, 6].Value = "دائن";
                worksheet.Cells[1, 7].Value = "الرصيد";

                // Data
                int row = 2;
                foreach (var entry in LedgerEntries)
                {
                    worksheet.Cells[row, 1].Value = entry.EntryDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 2].Value = entry.RefNumber;
                    worksheet.Cells[row, 3].Value = entry.AccountName;
                    worksheet.Cells[row, 4].Value = entry.Description;
                    worksheet.Cells[row, 5].Value = entry.Debit;
                    worksheet.Cells[row, 6].Value = entry.Credit;
                    worksheet.Cells[row, 7].Value = entry.Balance;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                package.SaveAs(new System.IO.FileInfo(filePath));
            }
        }
    }
}