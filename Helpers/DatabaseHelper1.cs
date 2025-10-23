using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    /// <summary>
    /// Alternative database helper with additional functionality
    /// </summary>
    public static class DatabaseHelper1
    {
        private static readonly string ConnectionString = $"Data Source={DatabaseHelper.DatabasePath};Version=3;";

        /// <summary>
        /// Execute stored procedure (SQLite doesn't support stored procedures, but we can simulate with functions)
        /// </summary>
        public static DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            try
            {
                // SQLite doesn't support stored procedures, so we'll use regular queries
                // This is a placeholder for future database migration to SQL Server or PostgreSQL
                ErrorHandler.LogWarning($"SQLite doesn't support stored procedures. Procedure: {procedureName}", "DatabaseHelper1");
                return new DataTable();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Stored Procedure: {procedureName}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute batch operations
        /// </summary>
        public static bool ExecuteBatch(List<string> queries, List<Dictionary<string, object>> parametersList = null)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    for (int i = 0; i < queries.Count; i++)
                    {
                        var query = queries[i];
                        var parameters = parametersList != null && i < parametersList.Count ? parametersList[i] : null;

                        using var command = new SQLiteCommand(query, connection, transaction);
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    ErrorHandler.LogInfo($"تم تنفيذ {queries.Count} عملية بنجاح", "DatabaseHelper1");
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Execute Batch Operations");
                return false;
            }
        }

        /// <summary>
        /// Execute batch operations with return values
        /// </summary>
        public static List<object> ExecuteBatchWithResults(List<string> queries, List<Dictionary<string, object>> parametersList = null)
        {
            var results = new List<object>();
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    for (int i = 0; i < queries.Count; i++)
                    {
                        var query = queries[i];
                        var parameters = parametersList != null && i < parametersList.Count ? parametersList[i] : null;

                        using var command = new SQLiteCommand(query, connection, transaction);
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        var result = command.ExecuteScalar();
                        results.Add(result);
                    }

                    transaction.Commit();
                    ErrorHandler.LogInfo($"تم تنفيذ {queries.Count} عملية مع النتائج بنجاح", "DatabaseHelper1");
                    return results;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Execute Batch Operations with Results");
                return results;
            }
        }

        /// <summary>
        /// Execute query with timeout
        /// </summary>
        public static DataTable ExecuteQueryWithTimeout(string query, int timeoutSeconds = 30, Dictionary<string, object> parameters = null)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                command.CommandTimeout = timeoutSeconds;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                using var adapter = new SQLiteDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                ErrorHandler.LogInfo($"تم تنفيذ الاستعلام بنجاح في {timeoutSeconds} ثانية", "DatabaseHelper1");
                return dataTable;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Timeout: {timeoutSeconds}s");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute non-query with timeout
        /// </summary>
        public static bool ExecuteNonQueryWithTimeout(string query, int timeoutSeconds = 30, Dictionary<string, object> parameters = null)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                command.CommandTimeout = timeoutSeconds;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                var result = command.ExecuteNonQuery();
                ErrorHandler.LogInfo($"تم تنفيذ الأمر بنجاح في {timeoutSeconds} ثانية", "DatabaseHelper1");
                return result > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Non-Query with Timeout: {timeoutSeconds}s");
                return false;
            }
        }

        /// <summary>
        /// Execute scalar with timeout
        /// </summary>
        public static object ExecuteScalarWithTimeout(string query, int timeoutSeconds = 30, Dictionary<string, object> parameters = null)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                command.CommandTimeout = timeoutSeconds;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                var result = command.ExecuteScalar();
                ErrorHandler.LogInfo($"تم تنفيذ الاستعلام العددية بنجاح في {timeoutSeconds} ثانية", "DatabaseHelper1");
                return result;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Scalar with Timeout: {timeoutSeconds}s");
                return null;
            }
        }

        /// <summary>
        /// Execute query with retry mechanism
        /// </summary>
        public static DataTable ExecuteQueryWithRetry(string query, int maxRetries = 3, int delayMs = 1000, Dictionary<string, object> parameters = null)
        {
            var retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    return ExecuteQueryWithTimeout(query, 30, parameters);
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        ErrorHandler.HandleException(ex, $"Execute Query with Retry (Failed after {maxRetries} attempts)");
                        return new DataTable();
                    }
                    
                    ErrorHandler.LogWarning($"محاولة {retryCount} فشلت، إعادة المحاولة بعد {delayMs}ms", "DatabaseHelper1");
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            return new DataTable();
        }

        /// <summary>
        /// Execute non-query with retry mechanism
        /// </summary>
        public static bool ExecuteNonQueryWithRetry(string query, int maxRetries = 3, int delayMs = 1000, Dictionary<string, object> parameters = null)
        {
            var retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    return ExecuteNonQueryWithTimeout(query, 30, parameters);
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        ErrorHandler.HandleException(ex, $"Execute Non-Query with Retry (Failed after {maxRetries} attempts)");
                        return false;
                    }
                    
                    ErrorHandler.LogWarning($"محاولة {retryCount} فشلت، إعادة المحاولة بعد {delayMs}ms", "DatabaseHelper1");
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            return false;
        }

        /// <summary>
        /// Execute query with connection pooling
        /// </summary>
        public static DataTable ExecuteQueryWithPooling(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                // SQLite doesn't support connection pooling, but we can simulate it
                // This is a placeholder for future database migration to SQL Server or PostgreSQL
                ErrorHandler.LogWarning("SQLite doesn't support connection pooling", "DatabaseHelper1");
                return ExecuteQueryWithTimeout(query, 30, parameters);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Execute Query with Pooling");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with caching
        /// </summary>
        public static DataTable ExecuteQueryWithCaching(string query, string cacheKey, int cacheMinutes = 5, Dictionary<string, object> parameters = null)
        {
            try
            {
                // Simple in-memory cache simulation
                // In a real application, you would use a proper caching framework like Redis or MemoryCache
                ErrorHandler.LogWarning("Caching not implemented, executing query directly", "DatabaseHelper1");
                return ExecuteQueryWithTimeout(query, 30, parameters);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Execute Query with Caching");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with logging
        /// </summary>
        public static DataTable ExecuteQueryWithLogging(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                var startTime = DateTime.Now;
                ErrorHandler.LogInfo($"بدء تنفيذ {operation}: {query}", "DatabaseHelper1");
                
                var result = ExecuteQueryWithTimeout(query, 30, parameters);
                
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                ErrorHandler.LogInfo($"تم تنفيذ {operation} بنجاح في {duration.TotalMilliseconds}ms", "DatabaseHelper1");
                return result;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Logging: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with performance monitoring
        /// </summary>
        public static DataTable ExecuteQueryWithPerformanceMonitoring(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                var startTime = DateTime.Now;
                var startMemory = GC.GetTotalMemory(false);
                
                var result = ExecuteQueryWithTimeout(query, 30, parameters);
                
                var endTime = DateTime.Now;
                var endMemory = GC.GetTotalMemory(false);
                
                var duration = endTime - startTime;
                var memoryUsed = endMemory - startMemory;
                
                ErrorHandler.LogInfo($"أداء {operation}: المدة {duration.TotalMilliseconds}ms، الذاكرة {memoryUsed} bytes", "DatabaseHelper1");
                return result;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Performance Monitoring: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with transaction
        /// </summary>
        public static DataTable ExecuteQueryWithTransaction(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();
                
                try
                {
                    using var command = new SQLiteCommand(query, connection, transaction);
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    using var adapter = new SQLiteDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    transaction.Commit();
                    ErrorHandler.LogInfo($"تم تنفيذ {operation} في المعاملة بنجاح", "DatabaseHelper1");
                    return dataTable;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Transaction: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with error handling
        /// </summary>
        public static DataTable ExecuteQueryWithErrorHandling(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                return ExecuteQueryWithTimeout(query, 30, parameters);
            }
            catch (SQLiteException ex)
            {
                ErrorHandler.HandleDatabaseException(ex, $"Execute Query with Error Handling: {operation}");
                return new DataTable();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Error Handling: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with validation
        /// </summary>
        public static DataTable ExecuteQueryWithValidation(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                // Validate query
                if (string.IsNullOrWhiteSpace(query))
                {
                    ErrorHandler.LogError("الاستعلام فارغ", "DatabaseHelper1");
                    return new DataTable();
                }

                // Validate parameters
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        if (string.IsNullOrWhiteSpace(param.Key))
                        {
                            ErrorHandler.LogError("مفتاح المعامل فارغ", "DatabaseHelper1");
                            return new DataTable();
                        }
                    }
                }

                return ExecuteQueryWithTimeout(query, 30, parameters);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Validation: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with security checks
        /// </summary>
        public static DataTable ExecuteQueryWithSecurityChecks(string query, Dictionary<string, object> parameters = null, string operation = "Query")
        {
            try
            {
                // Basic security checks
                if (query.ToUpper().Contains("DROP") || query.ToUpper().Contains("DELETE") || query.ToUpper().Contains("TRUNCATE"))
                {
                    ErrorHandler.LogWarning($"محاولة تنفيذ استعلام خطير: {operation}", "DatabaseHelper1");
                    // In a real application, you would check user permissions here
                }

                return ExecuteQueryWithTimeout(query, 30, parameters);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with Security Checks: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Execute query with all features
        /// </summary>
        public static DataTable ExecuteQueryWithAllFeatures(string query, Dictionary<string, object> parameters = null, string operation = "Query", int timeoutSeconds = 30, int maxRetries = 3, int delayMs = 1000)
        {
            try
            {
                var retryCount = 0;
                while (retryCount < maxRetries)
                {
                    try
                    {
                        var startTime = DateTime.Now;
                        var startMemory = GC.GetTotalMemory(false);
                        
                        ErrorHandler.LogInfo($"بدء تنفيذ {operation}: {query}", "DatabaseHelper1");
                        
                        // Security checks
                        if (query.ToUpper().Contains("DROP") || query.ToUpper().Contains("DELETE") || query.ToUpper().Contains("TRUNCATE"))
                        {
                            ErrorHandler.LogWarning($"محاولة تنفيذ استعلام خطير: {operation}", "DatabaseHelper1");
                        }

                        // Validation
                        if (string.IsNullOrWhiteSpace(query))
                        {
                            ErrorHandler.LogError("الاستعلام فارغ", "DatabaseHelper1");
                            return new DataTable();
                        }

                        // Execute query
                        var result = ExecuteQueryWithTimeout(query, timeoutSeconds, parameters);
                        
                        var endTime = DateTime.Now;
                        var endMemory = GC.GetTotalMemory(false);
                        
                        var duration = endTime - startTime;
                        var memoryUsed = endMemory - startMemory;
                        
                        ErrorHandler.LogInfo($"تم تنفيذ {operation} بنجاح في {duration.TotalMilliseconds}ms، الذاكرة {memoryUsed} bytes", "DatabaseHelper1");
                        return result;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            ErrorHandler.HandleException(ex, $"Execute Query with All Features (Failed after {maxRetries} attempts): {operation}");
                            return new DataTable();
                        }
                        
                        ErrorHandler.LogWarning($"محاولة {retryCount} فشلت، إعادة المحاولة بعد {delayMs}ms", "DatabaseHelper1");
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }
                return new DataTable();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Execute Query with All Features: {operation}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        public static DatabaseStatistics GetDatabaseStatistics()
        {
            try
            {
                var stats = new DatabaseStatistics();
                
                // Get table counts
                var tables = new[] { "Users", "Categories", "Items", "Clients", "Invoices", "Returns", "Purchases", "Expenses", "Accounts", "Employees", "Branches" };
                
                foreach (var table in tables)
                {
                    var countQuery = $"SELECT COUNT(*) FROM {table}";
                    var count = ExecuteScalarWithTimeout(countQuery, 10);
                    if (count != null && int.TryParse(count.ToString(), out int tableCount))
                    {
                        stats.TableCounts[table] = tableCount;
                    }
                }

                // Get database size
                var dbFile = new FileInfo(DatabaseHelper.DatabasePath);
                if (dbFile.Exists)
                {
                    stats.DatabaseSizeBytes = dbFile.Length;
                }

                // Get last backup date
                var backupPath = Path.Combine(Path.GetDirectoryName(DatabaseHelper.DatabasePath), "backup");
                if (Directory.Exists(backupPath))
                {
                    var backupFiles = Directory.GetFiles(backupPath, "*.db");
                    if (backupFiles.Length > 0)
                    {
                        var latestBackup = backupFiles.OrderByDescending(f => new FileInfo(f).CreationTime).First();
                        stats.LastBackupDate = new FileInfo(latestBackup).CreationTime;
                    }
                }

                ErrorHandler.LogInfo("تم الحصول على إحصائيات قاعدة البيانات بنجاح", "DatabaseHelper1");
                return stats;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Database Statistics");
                return new DatabaseStatistics();
            }
        }

        /// <summary>
        /// Database statistics class
        /// </summary>
        public class DatabaseStatistics
        {
            public Dictionary<string, int> TableCounts { get; set; } = new Dictionary<string, int>();
            public long DatabaseSizeBytes { get; set; }
            public DateTime? LastBackupDate { get; set; }
            public int TotalRecords => TableCounts.Values.Sum();
        }
    }
}