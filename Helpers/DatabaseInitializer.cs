using System;
using System.IO;
using System.Windows;

namespace ETAG_ERP.Helpers
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initialize database with all required tables and data
        /// </summary>
        public static void Initialize()
        {
            try
            {
                ErrorHandler.LogInfo("بدء تهيئة قاعدة البيانات", "Database Initializer");
                
                // Initialize database structure
                DatabaseHelper.InitializeDatabase();
                
                // Create indexes for better performance
                PerformanceOptimizer.CreateIndexes();
                
                // Seed initial data
                SeedInitialData();
                
                // Optimize database
                PerformanceOptimizer.OptimizeDatabase();
                
                ErrorHandler.LogInfo("تم تهيئة قاعدة البيانات بنجاح", "Database Initializer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Database Initialization");
                throw;
            }
        }

        /// <summary>
        /// Seed initial data
        /// </summary>
        private static void SeedInitialData()
        {
            try
            {
                // Check if data already exists
                var hasUsers = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Users");
                if (hasUsers != null && Convert.ToInt32(hasUsers) > 0)
                {
                    ErrorHandler.LogInfo("البيانات الابتدائية موجودة بالفعل", "Database Initializer");
                    return;
                }

                // Seed sample data
                SampleDataSeeder.SeedAllSampleData();
                
                // Seed categories
                SeedCategories();
                
                // Seed permissions
                SeedPermissions();
                
                ErrorHandler.LogInfo("تم إدخال البيانات الابتدائية بنجاح", "Database Initializer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Initial Data");
            }
        }

        /// <summary>
        /// Seed categories
        /// </summary>
        private static void SeedCategories()
        {
            try
            {
                var hasCategories = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Categories");
                if (hasCategories != null && Convert.ToInt32(hasCategories) > 0)
                {
                    return; // Categories already exist
                }

                var seedData = CategorySeeder.GetSeedData();
                DatabaseHelper.InsertCategoriesIfEmpty(seedData);
                
                ErrorHandler.LogInfo($"تم إدخال {seedData.Count} تصنيف", "Database Initializer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Categories");
            }
        }

        /// <summary>
        /// Seed permissions
        /// </summary>
        private static void SeedPermissions()
        {
            try
            {
                var hasPermissions = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Permissions");
                if (hasPermissions != null && Convert.ToInt32(hasPermissions) > 0)
                {
                    return; // Permissions already exist
                }

                PermissionManager.CreateDefaultRoles();
                
                ErrorHandler.LogInfo("تم إدخال الصلاحيات الابتدائية", "Database Initializer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Permissions");
            }
        }

        /// <summary>
        /// Reset database to initial state
        /// </summary>
        public static void ResetDatabase()
        {
            try
            {
                var result = MessageBox.Show(
                    "هل أنت متأكد من إعادة تعيين قاعدة البيانات؟\nسيتم حذف جميع البيانات الحالية.",
                    "تأكيد إعادة التعيين",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Backup current database
                    var backupPath = Path.Combine(FileHelper.GetApplicationDataDirectory(), 
                        $"backup_{DateTime.Now:yyyyMMddHHmmss}.db");
                    DatabaseHelper_Extensions.BackupDatabase(backupPath);
                    
                    // Reset database
                    DatabaseHelper.ResetDatabase();
                    
                    // Re-initialize
                    Initialize();
                    
                    ErrorHandler.ShowSuccess("تم إعادة تعيين قاعدة البيانات بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Reset Database");
            }
        }

        /// <summary>
        /// Backup database
        /// </summary>
        public static void BackupDatabase()
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Database files (*.db)|*.db",
                    FileName = $"ETAG_ERP_Backup_{DateTime.Now:yyyyMMddHHmmss}.db"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    DatabaseHelper_Extensions.BackupDatabase(saveFileDialog.FileName);
                    ErrorHandler.ShowSuccess("تم إنشاء نسخة احتياطية بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Backup Database");
            }
        }

        /// <summary>
        /// Restore database from backup
        /// </summary>
        public static void RestoreDatabase()
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Database files (*.db)|*.db",
                    Title = "اختر ملف النسخة الاحتياطية"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show(
                        "هل أنت متأكد من استعادة قاعدة البيانات؟\nسيتم استبدال البيانات الحالية.",
                        "تأكيد الاستعادة",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        DatabaseHelper_Extensions.RestoreDatabase(openFileDialog.FileName);
                        ErrorHandler.ShowSuccess("تم استعادة قاعدة البيانات بنجاح");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Restore Database");
            }
        }

        /// <summary>
        /// Check database integrity
        /// </summary>
        public static bool CheckDatabaseIntegrity()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    using (var command = new System.Data.SQLite.SQLiteCommand("PRAGMA integrity_check;", connection))
                    {
                        var result = command.ExecuteScalar().ToString();
                        return result == "ok";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Check Database Integrity");
                return false;
            }
        }

        /// <summary>
        /// Repair database
        /// </summary>
        public static void RepairDatabase()
        {
            try
            {
                var result = MessageBox.Show(
                    "هل تريد إصلاح قاعدة البيانات؟\nقد يستغرق هذا بعض الوقت.",
                    "تأكيد الإصلاح",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create backup first
                    var backupPath = Path.Combine(FileHelper.GetApplicationDataDirectory(), 
                        $"repair_backup_{DateTime.Now:yyyyMMddHHmmss}.db");
                    DatabaseHelper_Extensions.BackupDatabase(backupPath);
                    
                    // Vacuum database
                    PerformanceOptimizer.VacuumDatabase();
                    
                    // Recreate indexes
                    PerformanceOptimizer.CreateIndexes();
                    
                    ErrorHandler.ShowSuccess("تم إصلاح قاعدة البيانات بنجاح");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Repair Database");
            }
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        public static DatabaseStatistics GetDatabaseStatistics()
        {
            var stats = new DatabaseStatistics();
            
            try
            {
                stats.DatabaseSize = GetDatabaseSize();
                stats.TableCount = GetTableCount();
                stats.RecordCount = GetTotalRecordCount();
                stats.LastBackup = GetLastBackupDate();
                stats.IsIntegrityOk = CheckDatabaseIntegrity();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Database Statistics");
            }
            
            return stats;
        }

        /// <summary>
        /// Get database file size
        /// </summary>
        private static long GetDatabaseSize()
        {
            try
            {
                var dbPath = "ETAG_ERP.db";
                if (File.Exists(dbPath))
                {
                    return new FileInfo(dbPath).Length;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Database Size");
            }
            
            return 0;
        }

        /// <summary>
        /// Get table count
        /// </summary>
        private static int GetTableCount()
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM sqlite_master WHERE type='table'");
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Table Count");
                return 0;
            }
        }

        /// <summary>
        /// Get total record count
        /// </summary>
        private static int GetTotalRecordCount()
        {
            try
            {
                var tables = new[] { "Users", "Items", "Clients", "Invoices", "Categories", "Expenses", "Purchases" };
                int totalCount = 0;
                
                foreach (var table in tables)
                {
                    try
                    {
                        var result = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM {table}");
                        if (result != null)
                        {
                            totalCount += Convert.ToInt32(result);
                        }
                    }
                    catch
                    {
                        // Table might not exist, ignore
                    }
                }
                
                return totalCount;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Total Record Count");
                return 0;
            }
        }

        /// <summary>
        /// Get last backup date
        /// </summary>
        private static DateTime? GetLastBackupDate()
        {
            try
            {
                var backupDir = FileHelper.GetApplicationDataDirectory();
                var backupFiles = Directory.GetFiles(backupDir, "*.db")
                    .Where(f => Path.GetFileName(f).StartsWith("ETAG_ERP_Backup_"))
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .FirstOrDefault();
                
                if (backupFiles != null)
                {
                    return File.GetCreationTime(backupFiles);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Last Backup Date");
            }
            
            return null;
        }

        /// <summary>
        /// Cleanup old backups
        /// </summary>
        public static void CleanupOldBackups(int keepDays = 30)
        {
            try
            {
                var backupDir = FileHelper.GetApplicationDataDirectory();
                var backupFiles = Directory.GetFiles(backupDir, "*.db")
                    .Where(f => Path.GetFileName(f).StartsWith("ETAG_ERP_Backup_"))
                    .Where(f => File.GetCreationTime(f) < DateTime.Now.AddDays(-keepDays))
                    .ToList();
                
                foreach (var file in backupFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore individual file deletion errors
                    }
                }
                
                if (backupFiles.Count > 0)
                {
                    ErrorHandler.LogInfo($"تم حذف {backupFiles.Count} نسخة احتياطية قديمة", "Database Initializer");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Cleanup Old Backups");
            }
        }
    }

    /// <summary>
    /// Database statistics class
    /// </summary>
    public class DatabaseStatistics
    {
        public long DatabaseSize { get; set; }
        public int TableCount { get; set; }
        public int RecordCount { get; set; }
        public DateTime? LastBackup { get; set; }
        public bool IsIntegrityOk { get; set; }
        
        public string FormattedSize => FileHelper.FormatFileSize(DatabaseSize);
    }
}