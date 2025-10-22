using ETAG_ERP.Helpers;
using ETAG_ERP.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class AddUserView : Window, INotifyPropertyChanged
    {
        private User _user;
        private bool _isEditMode;

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _fullName = "";
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _phone = "";
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        private string _address = "";
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string _selectedRole = "Viewer";
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
            }
        }

        private bool _isAdmin = false;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
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

        public AddUserView()
        {
            InitializeComponent();
            DataContext = this;
            _isEditMode = false;
        }

        public AddUserView(User user) : this()
        {
            _user = user;
            _isEditMode = true;
            LoadUserData();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadUserData()
        {
            if (_user == null) return;

            Username = _user.Username;
            FullName = _user.FullName;
            Email = _user.Email;
            Phone = _user.Phone;
            Address = _user.Address;
            SelectedRole = _user.Role;
            IsAdmin = _user.IsAdmin;
            IsActive = _user.IsActive;

            // Set combo box selection
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Tag?.ToString() == _user.Role)
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Username))
                {
                    MessageBox.Show("يرجى إدخال اسم المستخدم", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(FullName))
                {
                    MessageBox.Show("يرجى إدخال الاسم الكامل", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("يرجى إدخال كلمة المرور", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!_isEditMode && PasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("كلمة المرور وتأكيدها غير متطابقتين", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
                {
                    MessageBox.Show("يرجى إدخال بريد إلكتروني صحيح", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Get selected role
                var selectedRoleItem = RoleComboBox.SelectedItem as ComboBoxItem;
                var role = selectedRoleItem?.Tag?.ToString() ?? "Viewer";

                if (_isEditMode)
                {
                    // Update existing user
                    _user.Username = Username;
                    _user.FullName = FullName;
                    _user.Email = Email;
                    _user.Phone = Phone;
                    _user.Address = Address;
                    _user.Role = role;
                    _user.IsAdmin = IsAdmin;
                    _user.IsActive = IsActive;
                    _user.UpdatedAt = DateTime.Now;

                    // Update password if provided
                    if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                    {
                        _user.PasswordHash = HashPassword(PasswordBox.Password);
                    }

                    DatabaseHelper.UpdateUser(_user);
                    MessageBox.Show("تم تحديث المستخدم بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Create new user
                    var newUser = new User
                    {
                        Username = Username,
                        FullName = FullName,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        Role = role,
                        IsAdmin = IsAdmin,
                        IsActive = IsActive,
                        PasswordHash = HashPassword(PasswordBox.Password),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    DatabaseHelper.InsertUser(newUser);
                    MessageBox.Show("تم إنشاء المستخدم بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ المستخدم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}