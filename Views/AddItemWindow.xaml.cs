using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ETAG_ERP.Views
{
    public partial class AddItemWindow : Window, INotifyPropertyChanged
    {
        private Item _item;
        private bool _isEditMode;

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        private string _itemCode = "";
        public string ItemCode
        {
            get => _itemCode;
            set
            {
                _itemCode = value;
                OnPropertyChanged(nameof(ItemCode));
            }
        }

        private string _itemName = "";
        public string ItemName
        {
            get => _itemName;
            set
            {
                _itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
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

        private decimal _sellingPrice1 = 0;
        public decimal SellingPrice1
        {
            get => _sellingPrice1;
            set
            {
                _sellingPrice1 = value;
                OnPropertyChanged(nameof(SellingPrice1));
            }
        }

        private decimal _sellingPrice2 = 0;
        public decimal SellingPrice2
        {
            get => _sellingPrice2;
            set
            {
                _sellingPrice2 = value;
                OnPropertyChanged(nameof(SellingPrice2));
            }
        }

        private decimal _sellingPrice3 = 0;
        public decimal SellingPrice3
        {
            get => _sellingPrice3;
            set
            {
                _sellingPrice3 = value;
                OnPropertyChanged(nameof(SellingPrice3));
            }
        }

        private decimal _costPrice = 0;
        public decimal CostPrice
        {
            get => _costPrice;
            set
            {
                _costPrice = value;
                OnPropertyChanged(nameof(CostPrice));
            }
        }

        private int _stockQuantity = 0;
        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                _stockQuantity = value;
                OnPropertyChanged(nameof(StockQuantity));
            }
        }

        private string _unit = "";
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged(nameof(Unit));
            }
        }

        private int _minQuantity = 0;
        public int MinQuantity
        {
            get => _minQuantity;
            set
            {
                _minQuantity = value;
                OnPropertyChanged(nameof(MinQuantity));
            }
        }

        private string _barcode = "";
        public string Barcode
        {
            get => _barcode;
            set
            {
                _barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }

        private decimal _taxRate = 0;
        public decimal TaxRate
        {
            get => _taxRate;
            set
            {
                _taxRate = value;
                OnPropertyChanged(nameof(TaxRate));
            }
        }

        private decimal _discountRate = 0;
        public decimal DiscountRate
        {
            get => _discountRate;
            set
            {
                _discountRate = value;
                OnPropertyChanged(nameof(DiscountRate));
            }
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddItemWindow()
        {
            InitializeComponent();
            DataContext = this;
            _isEditMode = false;
            LoadCategories();
            GenerateItemCode();
        }

        public AddItemWindow(Item item) : this()
        {
            _item = item;
            _isEditMode = true;
            LoadItemData();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadCategories()
        {
            try
            {
                Categories.Clear();
                var categories = DatabaseHelper.GetAllCategories();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Load Categories");
            }
        }

        private void LoadItemData()
        {
            if (_item == null) return;

            ItemCode = _item.ItemCode;
            ItemName = _item.ItemName;
            Description = _item.Description;
            SellingPrice1 = _item.SellingPrice1;
            SellingPrice2 = _item.SellingPrice2;
            SellingPrice3 = _item.SellingPrice3;
            CostPrice = _item.CostPrice;
            StockQuantity = _item.StockQuantity;
            Unit = _item.Unit;
            MinQuantity = _item.MinQuantity;
            Barcode = _item.Barcode;
            TaxRate = _item.TaxRate;
            DiscountRate = _item.DiscountRate;
            IsActive = _item.IsActive;

            // Set selected category
            if (_item.CategoryId > 0)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == _item.CategoryId);
            }
        }

        private void GenerateItemCode()
        {
            try
            {
                var lastItem = DatabaseHelper.ExecuteScalar("SELECT MAX(CAST(ItemCode AS INTEGER)) FROM Items WHERE ItemCode GLOB '[0-9]*'");
                int nextNumber = 1;
                
                if (lastItem != null && lastItem != DBNull.Value)
                {
                    nextNumber = Convert.ToInt32(lastItem) + 1;
                }

                ItemCode = nextNumber.ToString("D6"); // 6-digit code
            }
            catch (Exception ex)
            {
                ItemCode = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(ItemCode))
                {
                    MessageBox.Show("يرجى إدخال كود الصنف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ItemName))
                {
                    MessageBox.Show("يرجى إدخال اسم الصنف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedCategory == null)
                {
                    MessageBox.Show("يرجى اختيار التصنيف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SellingPrice1 <= 0)
                {
                    MessageBox.Show("يرجى إدخال سعر بيع صحيح", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StockQuantity < 0)
                {
                    MessageBox.Show("الكمية لا يمكن أن تكون سالبة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode)
                {
                    // Update existing item
                    _item.ItemCode = ItemCode;
                    _item.ItemName = ItemName;
                    _item.Description = Description;
                    _item.SellingPrice1 = SellingPrice1;
                    _item.SellingPrice2 = SellingPrice2;
                    _item.SellingPrice3 = SellingPrice3;
                    _item.CostPrice = CostPrice;
                    _item.StockQuantity = StockQuantity;
                    _item.Unit = Unit;
                    _item.MinQuantity = MinQuantity;
                    _item.Barcode = Barcode;
                    _item.TaxRate = TaxRate;
                    _item.DiscountRate = DiscountRate;
                    _item.IsActive = IsActive;
                    _item.CategoryId = SelectedCategory.Id;
                    _item.CategoryName = SelectedCategory.Name;
                    _item.UpdatedAt = DateTime.Now;

                    DatabaseHelper.UpdateItem(_item);
                    MessageBox.Show("تم تحديث الصنف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Create new item
                    var newItem = new Item
                    {
                        ItemCode = ItemCode,
                        ItemName = ItemName,
                        Description = Description,
                        SellingPrice1 = SellingPrice1,
                        SellingPrice2 = SellingPrice2,
                        SellingPrice3 = SellingPrice3,
                        CostPrice = CostPrice,
                        StockQuantity = StockQuantity,
                        Unit = Unit,
                        MinQuantity = MinQuantity,
                        Barcode = Barcode,
                        TaxRate = TaxRate,
                        DiscountRate = DiscountRate,
                        IsActive = IsActive,
                        CategoryId = SelectedCategory.Id,
                        CategoryName = SelectedCategory.Name,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    DatabaseHelper.InsertItem(newItem);
                    MessageBox.Show("تم إنشاء الصنف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save Item");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
