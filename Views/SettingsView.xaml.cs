using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ETAG_ERP.Views
{
    public partial class SettingsView : Window, INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<PermissionCheckBox> PermissionCheckBoxes { get; set; } = new ObservableCollection<PermissionCheckBox>();

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                LoadUserPermissions();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;
            LoadUsers();
            LoadSystemInfo();
            LoadSettings();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadUsers()
        {
            try
            {
                Users.Clear();
                var users = DatabaseHelper.GetAllUsers();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
                UsersCountText.Text = Users.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المستخدمين: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserPermissions()
        {
            if (SelectedUser == null) return;

            try
            {
                PermissionCheckBoxes.Clear();
                var allPermissions = Enum.GetValues<PermissionManager.Permission>();
                
                foreach (var permission in allPermissions)
                {
                    var hasPermission = PermissionManager.HasPermission(SelectedUser, permission);
                    PermissionCheckBoxes.Add(new PermissionCheckBox
                    {
                        Permission = permission,
                        DisplayName = PermissionManager.GetPermissionDisplayName(permission),
                        IsChecked = hasPermission
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل صلاحيات المستخدم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSystemInfo()
        {
            try
            {
                var systemInfo = $@"معلومات النظام:
الإصدار: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}
إطار العمل: .NET 8
نظام التشغيل: {Environment.OSVersion}
الذاكرة المتاحة: {GC.GetTotalMemory(false) / 1024 / 1024} MB
المستخدم الحالي: {Environment.UserName}
المجلد الحالي: {Environment.CurrentDirectory}
وقت التشغيل: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                SystemInfoText.Text = systemInfo;
            }
            catch (Exception ex)
            {
                SystemInfoText.Text = $"خطأ في تحميل معلومات النظام: {ex.Message}";
            }
        }

        private void LoadSettings()
        {
            try
            {
                // Load company settings
                CompanyNameTextBox.Text = DatabaseHelper.GetSetting("CompanyName") ?? "شركة ETAG";
                CompanyAddressTextBox.Text = DatabaseHelper.GetSetting("CompanyAddress") ?? "";
                CompanyPhoneTextBox.Text = DatabaseHelper.GetSetting("CompanyPhone") ?? "";
                CompanyEmailTextBox.Text = DatabaseHelper.GetSetting("CompanyEmail") ?? "";

                // Load theme settings
                var currentTheme = DatabaseHelper.GetSetting("Theme") ?? "Light";
                ThemeComboBox.SelectedIndex = currentTheme == "Dark" ? 1 : 0;

                // Load background image
                BackgroundImageTextBox.Text = DatabaseHelper.GetSetting("BackgroundImage") ?? "";

                // Load database path
                DatabasePathTextBox.Text = DatabaseHelper.GetSetting("DatabasePath") ?? "ETAG_ERP.db";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // User Management Events
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserView();
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var editUserWindow = new AddUserView(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستخدم للتعديل", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                if (selectedUser.IsAdmin)
                {
                    MessageBox.Show("لا يمكن حذف المدير الرئيسي", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"هل أنت متأكد من حذف المستخدم '{selectedUser.Username}'؟", 
                    "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteUser(selectedUser.Id);
                        LoadUsers();
                        MessageBox.Show("تم حذف المستخدم بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في حذف المستخدم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستخدم للحذف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        // Permission Management Events
        private void RefreshPermissionsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserPermissions();
        }

        // System Settings Events
        private void BrowseDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                Title = "اختر ملف قاعدة البيانات"
            };

            if (openDialog.ShowDialog() == true)
            {
                DatabasePathTextBox.Text = openDialog.FileName;
            }
        }

        private void RecreateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("هل أنت متأكد من إعادة إنشاء قاعدة البيانات؟ سيتم حذف جميع البيانات!", 
                "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DatabaseHelper.ResetDatabase();
                    MessageBox.Show("تم إعادة إنشاء قاعدة البيانات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في إعادة إنشاء قاعدة البيانات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BrowseBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                Title = "اختر صورة الخلفية"
            };

            if (openDialog.ShowDialog() == true)
            {
                BackgroundImageTextBox.Text = openDialog.FileName;
            }
        }

        private void CreateBackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "SQLite Database (*.db)|*.db",
                    FileName = $"ETAG_ERP_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db",
                    Title = "حفظ النسخة الاحتياطية"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    System.IO.File.Copy("ETAG_ERP.db", saveDialog.FileName, true);
                    MessageBox.Show("تم إنشاء النسخة الاحتياطية بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء النسخة الاحتياطية: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "SQLite Database (*.db)|*.db",
                    Title = "اختر النسخة الاحتياطية"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show("هل أنت متأكد من استعادة النسخة الاحتياطية؟ سيتم استبدال البيانات الحالية!", 
                        "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        System.IO.File.Copy(openDialog.FileName, "ETAG_ERP.db", true);
                        MessageBox.Show("تم استعادة النسخة الاحتياطية بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save company settings
                DatabaseHelper.SetSetting("CompanyName", CompanyNameTextBox.Text);
                DatabaseHelper.SetSetting("CompanyAddress", CompanyAddressTextBox.Text);
                DatabaseHelper.SetSetting("CompanyPhone", CompanyPhoneTextBox.Text);
                DatabaseHelper.SetSetting("CompanyEmail", CompanyEmailTextBox.Text);

                // Save theme settings
                var selectedTheme = ThemeComboBox.SelectedIndex == 1 ? "Dark" : "Light";
                DatabaseHelper.SetSetting("Theme", selectedTheme);

                // Save background image
                DatabaseHelper.SetSetting("BackgroundImage", BackgroundImageTextBox.Text);

                // Save database path
                DatabaseHelper.SetSetting("DatabasePath", DatabasePathTextBox.Text);

                MessageBox.Show("تم حفظ الإعدادات بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class PermissionCheckBox : INotifyPropertyChanged
    {
        private bool _isChecked;

        public PermissionManager.Permission Permission { get; set; }
        public string DisplayName { get; set; } = "";
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}