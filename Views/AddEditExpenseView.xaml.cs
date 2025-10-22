using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ETAG_ERP.Views
{
    public partial class AddEditExpenseView : UserControl, INotifyPropertyChanged
    {
        private Expense _expense;
        public Expense Expense
        {
            get => _expense;
            set
            {
                _expense = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public AddEditExpenseView()
        {
            InitializeComponent();
            this.DataContext = this;
            
            Expense = new Expense 
            { 
                ExpenseDate = DateTime.Now,
                IsActive = true
            };

            SaveCommand = new RelayCommand(obj => Save(obj), null);
            CancelCommand = new RelayCommand(obj => Cancel(obj), null);
        }

        private void Save(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Expense.Description))
            {
                MessageBox.Show("الرجاء إدخال وصف المصروف.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Expense.Amount <= 0)
            {
                MessageBox.Show("الرجاء إدخال مبلغ صحيح.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                DatabaseHelper.InsertExpense(Expense);
                MessageBox.Show("تم حفظ المصروف بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // إعادة تعيين النموذج
                Expense = new Expense 
                { 
                    ExpenseDate = DateTime.Now,
                    IsActive = true
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ المصروف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            Expense = new Expense 
            { 
                ExpenseDate = DateTime.Now,
                IsActive = true
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}