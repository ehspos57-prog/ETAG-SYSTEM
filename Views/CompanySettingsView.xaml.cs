using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ETAG_ERP.ViewModels;

namespace ETAG_ERP.Views
{
    public partial class CompanySettingsView : UserControl
    {
        public CompanySettingsView()
        {
            InitializeComponent();
            this.DataContext = new CompanySettingsViewModel();
        }

        private void BrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "اختيار شعار الشركة",
                Filter = "ملفات الصور|*.png;*.jpg;*.jpeg;*.bmp;*.gif|جميع الملفات|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.LogoPath = openFileDialog.FileName;
                }
            }
        }

        private void BrowseBackupPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "اختيار مجلد النسخ الاحتياطي",
                ShowNewFolderButton = true
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.BackupPath = folderDialog.SelectedPath;
                }
            }
        }

        private void SaveCompanyInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.SaveCompanyInfo();
                    MessageBox.Show("تم حفظ معلومات الشركة بنجاح", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ معلومات الشركة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBusinessSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.SaveBusinessSettings();
                    MessageBox.Show("تم حفظ إعدادات الأعمال بنجاح", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ إعدادات الأعمال: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveTaxSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.SaveTaxSettings();
                    MessageBox.Show("تم حفظ الإعدادات الضريبية بنجاح", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات الضريبية: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSystemSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var viewModel = DataContext as CompanySettingsViewModel;
                if (viewModel != null)
                {
                    viewModel.SaveSystemSettings();
                    MessageBox.Show("تم حفظ إعدادات النظام بنجاح", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ إعدادات النظام: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class CompanySettingsViewModel : BaseViewModel
    {
        // Company Information
        public string CompanyName { get; set; } = "ETAG للأنظمة المتقدمة";
        public string CompanyNameEn { get; set; } = "ETAG Advanced Systems";
        public string TaxNumber { get; set; } = "1234567890";
        public string CommercialRecord { get; set; } = "CR123456789";
        public string Address { get; set; } = "الرياض، المملكة العربية السعودية";
        public string Phone { get; set; } = "+966112345678";
        public string Email { get; set; } = "info@etag-systems.com";
        public string Website { get; set; } = "www.etag-systems.com";
        public string LogoPath { get; set; } = "Assets/Images/company_logo.png";
        public string Currency { get; set; } = "ريال سعودي (ر.س)";

        // Business Settings
        public string DefaultTaxRate { get; set; } = "15";
        public string InvoicePrefix { get; set; } = "INV";
        public bool AutoBackup { get; set; } = true;
        public bool EmailNotifications { get; set; } = true;
        public string BackupPath { get; set; } = "C:\\ETAG_Backup";
        public string BackupFrequency { get; set; } = "يومي";

        // Tax Settings
        public string VATRate { get; set; } = "15";
        public string IncomeTaxRate { get; set; } = "20";
        public string TaxAuthority { get; set; } = "مصلحة الضرائب المصرية";
        public string TaxYear { get; set; } = "2024";
        public bool AutoTaxCalculation { get; set; } = true;
        public bool AutoTaxReporting { get; set; } = false;
        public bool TaxPortalIntegration { get; set; } = true;

        // System Settings
        public string DefaultLanguage { get; set; } = "العربية";
        public string DefaultTheme { get; set; } = "فاتح";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "24 ساعة";
        public string SessionTimeout { get; set; } = "60";
        public string MaxLoginAttempts { get; set; } = "5";
        public bool AutoLogout { get; set; } = true;
        public bool DebugMode { get; set; } = false;

        public void SaveCompanyInfo()
        {
            // Save company information to database or configuration
            // This would typically involve database operations
            Console.WriteLine("Saving company information...");
        }

        public void SaveBusinessSettings()
        {
            // Save business settings to database or configuration
            Console.WriteLine("Saving business settings...");
        }

        public void SaveTaxSettings()
        {
            // Save tax settings to database or configuration
            Console.WriteLine("Saving tax settings...");
        }

        public void SaveSystemSettings()
        {
            // Save system settings to database or configuration
            Console.WriteLine("Saving system settings...");
        }
    }
}



