using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace ETAG_ERP.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate email address
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate phone number
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            try
            {
                var phoneRegex = new Regex(@"^(\+?2)?01[0-9]{9}$");
                return phoneRegex.IsMatch(phone.Replace(" ", "").Replace("-", ""));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate Egyptian tax number
        /// </summary>
        public static bool IsValidTaxNumber(string taxNumber)
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
                return false;

            try
            {
                var taxRegex = new Regex(@"^\d{9}$");
                return taxRegex.IsMatch(taxNumber);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate commercial record number
        /// </summary>
        public static bool IsValidCommercialRecord(string commercialRecord)
        {
            if (string.IsNullOrWhiteSpace(commercialRecord))
                return false;

            try
            {
                var crRegex = new Regex(@"^CR\d{6}$");
                return crRegex.IsMatch(commercialRecord.ToUpper());
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate decimal number
        /// </summary>
        public static bool IsValidDecimal(string value, decimal minValue = 0, decimal maxValue = decimal.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                if (decimal.TryParse(value, out decimal result))
                {
                    return result >= minValue && result <= maxValue;
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return false;
        }

        /// <summary>
        /// Validate integer number
        /// </summary>
        public static bool IsValidInteger(string value, int minValue = 0, int maxValue = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                if (int.TryParse(value, out int result))
                {
                    return result >= minValue && result <= maxValue;
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return false;
        }

        /// <summary>
        /// Validate date
        /// </summary>
        public static bool IsValidDate(string value, DateTime? minDate = null, DateTime? maxDate = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    if (minDate.HasValue && result < minDate.Value)
                        return false;
                    
                    if (maxDate.HasValue && result > maxDate.Value)
                        return false;
                    
                    return true;
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return false;
        }

        /// <summary>
        /// Validate required field
        /// </summary>
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validate string length
        /// </summary>
        public static bool IsValidLength(string value, int minLength = 0, int maxLength = int.MaxValue)
        {
            if (value == null)
                return minLength == 0;

            return value.Length >= minLength && value.Length <= maxLength;
        }

        /// <summary>
        /// Validate item
        /// </summary>
        public static ValidationResult ValidateItem(Item item)
        {
            var result = new ValidationResult();

            if (!IsRequired(item.ItemName))
            {
                result.AddError("اسم الصنف مطلوب");
            }
            else if (!IsValidLength(item.ItemName, 1, 100))
            {
                result.AddError("اسم الصنف يجب أن يكون بين 1 و 100 حرف");
            }

            if (!IsRequired(item.Code))
            {
                result.AddError("كود الصنف مطلوب");
            }
            else if (!IsValidLength(item.Code, 1, 50))
            {
                result.AddError("كود الصنف يجب أن يكون بين 1 و 50 حرف");
            }

            if (item.SellingPrice <= 0)
            {
                result.AddError("سعر البيع يجب أن يكون أكبر من صفر");
            }

            if (item.PurchasePrice < 0)
            {
                result.AddError("سعر الشراء لا يمكن أن يكون سالب");
            }

            if (item.Quantity < 0)
            {
                result.AddError("الكمية لا يمكن أن تكون سالبة");
            }

            if (item.MinStock < 0)
            {
                result.AddError("الحد الأدنى للمخزون لا يمكن أن يكون سالب");
            }

            if (!string.IsNullOrEmpty(item.Email) && !IsValidEmail(item.Email))
            {
                result.AddError("البريد الإلكتروني غير صحيح");
            }

            return result;
        }

        /// <summary>
        /// Validate client
        /// </summary>
        public static ValidationResult ValidateClient(Client client)
        {
            var result = new ValidationResult();

            if (!IsRequired(client.Name))
            {
                result.AddError("اسم العميل مطلوب");
            }
            else if (!IsValidLength(client.Name, 1, 100))
            {
                result.AddError("اسم العميل يجب أن يكون بين 1 و 100 حرف");
            }

            if (!string.IsNullOrEmpty(client.Phone) && !IsValidPhoneNumber(client.Phone))
            {
                result.AddError("رقم الهاتف غير صحيح");
            }

            if (!string.IsNullOrEmpty(client.Email) && !IsValidEmail(client.Email))
            {
                result.AddError("البريد الإلكتروني غير صحيح");
            }

            if (!string.IsNullOrEmpty(client.TaxNumber) && !IsValidTaxNumber(client.TaxNumber))
            {
                result.AddError("الرقم الضريبي غير صحيح");
            }

            if (!string.IsNullOrEmpty(client.CommercialRecord) && !IsValidCommercialRecord(client.CommercialRecord))
            {
                result.AddError("رقم السجل التجاري غير صحيح");
            }

            return result;
        }

        /// <summary>
        /// Validate invoice
        /// </summary>
        public static ValidationResult ValidateInvoice(Invoice invoice)
        {
            var result = new ValidationResult();

            if (!IsRequired(invoice.InvoiceNumber))
            {
                result.AddError("رقم الفاتورة مطلوب");
            }
            else if (!IsValidLength(invoice.InvoiceNumber, 1, 50))
            {
                result.AddError("رقم الفاتورة يجب أن يكون بين 1 و 50 حرف");
            }

            if (invoice.ClientId == null || invoice.ClientId <= 0)
            {
                result.AddError("العميل مطلوب");
            }

            if (invoice.TotalAmount <= 0)
            {
                result.AddError("المبلغ الإجمالي يجب أن يكون أكبر من صفر");
            }

            if (invoice.PaidAmount < 0)
            {
                result.AddError("المبلغ المدفوع لا يمكن أن يكون سالب");
            }

            if (invoice.PaidAmount > invoice.TotalAmount)
            {
                result.AddError("المبلغ المدفوع لا يمكن أن يكون أكبر من المبلغ الإجمالي");
            }

            if (invoice.Items == null || invoice.Items.Count == 0)
            {
                result.AddError("الفاتورة يجب أن تحتوي على صنف واحد على الأقل");
            }

            return result;
        }

        /// <summary>
        /// Validate user
        /// </summary>
        public static ValidationResult ValidateUser(User user)
        {
            var result = new ValidationResult();

            if (!IsRequired(user.Username))
            {
                result.AddError("اسم المستخدم مطلوب");
            }
            else if (!IsValidLength(user.Username, 3, 50))
            {
                result.AddError("اسم المستخدم يجب أن يكون بين 3 و 50 حرف");
            }

            if (!IsRequired(user.FullName))
            {
                result.AddError("الاسم الكامل مطلوب");
            }
            else if (!IsValidLength(user.FullName, 1, 100))
            {
                result.AddError("الاسم الكامل يجب أن يكون بين 1 و 100 حرف");
            }

            if (!string.IsNullOrEmpty(user.Email) && !IsValidEmail(user.Email))
            {
                result.AddError("البريد الإلكتروني غير صحيح");
            }

            if (!string.IsNullOrEmpty(user.Phone) && !IsValidPhoneNumber(user.Phone))
            {
                result.AddError("رقم الهاتف غير صحيح");
            }

            return result;
        }

        /// <summary>
        /// Validate category
        /// </summary>
        public static ValidationResult ValidateCategory(Category category)
        {
            var result = new ValidationResult();

            if (!IsRequired(category.Name))
            {
                result.AddError("اسم التصنيف مطلوب");
            }
            else if (!IsValidLength(category.Name, 1, 100))
            {
                result.AddError("اسم التصنيف يجب أن يكون بين 1 و 100 حرف");
            }

            return result;
        }

        /// <summary>
        /// Validate expense
        /// </summary>
        public static ValidationResult ValidateExpense(Expense expense)
        {
            var result = new ValidationResult();

            if (!IsRequired(expense.Description))
            {
                result.AddError("وصف المصروف مطلوب");
            }
            else if (!IsValidLength(expense.Description, 1, 200))
            {
                result.AddError("وصف المصروف يجب أن يكون بين 1 و 200 حرف");
            }

            if (expense.Amount <= 0)
            {
                result.AddError("مبلغ المصروف يجب أن يكون أكبر من صفر");
            }

            if (expense.Date > DateTime.Now)
            {
                result.AddError("تاريخ المصروف لا يمكن أن يكون في المستقبل");
            }

            return result;
        }

        /// <summary>
        /// Validate account
        /// </summary>
        public static ValidationResult ValidateAccount(Account account)
        {
            var result = new ValidationResult();

            if (!IsRequired(account.Name))
            {
                result.AddError("اسم الحساب مطلوب");
            }
            else if (!IsValidLength(account.Name, 1, 100))
            {
                result.AddError("اسم الحساب يجب أن يكون بين 1 و 100 حرف");
            }

            if (!IsRequired(account.Type))
            {
                result.AddError("نوع الحساب مطلوب");
            }

            return result;
        }

        /// <summary>
        /// Validate employee
        /// </summary>
        public static ValidationResult ValidateEmployee(Employee employee)
        {
            var result = new ValidationResult();

            if (!IsRequired(employee.FullName))
            {
                result.AddError("الاسم الكامل مطلوب");
            }
            else if (!IsValidLength(employee.FullName, 1, 100))
            {
                result.AddError("الاسم الكامل يجب أن يكون بين 1 و 100 حرف");
            }

            if (!IsRequired(employee.JobTitle))
            {
                result.AddError("المسمى الوظيفي مطلوب");
            }
            else if (!IsValidLength(employee.JobTitle, 1, 100))
            {
                result.AddError("المسمى الوظيفي يجب أن يكون بين 1 و 100 حرف");
            }

            if (employee.Salary < 0)
            {
                result.AddError("الراتب لا يمكن أن يكون سالب");
            }

            if (!string.IsNullOrEmpty(employee.Email) && !IsValidEmail(employee.Email))
            {
                result.AddError("البريد الإلكتروني غير صحيح");
            }

            if (!string.IsNullOrEmpty(employee.Phone) && !IsValidPhoneNumber(employee.Phone))
            {
                result.AddError("رقم الهاتف غير صحيح");
            }

            return result;
        }

        /// <summary>
        /// Validate branch
        /// </summary>
        public static ValidationResult ValidateBranch(Branch branch)
        {
            var result = new ValidationResult();

            if (!IsRequired(branch.Name))
            {
                result.AddError("اسم الفرع مطلوب");
            }
            else if (!IsValidLength(branch.Name, 1, 100))
            {
                result.AddError("اسم الفرع يجب أن يكون بين 1 و 100 حرف");
            }

            if (!string.IsNullOrEmpty(branch.Phone) && !IsValidPhoneNumber(branch.Phone))
            {
                result.AddError("رقم الهاتف غير صحيح");
            }

            if (!string.IsNullOrEmpty(branch.Email) && !IsValidEmail(branch.Email))
            {
                result.AddError("البريد الإلكتروني غير صحيح");
            }

            return result;
        }

        /// <summary>
        /// Show validation errors
        /// </summary>
        public static void ShowValidationErrors(ValidationResult result)
        {
            if (result.HasErrors)
            {
                var errorMessage = string.Join("\n", result.Errors);
                MessageBox.Show(errorMessage, "أخطاء في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        public static ValidationResult ValidatePassword(string password)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.AddError("كلمة المرور مطلوبة");
                return result;
            }

            if (password.Length < 6)
            {
                result.AddError("كلمة المرور يجب أن تكون 6 أحرف على الأقل");
            }

            if (!password.Any(char.IsDigit))
            {
                result.AddError("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل");
            }

            if (!password.Any(char.IsLetter))
            {
                result.AddError("كلمة المرور يجب أن تحتوي على حرف واحد على الأقل");
            }

            return result;
        }

        /// <summary>
        /// Validate username format
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            try
            {
                var usernameRegex = new Regex(@"^[a-zA-Z0-9_]{3,50}$");
                return usernameRegex.IsMatch(username);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate barcode format
        /// </summary>
        public static bool IsValidBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            try
            {
                var barcodeRegex = new Regex(@"^\d{8,13}$");
                return barcodeRegex.IsMatch(barcode);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate URL format
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            try
            {
                var urlRegex = new Regex(@"^https?://[^\s/$.?#].[^\s]*$");
                return urlRegex.IsMatch(url);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate file extension
        /// </summary>
        public static bool IsValidFileExtension(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            try
            {
                var extension = System.IO.Path.GetExtension(fileName).ToLower();
                return allowedExtensions.Contains(extension);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate file size
        /// </summary>
        public static bool IsValidFileSize(long fileSize, long maxSizeInBytes)
        {
            return fileSize <= maxSizeInBytes;
        }

        internal static bool IsValidPhone(string phone)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Validation result class
    /// </summary>
    public class ValidationResult
    {
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        public bool HasErrors => Errors.Count > 0;
        public bool HasWarnings => Warnings.Count > 0;
        public bool IsValid => !HasErrors;

        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error) && !Errors.Contains(error))
            {
                Errors.Add(error);
            }
        }

        public void AddWarning(string warning)
        {
            if (!string.IsNullOrWhiteSpace(warning) && !Warnings.Contains(warning))
            {
                Warnings.Add(warning);
            }
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                AddError(error);
            }
        }

        public void AddWarnings(IEnumerable<string> warnings)
        {
            foreach (var warning in warnings)
            {
                AddWarning(warning);
            }
        }

        public void Clear()
        {
            Errors.Clear();
            Warnings.Clear();
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            
            if (HasErrors)
            {
                result.AppendLine("الأخطاء:");
                foreach (var error in Errors)
                {
                    result.AppendLine($"• {error}");
                }
            }

            if (HasWarnings)
            {
                if (HasErrors)
                    result.AppendLine();
                
                result.AppendLine("التحذيرات:");
                foreach (var warning in Warnings)
                {
                    result.AppendLine($"• {warning}");
                }
            }

            return result.ToString();
        }
    }
}