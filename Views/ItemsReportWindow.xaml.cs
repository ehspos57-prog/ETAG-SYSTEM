using ETAG_ERP.Models;
using ETAG_ERP.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ETAG_ERP.Views
{
    public partial class ItemsReportWindow : Window, INotifyPropertyChanged
    {
        private Category _category;
        private List<Item> _items;
        private ObservableCollection<ItemReportModel> _reportItems;

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ReportTitle));
            }
        }

        public string ReportTitle => $"تقرير الأصناف - {Category?.Name}";

        public List<Item> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
                GenerateReport();
            }
        }

        public ObservableCollection<ItemReportModel> ReportItems
        {
            get => _reportItems;
            set
            {
                _reportItems = value;
                OnPropertyChanged();
            }
        }

        public int ItemsCount => ReportItems?.Count ?? 0;
        public decimal TotalValue => ReportItems?.Sum(x => x.TotalValue) ?? 0;
        public decimal AveragePrice => ItemsCount > 0 ? TotalValue / ItemsCount : 0;

        public ItemsReportWindow(Category category, List<Item> items)
        {
            InitializeComponent();
            DataContext = this;
            
            Category = category;
            Items = items;
            ReportItems = new ObservableCollection<ItemReportModel>();
        }

        private void GenerateReport()
        {
            ReportItems.Clear();
            
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    var reportItem = new ItemReportModel
                    {
                        ItemID = item.ItemID,
                        ItemCode = item.ItemCode,
                        Name = item.Name,
                        Description = item.Description,
                        SellPrice = item.SellPrice,
                        CostPrice = item.CostPrice,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        TotalValue = item.Quantity * item.SellPrice
                    };
                    
                    ReportItems.Add(reportItem);
                }
            }
            
            ReportDataGrid.ItemsSource = ReportItems;
            OnPropertyChanged(nameof(ItemsCount));
            OnPropertyChanged(nameof(TotalValue));
            OnPropertyChanged(nameof(AveragePrice));
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // إنشاء مستند للطباعة
                    var document = CreatePrintDocument();
                    printDialog.PrintDocument(document.DocumentPaginator, "تقرير الأصناف");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة: {ex.Message}", "خطأ", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"تقرير_الأصناف_{Category?.Name}_{DateTime.Now:yyyyMMdd}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // تصدير إلى PDF
                    ExportHelper.ExportItemsToPdf(ReportItems.ToList(), saveDialog.FileName, Category);
                    MessageBox.Show("تم تصدير التقرير إلى PDF بنجاح", "نجاح", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير PDF: {ex.Message}", "خطأ", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"تقرير_الأصناف_{Category?.Name}_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // تصدير إلى Excel
                    ExportHelper.ExportItemsToExcel(ReportItems.ToList(), saveDialog.FileName, Category);
                    MessageBox.Show("تم تصدير التقرير إلى Excel بنجاح", "نجاح", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تصدير Excel: {ex.Message}", "خطأ", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private FlowDocument CreatePrintDocument()
        {
            var document = new FlowDocument();
            
            // العنوان
            var title = new Paragraph(new Run($"تقرير الأصناف - {Category?.Name}"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            document.Blocks.Add(title);

            // معلومات التقرير
            var info = new Paragraph();
            info.Inlines.Add(new Run($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}"));
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"عدد الأصناف: {ItemsCount}"));
            info.Inlines.Add(new LineBreak());
            info.Inlines.Add(new Run($"القيمة الإجمالية: {TotalValue:C}"));
            document.Blocks.Add(info);

            // جدول البيانات
            var table = new Table();
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(200) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الكود"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("اسم الصنف"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("سعر البيع"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("الكمية"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("القيمة"))) { FontWeight = FontWeights.Bold });
            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(headerRow);

            foreach (var item in ReportItems)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.ItemCode))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Name))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.SellPrice.ToString("C")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.Quantity.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(item.TotalValue.ToString("C")))));
                table.RowGroups[0].Rows.Add(row);
            }

            document.Blocks.Add(table);
            return document;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ItemReportModel
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal SellPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "";
        public decimal TotalValue { get; set; }
    }
}
