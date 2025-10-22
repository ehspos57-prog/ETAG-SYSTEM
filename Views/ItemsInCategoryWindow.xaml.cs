using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class ItemsInCategoryWindow : Window, INotifyPropertyChanged
    {
        private Category _category;
        private Item _selectedItem;
        private ObservableCollection<Item> _items;
        private ObservableCollection<Item> _filteredItems;

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        public string CategoryName => $"الأصناف في تصنيف: {Category?.Name}";

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsItemSelected));
            }
        }

        public bool IsItemSelected => SelectedItem != null;

        public ObservableCollection<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
                FilterItems();
            }
        }

        public ObservableCollection<Item> FilteredItems
        {
            get => _filteredItems;
            set
            {
                _filteredItems = value;
                OnPropertyChanged();
            }
        }

        public ItemsInCategoryWindow(Category category)
        {
            InitializeComponent();
            DataContext = this;
            
            Category = category;
            Items = new ObservableCollection<Item>();
            FilteredItems = new ObservableCollection<Item>();
            
            LoadItems();
        }

        private void LoadItems()
        {
            Items.Clear();
            
            if (Category != null)
            {
                var items = DatabaseHelper.GetItemsByCategoryId(Category.CategoryID);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }

        private void FilterItems()
        {
            FilteredItems.Clear();
            
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    FilteredItems.Add(item);
                }
            }
            
            ItemsDataGrid.ItemsSource = FilteredItems;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterItems();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchTextBox.Text?.ToLower() ?? "";
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilterItems();
                return;
            }

            FilteredItems.Clear();
            
            var filtered = Items.Where(item => 
                item.Name.ToLower().Contains(searchText) ||
                item.ItemCode.ToLower().Contains(searchText) ||
                item.Description.ToLower().Contains(searchText));
            
            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }
            
            ItemsDataGrid.ItemsSource = FilteredItems;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadItems();
            SearchTextBox.Clear();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItemWindow();
            if (addItemWindow.ShowDialog() == true)
            {
                LoadItems();
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                var editItemWindow = new AddItemWindow(SelectedItem);
                if (editItemWindow.ShowDialog() == true)
                {
                    LoadItems();
                }
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                var result = MessageBox.Show(
                    $"هل أنت متأكد من حذف الصنف '{SelectedItem.Name}'؟",
                    "تأكيد الحذف",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteItem(SelectedItem.ItemID);
                        LoadItems();
                        MessageBox.Show("تم حذف الصنف بنجاح", "نجاح", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في حذف الصنف: {ex.Message}", "خطأ", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            // إنشاء تقرير الأصناف في التصنيف
            var reportWindow = new ItemsReportWindow(Category, FilteredItems.ToList());
            reportWindow.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsDataGrid.SelectedItem is Item item)
            {
                SelectedItem = item;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
