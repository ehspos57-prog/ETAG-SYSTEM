using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AddPurchaseItemWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public PurchaseItemModel SelectedItem { get; set; } = new PurchaseItemModel();

        private Item _selectedItem;
        public Item SelectedItemFromCombo
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItemFromCombo));
                if (value != null)
                {
                    SelectedItem.ItemName = value.ItemName;
                    SelectedItem.UnitPrice = value.SellingPrice1; // Use first selling price as unit price
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddPurchaseItemWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadItems();
            SetupEventHandlers();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadItems()
        {
            try
            {
                Items.Clear();
                var items = DatabaseHelper.GetAllItems();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الأصناف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            SelectedItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedItem.Quantity) || e.PropertyName == nameof(SelectedItem.UnitPrice))
                {
                    UpdateTotal();
                }
            };
        }

        private void UpdateTotal()
        {
            var total = SelectedItem.Quantity * SelectedItem.UnitPrice;
            TotalText.Text = total.ToString("N2");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(SelectedItem.ItemName))
                {
                    MessageBox.Show("يرجى إدخال اسم الصنف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedItem.Quantity <= 0)
                {
                    MessageBox.Show("يرجى إدخال كمية صحيحة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedItem.UnitPrice <= 0)
                {
                    MessageBox.Show("يرجى إدخال سعر صحيح", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة الصنف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
