using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ETAG_ERP.Views
{
    public partial class ItemsView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        private ICollectionView _itemsView;

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemsView()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
            SetupFiltering();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadData()
        {
            try
            {
                // Load Items
                Items.Clear();
                var items = DatabaseHelper.GetAllItems();
                foreach (var item in items)
                {
                    Items.Add(item);
                }

                // Load Categories
                Categories.Clear();
                var categories = DatabaseHelper.GetAllCategories();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                // Populate Category ComboBox
                CategoryComboBox.Items.Clear();
                CategoryComboBox.Items.Add(new ComboBoxItem { Content = "جميع التصنيفات", IsSelected = true });
                foreach (var category in categories)
                {
                    CategoryComboBox.Items.Add(new ComboBoxItem { Content = category.Name, Tag = category });
                }

                UpdateSummary();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Load Items Data");
            }
        }

        private void SetupFiltering()
        {
            _itemsView = CollectionViewSource.GetDefaultView(Items);
            _itemsView.Filter = FilterPredicate;
        }

        private bool FilterPredicate(object obj)
        {
            if (obj is not Item item) return false;

            // Search filter
            var searchText = SearchTextBox?.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {
                if (!item.ItemName.ToLower().Contains(searchText) &&
                    !item.ItemCode.ToLower().Contains(searchText))
                {
                    return false;
                }
            }

            // Category filter
            var selectedCategory = CategoryComboBox?.SelectedItem as ComboBoxItem;
            if (selectedCategory?.Tag is Category category)
            {
                if (item.CategoryId != category.Id)
                {
                    return false;
                }
            }

            // Status filter
            var selectedStatus = StatusComboBox?.SelectedItem as ComboBoxItem;
            if (selectedStatus?.Content.ToString() != "جميع الحالات")
            {
                var isActive = selectedStatus?.Content.ToString() == "نشط";
                if (item.IsActive != isActive)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateSummary()
        {
            try
            {
                var totalItems = Items.Count;
                var stockValue = Items.Sum(i => i.StockQuantity * i.SellingPrice1);
                var lowStockItems = Items.Count(i => i.StockQuantity > 0 && i.StockQuantity <= 10);
                var outOfStockItems = Items.Count(i => i.StockQuantity <= 0);

                TotalItemsText.Text = totalItems.ToString();
                StockValueText.Text = stockValue.ToString("N2");
                LowStockText.Text = lowStockItems.ToString();
                OutOfStockText.Text = outOfStockItems.ToString();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Update Summary");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _itemsView?.Refresh();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _itemsView?.Refresh();
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _itemsView?.Refresh();
        }

        private void ItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addItemWindow = new AddItemWindow();
                if (addItemWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Item");
            }
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Item item)
                {
                    var editItemWindow = new AddItemWindow(item);
                    if (editItemWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Edit Item");
            }
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Item item)
                {
                    var result = MessageBox.Show($"هل أنت متأكد من حذف الصنف '{item.ItemName}'؟", 
                        "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        DatabaseHelper.DeleteItem(item.Id);
                        LoadData();
                        MessageBox.Show("تم حذف الصنف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Delete Item");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ExportItemsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"الأصناف_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show("تم تصدير الأصناف بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Export Items");
            }
        }

        private void PrintItemsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var document = GeneratePrintDocument();
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "طباعة الأصناف");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Print Items");
            }
        }

        private void ExportToExcel(string filePath)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("الأصناف");
                
                // Headers
                worksheet.Cells[1, 1].Value = "الكود";
                worksheet.Cells[1, 2].Value = "اسم الصنف";
                worksheet.Cells[1, 3].Value = "التصنيف";
                worksheet.Cells[1, 4].Value = "الكمية";
                worksheet.Cells[1, 5].Value = "سعر البيع";
                worksheet.Cells[1, 6].Value = "الوحدة";
                worksheet.Cells[1, 7].Value = "الحالة";

                // Data
                int row = 2;
                foreach (var item in Items)
                {
                    worksheet.Cells[row, 1].Value = item.ItemCode;
                    worksheet.Cells[row, 2].Value = item.ItemName;
                    worksheet.Cells[row, 3].Value = item.CategoryName;
                    worksheet.Cells[row, 4].Value = item.StockQuantity;
                    worksheet.Cells[row, 5].Value = item.SellingPrice1;
                    worksheet.Cells[row, 6].Value = item.Unit;
                    worksheet.Cells[row, 7].Value = item.IsActive ? "نشط" : "غير نشط";
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                package.SaveAs(new System.IO.FileInfo(filePath));
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
            var title = new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("قائمة الأصناف"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(title);

            // Items Table
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
            string[] headers = { "الكود", "اسم الصنف", "التصنيف", "الكمية", "سعر البيع", "الوحدة", "الحالة" };
            
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
            foreach (var item in Items)
            {
                var row = new System.Windows.Documents.TableRow();
                
                string[] values = {
                    item.ItemCode,
                    item.ItemName,
                    item.CategoryName,
                    item.StockQuantity.ToString(),
                    item.SellingPrice1.ToString("N2"),
                    item.Unit,
                    item.IsActive ? "نشط" : "غير نشط"
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
    }
}