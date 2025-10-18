using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ETAG_ERP.Views
{
    public partial class AddLedgerEntryWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Account> Accounts { get; set; } = new ObservableCollection<Account>();

        private DateTime _entryDate = DateTime.Now;
        public DateTime EntryDate
        {
            get => _entryDate;
            set
            {
                _entryDate = value;
                OnPropertyChanged(nameof(EntryDate));
            }
        }

        private string _entryNumber = "";
        public string EntryNumber
        {
            get => _entryNumber;
            set
            {
                _entryNumber = value;
                OnPropertyChanged(nameof(EntryNumber));
            }
        }

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

        private string _description = "";
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _amountType = "مدين";
        public string AmountType
        {
            get => _amountType;
            set
            {
                _amountType = value;
                OnPropertyChanged(nameof(AmountType));
            }
        }

        private decimal _amount = 0;
        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        private string _reference = "";
        public string Reference
        {
            get => _reference;
            set
            {
                _reference = value;
                OnPropertyChanged(nameof(Reference));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddLedgerEntryWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadAccounts();
            GenerateEntryNumber();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void GenerateEntryNumber()
        {
            try
            {
                // Generate next entry number
                var lastEntry = DatabaseHelper.ExecuteScalar("SELECT MAX(CAST(RefNumber AS INTEGER)) FROM LedgerEntries WHERE RefNumber GLOB '[0-9]*'");
                int nextNumber = 1;
                
                if (lastEntry != null && lastEntry != DBNull.Value)
                {
                    nextNumber = Convert.ToInt32(lastEntry) + 1;
                }

                EntryNumber = nextNumber.ToString();
            }
            catch (Exception ex)
            {
                EntryNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (SelectedAccount == null)
                {
                    MessageBox.Show("يرجى اختيار الحساب", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    MessageBox.Show("يرجى إدخال البيان", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Amount <= 0)
                {
                    MessageBox.Show("يرجى إدخال مبلغ صحيح", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create ledger entry
                var entry = new LedgerEntry
                {
                    EntryDate = EntryDate,
                    AccountName = SelectedAccount.Name,
                    Description = Description,
                    Type = "Manual Entry",
                    RefNumber = EntryNumber,
                    AccountId = SelectedAccount.Id,
                    Debit = AmountType == "مدين" ? Amount : 0,
                    Credit = AmountType == "دائن" ? Amount : 0,
                    Balance = 0 // Will be calculated by the system
                };

                // Save to database
                string sql = @"
                    INSERT INTO LedgerEntries (EntryDate, AccountName, Description, Type, RefNumber, AccountId, Debit, Credit, Balance)
                    VALUES (@EntryDate, @AccountName, @Description, @Type, @RefNumber, @AccountId, @Debit, @Credit, @Balance)";

                DatabaseHelper.ExecuteNonQuery(sql,
                    new System.Data.SQLite.SQLiteParameter("@EntryDate", entry.EntryDate.ToString("yyyy-MM-dd")),
                    new System.Data.SQLite.SQLiteParameter("@AccountName", entry.AccountName),
                    new System.Data.SQLite.SQLiteParameter("@Description", entry.Description),
                    new System.Data.SQLite.SQLiteParameter("@Type", entry.Type),
                    new System.Data.SQLite.SQLiteParameter("@RefNumber", entry.RefNumber),
                    new System.Data.SQLite.SQLiteParameter("@AccountId", entry.AccountId),
                    new System.Data.SQLite.SQLiteParameter("@Debit", entry.Debit),
                    new System.Data.SQLite.SQLiteParameter("@Credit", entry.Credit),
                    new System.Data.SQLite.SQLiteParameter("@Balance", entry.Balance)
                );

                // Update account balance
                UpdateAccountBalance(SelectedAccount.Id, entry.Debit, entry.Credit);

                MessageBox.Show("تم حفظ القيد بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ القيد: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAccountBalance(int accountId, decimal debit, decimal credit)
        {
            try
            {
                // Get current balance
                var currentBalance = DatabaseHelper.ExecuteScalar("SELECT Balance FROM Accounts WHERE Id = @Id", 
                    new System.Data.SQLite.SQLiteParameter("@Id", accountId));

                decimal balance = 0;
                if (currentBalance != null && currentBalance != DBNull.Value)
                {
                    balance = Convert.ToDecimal(currentBalance);
                }

                // Update balance
                balance += debit - credit;
                
                DatabaseHelper.ExecuteNonQuery("UPDATE Accounts SET Balance = @Balance WHERE Id = @Id",
                    new System.Data.SQLite.SQLiteParameter("@Balance", balance),
                    new System.Data.SQLite.SQLiteParameter("@Id", accountId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث رصيد الحساب: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
