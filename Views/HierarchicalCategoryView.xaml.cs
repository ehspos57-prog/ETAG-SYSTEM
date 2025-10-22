using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class HierarchicalCategoryView : UserControl, INotifyPropertyChanged
    {
        private Category _selectedCategory;
        private Category _currentParentCategory;
        private ObservableCollection<Category> _currentCategories;
        private ObservableCollection<Category> _breadcrumbPath;

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCategorySelected));
            }
        }

        public Category CurrentParentCategory
        {
            get => _currentParentCategory;
            set
            {
                _currentParentCategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoBack));
                LoadCategories();
            }
        }

        public ObservableCollection<Category> CurrentCategories
        {
            get => _currentCategories;
            set
            {
                _currentCategories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> BreadcrumbPath
        {
            get => _breadcrumbPath;
            set
            {
                _breadcrumbPath = value;
                OnPropertyChanged();
            }
        }

        public bool IsCategorySelected => SelectedCategory != null;
        public bool CanGoBack => CurrentParentCategory != null;

        public HierarchicalCategoryView()
        {
            InitializeComponent();
            DataContext = this;
            CurrentCategories = new ObservableCollection<Category>();
            BreadcrumbPath = new ObservableCollection<Category>();
            LoadRootCategories();
        }

        private void LoadRootCategories()
        {
            CurrentParentCategory = null;
            CurrentCategories.Clear();
            BreadcrumbPath.Clear();

            // تحميل التصنيفات الجذرية
            var rootCategories = DatabaseHelper.GetCategoriesByParentId(null);
            foreach (var category in rootCategories)
            {
                CurrentCategories.Add(category);
            }
        }

        private void LoadCategories()
        {
            CurrentCategories.Clear();
            
            if (CurrentParentCategory == null)
            {
                LoadRootCategories();
                return;
            }

            // تحميل التصنيفات الفرعية
            var subCategories = DatabaseHelper.GetCategoriesByParentId(CurrentParentCategory.CategoryID);
            foreach (var category in subCategories)
            {
                CurrentCategories.Add(category);
            }

            // تحديث مسار التنقل
            UpdateBreadcrumbPath();
        }

        private void UpdateBreadcrumbPath()
        {
            BreadcrumbPath.Clear();
            
            if (CurrentParentCategory == null) return;

            // بناء مسار التنقل من الجذر إلى التصنيف الحالي
            var path = new List<Category>();
            var current = CurrentParentCategory;
            
            while (current != null)
            {
                path.Insert(0, current);
                current = DatabaseHelper.GetCategoryById(current.ParentCategoryID ?? 0);
            }

            foreach (var category in path)
            {
                BreadcrumbPath.Add(category);
            }
        }

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                SelectedCategory = category;
                
                // التحقق من وجود تصنيفات فرعية
                var hasSubCategories = DatabaseHelper.HasSubCategories(category.CategoryID);
                
                if (hasSubCategories)
                {
                    // الانتقال إلى التصنيفات الفرعية
                    CurrentParentCategory = category;
                }
                else
                {
                    // عرض الأصناف في هذا التصنيف
                    ShowItemsInCategory(category);
                }
            }
        }

        private void ShowItemsInCategory(Category category)
        {
            // فتح نافذة عرض الأصناف
            var itemsWindow = new ItemsInCategoryWindow(category);
            itemsWindow.ShowDialog();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            LoadRootCategories();
        }

        private void Breadcrumb_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Category category)
            {
                CurrentParentCategory = category;
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryWindow(CurrentParentCategory);
            if (addCategoryWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategory != null)
            {
                var editCategoryWindow = new AddCategoryWindow(SelectedCategory);
                if (editCategoryWindow.ShowDialog() == true)
                {
                    LoadCategories();
                }
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategory != null)
            {
                var result = MessageBox.Show(
                    $"هل أنت متأكد من حذف التصنيف '{SelectedCategory.Name}'؟",
                    "تأكيد الحذف",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteCategory(SelectedCategory.CategoryID);
                        LoadCategories();
                        MessageBox.Show("تم حذف التصنيف بنجاح", "نجاح", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في حذف التصنيف: {ex.Message}", "خطأ", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentParentCategory != null)
            {
                var parentId = CurrentParentCategory.ParentCategoryID;
                CurrentParentCategory = parentId.HasValue ? 
                    DatabaseHelper.GetCategoryById(parentId.Value) : null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
