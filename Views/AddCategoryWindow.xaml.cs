using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ETAG_ERP.Views
{
    public partial class AddCategoryWindow : Window, INotifyPropertyChanged
    {
        private Category _category;
        private bool _isEditMode;
        private ObservableCollection<Category> _parentCategories;

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public string WindowTitle => IsEditMode ? "تعديل التصنيف" : "إضافة تصنيف جديد";

        public ObservableCollection<Category> ParentCategories
        {
            get => _parentCategories;
            set
            {
                _parentCategories = value;
                OnPropertyChanged();
            }
        }

        public AddCategoryWindow(Category category = null)
        {
            InitializeComponent();
            DataContext = this;
            
            ParentCategories = new ObservableCollection<Category>();
            LoadParentCategories();

            if (category != null)
            {
                // وضع التعديل
                IsEditMode = true;
                Category = new Category
                {
                    CategoryID = category.CategoryID,
                    Name = category.Name,
                    Description = category.Description,
                    ParentCategoryID = category.ParentCategoryID,
                    Icon = category.Icon,
                    IsActive = category.IsActive
                };
            }
            else
            {
                // وضع الإضافة
                IsEditMode = false;
                Category = new Category
                {
                    Name = "",
                    Description = "",
                    ParentCategoryID = null,
                    Icon = "📁",
                    IsActive = true
                };
            }
        }

        private void LoadParentCategories()
        {
            ParentCategories.Clear();
            
            // إضافة خيار "لا يوجد تصنيف أب"
            ParentCategories.Add(new Category 
            { 
                CategoryID = 0, 
                Name = "لا يوجد تصنيف أب" 
            });

            // تحميل التصنيفات المتاحة
            var categories = DatabaseHelper.GetAllCategories();
            foreach (var category in categories)
            {
                if (!IsEditMode || category.CategoryID != Category.CategoryID)
                {
                    ParentCategories.Add(category);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // التحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(Category.Name))
            {
                MessageBox.Show("يرجى إدخال اسم التصنيف", "خطأ", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            try
            {
                if (IsEditMode)
                {
                    // تحديث التصنيف
                    DatabaseHelper.UpdateCategory(Category);
                    MessageBox.Show("تم تحديث التصنيف بنجاح", "نجاح", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // إضافة تصنيف جديد
                    DatabaseHelper.InsertCategory(Category);
                    MessageBox.Show("تم إضافة التصنيف بنجاح", "نجاح", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ التصنيف: {ex.Message}", "خطأ", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectIcon_Click(object sender, RoutedEventArgs e)
        {
            var iconWindow = new IconSelectorWindow();
            if (iconWindow.ShowDialog() == true)
            {
                Category.Icon = iconWindow.SelectedIcon;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
