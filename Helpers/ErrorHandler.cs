using System;
using System.Windows;
using System.IO;
using System.Text;

namespace ETAG_ERP.Helpers
{
    public static class ErrorHandler
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ErrorLog.txt");

        static ErrorHandler()
        {
            // Ensure logs directory exists
            var logsDir = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }
        }

        public static void HandleException(Exception ex, string context = "")
        {
            try
            {
                // Log the exception
                LogException(ex, context);

                // Show user-friendly message
                ShowUserFriendlyError(ex, context);
            }
            catch (Exception logEx)
            {
                // If logging fails, show basic error
                MessageBox.Show($"حدث خطأ في النظام: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void HandleDatabaseException(Exception ex, string operation = "")
        {
            var context = $"Database Operation: {operation}";
            HandleException(ex, context);
        }

        public static void HandleUIException(Exception ex, string controlName = "")
        {
            var context = $"UI Control: {controlName}";
            HandleException(ex, context);
        }

        public static void HandleFileException(Exception ex, string fileName = "")
        {
            var context = $"File Operation: {fileName}";
            HandleException(ex, context);
        }

        public static void HandleNetworkException(Exception ex, string operation = "")
        {
            var context = $"Network Operation: {operation}";
            HandleException(ex, context);
        }

        private static void LogException(Exception ex, string context)
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"=== ERROR LOG - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                logEntry.AppendLine($"Context: {context}");
                logEntry.AppendLine($"Exception Type: {ex.GetType().Name}");
                logEntry.AppendLine($"Message: {ex.Message}");
                logEntry.AppendLine($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    logEntry.AppendLine($"Inner Exception: {ex.InnerException.Message}");
                    logEntry.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
                
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine();

                File.AppendAllText(LogFilePath, logEntry.ToString(), Encoding.UTF8);
            }
            catch
            {
                // If logging fails, ignore to prevent infinite loop
            }
        }

        private static void ShowUserFriendlyError(Exception ex, string context)
        {
            string userMessage = GetUserFriendlyMessage(ex, context);
            
            MessageBox.Show(userMessage, "خطأ في النظام", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static string GetUserFriendlyMessage(Exception ex, string context)
        {
            return ex switch
            {
                System.Data.SQLite.SQLiteException sqlEx => GetSQLiteErrorMessage(sqlEx),
                System.IO.FileNotFoundException => "الملف المطلوب غير موجود. يرجى التحقق من المسار.",
                System.IO.DirectoryNotFoundException => "المجلد المطلوب غير موجود. يرجى التحقق من المسار.",
                System.IO.IOException ioEx => "خطأ في الوصول للملف. تأكد من أن الملف غير مفتوح في برنامج آخر.",
                System.UnauthorizedAccessException => "ليس لديك صلاحية للوصول لهذا الملف أو المجلد.",
                System.OutOfMemoryException => "الذاكرة غير كافية. يرجى إغلاق بعض البرامج والمحاولة مرة أخرى.",
                System.ArgumentNullException => "قيمة مطلوبة مفقودة. يرجى ملء جميع الحقول المطلوبة.",
                System.ArgumentException => "البيانات المدخلة غير صحيحة. يرجى التحقق من القيم المدخلة.",
                System.InvalidOperationException => "عملية غير صحيحة. يرجى المحاولة مرة أخرى.",
                System.NotSupportedException => "هذه العملية غير مدعومة في النظام الحالي.",
                System.TimeoutException => "انتهت مهلة العملية. يرجى المحاولة مرة أخرى.",
                System.Net.NetworkInformation.NetworkInformationException => "خطأ في الاتصال بالشبكة. تأكد من اتصال الإنترنت.",
                _ => $"حدث خطأ غير متوقع: {ex.Message}"
            };

        }

        private static string GetSQLiteErrorMessage(System.Data.SQLite.SQLiteException ex)
        {
            return ex.ResultCode switch
            {
                System.Data.SQLite.SQLiteErrorCode.Constraint => "انتهاك قيود قاعدة البيانات. تأكد من صحة البيانات المدخلة.",
                System.Data.SQLite.SQLiteErrorCode.Busy => "قاعدة البيانات مشغولة. يرجى المحاولة مرة أخرى.",
                System.Data.SQLite.SQLiteErrorCode.Locked => "قاعدة البيانات مقفلة. تأكد من إغلاق جميع النوافذ الأخرى.",
                System.Data.SQLite.SQLiteErrorCode.Corrupt => "قاعدة البيانات تالفة. يرجى استعادة نسخة احتياطية.",
                System.Data.SQLite.SQLiteErrorCode.Full => "قاعدة البيانات ممتلئة. يرجى تنظيف البيانات القديمة.",
                System.Data.SQLite.SQLiteErrorCode.CantOpen => "لا يمكن فتح قاعدة البيانات. تأكد من الصلاحيات.",
                System.Data.SQLite.SQLiteErrorCode.ReadOnly => "قاعدة البيانات للقراءة فقط. لا يمكن التعديل.",
                _ => $"خطأ في قاعدة البيانات: {ex.Message}"
            };
        }

        public static void ShowWarning(string message, string title = "تحذير")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowInfo(string message, string title = "معلومات")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowSuccess(string message, string title = "نجح")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool ShowConfirmation(string message, string title = "تأكيد")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static void LogInfo(string message, string context = "")
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"=== INFO LOG - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                logEntry.AppendLine($"Context: {context}");
                logEntry.AppendLine($"Message: {message}");
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine();

                File.AppendAllText(LogFilePath, logEntry.ToString(), Encoding.UTF8);
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        public static void LogWarning(string message, string context = "")
        {
            try
            {
                var logEntry = new StringBuilder();
                logEntry.AppendLine($"=== WARNING LOG - {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
                logEntry.AppendLine($"Context: {context}");
                logEntry.AppendLine($"Message: {message}");
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine();

                File.AppendAllText(LogFilePath, logEntry.ToString(), Encoding.UTF8);
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        public static void ClearLogs()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    File.Delete(LogFilePath);
                }
            }
            catch
            {
                // If clearing fails, ignore
            }
        }

        public static string GetLogContent()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    return File.ReadAllText(LogFilePath, Encoding.UTF8);
                }
            }
            catch
            {
                // If reading fails, return empty
            }
            
            return "لا توجد سجلات خطأ";
        }

        public static void ExportLogs(string filePath)
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    File.Copy(LogFilePath, filePath, true);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Export Logs");
            }
        }

        public static void HandleApplicationStartup()
        {
            try
            {
                LogInfo("Application Started", "Startup");
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        public static void HandleApplicationShutdown()
        {
            try
            {
                LogInfo("Application Shutdown", "Shutdown");
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        public static void HandleUserAction(string action, string details = "")
        {
            try
            {
                LogInfo($"User Action: {action}", details);
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        public static void HandleDatabaseOperation(string operation, bool success, string details = "")
        {
            try
            {
                var message = $"Database Operation: {operation} - {(success ? "Success" : "Failed")}";
                if (success)
                {
                    LogInfo(message, details);
                }
                else
                {
                    LogWarning(message, details);
                }
            }
            catch
            {
                // If logging fails, ignore
            }
        }

        internal static void Initialize()
        {
            throw new NotImplementedException();
        }

        internal static void LogError(Exception ex, string v)
        {
            throw new NotImplementedException();
        }

        internal static void LogError(string v, string v1)
        {
            throw new NotImplementedException();
        }

        internal static void LogError(string v)
        {
            throw new NotImplementedException();
        }
    }
}
