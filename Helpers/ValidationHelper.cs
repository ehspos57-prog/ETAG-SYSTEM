using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;

namespace ETAG_ERP.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

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

        public static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove all non-digit characters
            var digits = Regex.Replace(phone, @"\D", "");
            
            // Check if it's a valid phone number (7-15 digits)
            return digits.Length >= 7 && digits.Length <= 15;
        }

        public static bool ValidateRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool ValidateNumber(string value)
        {
            return decimal.TryParse(value, out _);
        }

        public static bool ValidateInteger(string value)
        {
            return int.TryParse(value, out _);
        }

        public static bool ValidatePositiveNumber(decimal value)
        {
            return value > 0;
        }

        public static bool ValidateNonNegativeNumber(decimal value)
        {
            return value >= 0;
        }

        public static bool ValidateDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Password must be at least 6 characters
            return password.Length >= 6;
        }

        public static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            // Username must be 3-20 characters, alphanumeric and underscore only
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");
        }

        public static bool ValidateArabicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Check if text contains Arabic characters
            return Regex.IsMatch(text, @"[\u0600-\u06FF]");
        }

        public static bool ValidateEnglishText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Check if text contains English characters
            return Regex.IsMatch(text, @"[a-zA-Z]");
        }

        public static bool ValidateBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            // Barcode should be 8-13 digits
            return Regex.IsMatch(barcode, @"^\d{8,13}$");
        }

        public static bool ValidateTaxNumber(string taxNumber)
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
                return false;

            // Tax number should be 9-15 digits
            return Regex.IsMatch(taxNumber, @"^\d{9,15}$");
        }

        public static bool ValidateCommercialRecord(string commercialRecord)
        {
            if (string.IsNullOrWhiteSpace(commercialRecord))
                return false;

            // Commercial record should be 6-12 alphanumeric characters
            return Regex.IsMatch(commercialRecord, @"^[a-zA-Z0-9]{6,12}$");
        }

        public static void ShowValidationError(string message)
        {
            MessageBox.Show(message, "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowValidationSuccess(string message)
        {
            MessageBox.Show(message, "نجح التحقق", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool ValidateInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                ShowValidationError("فاتورة غير صحيحة");
                return false;
            }

            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
            {
                ShowValidationError("يرجى إدخال رقم الفاتورة");
                return false;
            }

            if (invoice.Client == null)
            {
                ShowValidationError("يرجى اختيار العميل");
                return false;
            }

            if (invoice.TotalAmount <= 0)
            {
                ShowValidationError("يرجى إدخال مبلغ صحيح");
                return false;
            }

            if (invoice.PaidAmount < 0)
            {
                ShowValidationError("المبلغ المدفوع لا يمكن أن يكون سالباً");
                return false;
            }

            if (invoice.PaidAmount > invoice.TotalAmount)
            {
                ShowValidationError("المبلغ المدفوع لا يمكن أن يكون أكبر من إجمالي الفاتورة");
                return false;
            }

            return true;
        }

        public static bool ValidateClient(Client client)
        {
            if (client == null)
            {
                ShowValidationError("عميل غير صحيح");
                return false;
            }

            if (string.IsNullOrWhiteSpace(client.Name))
            {
                ShowValidationError("يرجى إدخال اسم العميل");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(client.Email) && !ValidateEmail(client.Email))
            {
                ShowValidationError("يرجى إدخال بريد إلكتروني صحيح");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(client.Phone) && !ValidatePhone(client.Phone))
            {
                ShowValidationError("يرجى إدخال رقم هاتف صحيح");
                return false;
            }

            return true;
        }

        public static bool ValidateItem(Item item)
        {
            if (item == null)
            {
                ShowValidationError("صنف غير صحيح");
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.ItemName))
            {
                ShowValidationError("يرجى إدخال اسم الصنف");
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.ItemCode))
            {
                ShowValidationError("يرجى إدخال كود الصنف");
                return false;
            }

            if (item.SellingPrice1 <= 0)
            {
                ShowValidationError("يرجى إدخال سعر بيع صحيح");
                return false;
            }

            if (item.StockQuantity < 0)
            {
                ShowValidationError("الكمية في المخزون لا يمكن أن تكون سالبة");
                return false;
            }

            return true;
        }

        public static bool ValidateUser(User user)
        {
            if (user == null)
            {
                ShowValidationError("مستخدم غير صحيح");
                return false;
            }

            if (!ValidateUsername(user.Username))
            {
                ShowValidationError("اسم المستخدم يجب أن يكون 3-20 حرف (أحرف إنجليزية وأرقام و _ فقط)");
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                ShowValidationError("يرجى إدخال الاسم الكامل");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(user.Email) && !ValidateEmail(user.Email))
            {
                ShowValidationError("يرجى إدخال بريد إلكتروني صحيح");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(user.Phone) && !ValidatePhone(user.Phone))
            {
                ShowValidationError("يرجى إدخال رقم هاتف صحيح");
                return false;
            }

            return true;
        }

        public static bool ValidatePurchase(Purchase purchase)
        {
            if (purchase == null)
            {
                ShowValidationError("أمر شراء غير صحيح");
                return false;
            }

            if (string.IsNullOrWhiteSpace(purchase.PurchaseNumber))
            {
                ShowValidationError("يرجى إدخال رقم أمر الشراء");
                return false;
            }

            if (string.IsNullOrWhiteSpace(purchase.Supplier))
            {
                ShowValidationError("يرجى إدخال اسم المورد");
                return false;
            }

            if (purchase.Total <= 0)
            {
                ShowValidationError("يرجى إدخال مبلغ صحيح");
                return false;
            }

            if (purchase.Paid < 0)
            {
                ShowValidationError("المبلغ المدفوع لا يمكن أن يكون سالباً");
                return false;
            }

            if (purchase.Paid > purchase.Total)
            {
                ShowValidationError("المبلغ المدفوع لا يمكن أن يكون أكبر من إجمالي الأمر");
                return false;
            }

            return true;
        }

        public static void SetValidationError(Control control, string message)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                textBox.ToolTip = message;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                comboBox.ToolTip = message;
            }
        }

        public static void ClearValidationError(Control control)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                textBox.ToolTip = null;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                comboBox.ToolTip = null;
            }
        }

        public static bool ValidateAllControls(params Control[] controls)
        {
            bool isValid = true;

            foreach (var control in controls)
            {
                if (control is TextBox textBox)
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        SetValidationError(control, "هذا الحقل مطلوب");
                        isValid = false;
                    }
                    else
                    {
                        ClearValidationError(control);
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.SelectedItem == null)
                    {
                        SetValidationError(control, "يرجى اختيار قيمة");
                        isValid = false;
                    }
                    else
                    {
                        ClearValidationError(control);
                    }
                }
            }

            return isValid;
        }
    }
}
