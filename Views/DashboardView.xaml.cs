using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to new invoice creation
            MessageBox.Show("فتح شاشة إنشاء فاتورة جديدة", "فاتورة جديدة", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to new customer creation
            MessageBox.Show("فتح شاشة إضافة عميل جديد", "عميل جديد", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to new item creation
            MessageBox.Show("فتح شاشة إضافة صنف جديد", "صنف جديد", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to reports
            MessageBox.Show("فتح شاشة التقارير", "التقارير", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}