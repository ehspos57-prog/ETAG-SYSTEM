using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class PurchaseView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<PurchaseItemModel> PurchaseItems { get; set; } = new ObservableCollection<PurchaseItemModel>();
        public ObservableCollection<Supplier> Suppliers { get; set; } = new ObservableCollection<Supplier>();

        private string _purchaseNumber = "";
        public string PurchaseNumber
        {
            get => _purchaseNumber;
            set
            {
                _purchaseNumber = value;
                OnPropertyChanged(nameof(PurchaseNumber));
            }
        }

        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }

        private DateTime _purchaseDate = DateTime.Now;
        public DateTime PurchaseDate
        {
            get => _purchaseDate;
            set
            {
                _purchaseDate = value;
                OnPropertyChanged(nameof(PurchaseDate));
            }
        }

        private decimal _totalAmount = 0;
        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
                UpdateSummary();
            }
        }

        private decimal _paidAmount = 0;
        public decimal PaidAmount
        {
            get => _paidAmount;
            set
            {
                _paidAmount = value;
                OnPropertyChanged(nameof(PaidAmount));
                UpdateSummary();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PurchaseView()
        {
            InitializeComponent();
            DataContext = this;
            LoadSuppliers();
            GeneratePurchaseNumber();
            PurchaseItemsDataGrid.ItemsSource = PurchaseItems;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSuppliers()
        {
            try
            {
                Suppliers.Clear();
                // Note: You may need to implement GetAllSuppliers in DatabaseHelper
                // var suppliers = DatabaseHelper.GetAllSuppliers();
                // foreach (var supplier in suppliers)
                // {
                //     Suppliers.Add(supplier);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الموردين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePurchaseNumber()
        {
            try
            {
                var lastPurchase = DatabaseHelper.ExecuteScalar("SELECT MAX(CAST(PurchaseNumber AS INTEGER)) FROM Purchases WHERE PurchaseNumber GLOB '[0-9]*'");
                int nextNumber = 1;
                
                if (lastPurchase != null && lastPurchase != DBNull.Value)
                {
                    nextNumber = Convert.ToInt32(lastPurchase) + 1;
                }

                PurchaseNumber = nextNumber.ToString();
            }
            catch (Exception ex)
            {
                PurchaseNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddPurchaseItemWindow();
            if (addItemWindow.ShowDialog() == true)
            {
                var newItem = addItemWindow.SelectedItem;
                if (newItem != null)
                {
                    PurchaseItems.Add(newItem);
                    CalculateTotal();
                }
            }
        }

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PurchaseItemModel item)
            {
                PurchaseItems.Remove(item);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            TotalAmount = PurchaseItems.Sum(item => item.Total);
            TotalAmountText.Text = TotalAmount.ToString("N2");
        }

        private void UpdateSummary()
        {
            var remaining = TotalAmount - PaidAmount;
            RemainingAmountText.Text = remaining.ToString("N2");
            
            if (remaining > 0)
                RemainingAmountText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            else if (remaining < 0)
                RemainingAmountText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
            else
                RemainingAmountText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
        }

        private void PaidAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(PaidAmountTextBox.Text, out decimal paid))
            {
                PaidAmount = paid;
            }
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PurchaseItems.Count == 0)
                {
                    MessageBox.Show("يرجى إضافة أصناف للشراء", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedSupplier == null)
                {
                    MessageBox.Show("يرجى اختيار المورد", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var purchase = new Purchase
                {
                    PurchaseNumber = PurchaseNumber,
                    Supplier = SelectedSupplier.Name,
                    Date = PurchaseDate,
                    Total = TotalAmount,
                    Paid = PaidAmount,
                    Notes = "",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                DatabaseHelper.InsertPurchase(purchase);

                // Save purchase items
                foreach (var item in PurchaseItems)
                {
                    var purchaseItem = new PurchaseItem
                    {
                        PurchaseId = purchase.Id,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Total = item.Total
                    };
                    // Note: You may need to implement InsertPurchaseItem in DatabaseHelper
                    // DatabaseHelper.InsertPurchaseItem(purchaseItem);
                }

                MessageBox.Show("تم حفظ أمر الشراء بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ أمر الشراء: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var document = GeneratePrintDocument();
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "طباعة أمر الشراء");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"أمر_الشراء_{PurchaseNumber}_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show("تم تصدير أمر الشراء بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            PurchaseItems.Clear();
            TotalAmount = 0;
            PaidAmount = 0;
            GeneratePurchaseNumber();
        }

        private System.Windows.Documents.FlowDocument GeneratePrintDocument()
        {
            var doc = new System.Windows.Documents.FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new System.Windows.Media.FontFamily("Arial")
            };

            // Title
            var title = new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("أمر شراء"))
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(title);

            // Purchase Info
            var info = new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run($"رقم الأمر: {PurchaseNumber}\nالمورد: {SelectedSupplier?.Name}\nالتاريخ: {PurchaseDate:yyyy-MM-dd}"))
            {
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(info);

            // Items Table
            var table = new System.Windows.Documents.Table
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            // Add columns
            for (int i = 0; i < 4; i++)
                table.Columns.Add(new System.Windows.Documents.TableColumn());

            // Header row
            var headerRow = new System.Windows.Documents.TableRow();
            string[] headers = { "اسم الصنف", "الكمية", "سعر الوحدة", "المجموع" };
            
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
            foreach (var item in PurchaseItems)
            {
                var row = new System.Windows.Documents.TableRow();
                
                string[] values = {
                    item.ItemName,
                    item.Quantity.ToString(),
                    item.UnitPrice.ToString("N2"),
                    item.Total.ToString("N2")
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

            // Total
            var total = new System.Windows.Documents.Paragraph(
                new System.Windows.Documents.Run($"إجمالي المبلغ: {TotalAmount:N2}"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(total);

            return doc;
        }

        private void ExportToExcel(string filePath)
        {
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("أمر الشراء");
                
                // Headers
                worksheet.Cells[1, 1].Value = "رقم الأمر";
                worksheet.Cells[1, 2].Value = PurchaseNumber;
                worksheet.Cells[2, 1].Value = "المورد";
                worksheet.Cells[2, 2].Value = SelectedSupplier?.Name;
                worksheet.Cells[3, 1].Value = "التاريخ";
                worksheet.Cells[3, 2].Value = PurchaseDate.ToString("yyyy-MM-dd");

                // Items
                worksheet.Cells[5, 1].Value = "اسم الصنف";
                worksheet.Cells[5, 2].Value = "الكمية";
                worksheet.Cells[5, 3].Value = "سعر الوحدة";
                worksheet.Cells[5, 4].Value = "المجموع";

                int row = 6;
                foreach (var item in PurchaseItems)
                {
                    worksheet.Cells[row, 1].Value = item.ItemName;
                    worksheet.Cells[row, 2].Value = item.Quantity;
                    worksheet.Cells[row, 3].Value = item.UnitPrice;
                    worksheet.Cells[row, 4].Value = item.Total;
                    row++;
                }

                // Total
                worksheet.Cells[row + 1, 3].Value = "الإجمالي";
                worksheet.Cells[row + 1, 4].Value = TotalAmount;

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                package.SaveAs(new System.IO.FileInfo(filePath));
            }
        }
    }

    public class PurchaseItemModel : INotifyPropertyChanged
    {
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

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                CalculateTotal();
            }
        }

        private decimal _unitPrice = 0;
        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                CalculateTotal();
            }
        }

        private decimal _total = 0;
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateTotal()
        {
            Total = Quantity * UnitPrice;
        }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Email { get; set; } = "";
    }
}