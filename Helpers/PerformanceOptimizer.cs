using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ETAG_ERP.Helpers
{
    public static class PerformanceOptimizer
    {
        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private static readonly Dictionary<string, DateTime> _cacheTimestamps = new Dictionary<string, DateTime>();
        private static readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// Initialize performance optimizer
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // Start cache cleanup task
                Task.Run(CleanupCache);
                
                // Optimize database
                OptimizeDatabase();
                
                ErrorHandler.LogInfo("Performance Optimizer initialized", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initialize Performance Optimizer");
            }
        }

        /// <summary>
        /// Optimize database performance
        /// </summary>
        public static void OptimizeDatabase()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    // Enable WAL mode for better concurrency
                    using (var command = new SQLiteCommand("PRAGMA journal_mode=WAL;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Set synchronous mode to NORMAL for better performance
                    using (var command = new SQLiteCommand("PRAGMA synchronous=NORMAL;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Set cache size to 10MB
                    using (var command = new SQLiteCommand("PRAGMA cache_size=10000;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Set temp store to memory
                    using (var command = new SQLiteCommand("PRAGMA temp_store=MEMORY;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    // Analyze database for query optimization
                    using (var command = new SQLiteCommand("ANALYZE;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                
                ErrorHandler.LogInfo("Database optimized successfully", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Optimize Database");
            }
        }

        /// <summary>
        /// Cache data with expiration
        /// </summary>
        public static void CacheData<T>(string key, T data, TimeSpan? expiration = null)
        {
            try
            {
                lock (_cacheLock)
                {
                    _cache[key] = data;
                    _cacheTimestamps[key] = DateTime.Now.Add(expiration ?? _cacheExpiration);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Cache Data");
            }
        }

        /// <summary>
        /// Get cached data
        /// </summary>
        public static T? GetCachedData<T>(string key)
        {
            try
            {
                lock (_cacheLock)
                {
                    if (_cache.ContainsKey(key) && _cacheTimestamps.ContainsKey(key))
                    {
                        if (DateTime.Now < _cacheTimestamps[key])
                        {
                            return (T)_cache[key];
                        }
                        else
                        {
                            // Cache expired, remove it
                            _cache.Remove(key);
                            _cacheTimestamps.Remove(key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Cached Data");
            }

            return default(T);
        }

        /// <summary>
        /// Remove cached data
        /// </summary>
        public static void RemoveCachedData(string key)
        {
            try
            {
                lock (_cacheLock)
                {
                    _cache.Remove(key);
                    _cacheTimestamps.Remove(key);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Remove Cached Data");
            }
        }

        /// <summary>
        /// Clear all cached data
        /// </summary>
        public static void ClearCache()
        {
            try
            {
                lock (_cacheLock)
                {
                    _cache.Clear();
                    _cacheTimestamps.Clear();
                }
                
                ErrorHandler.LogInfo("Cache cleared", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Clear Cache");
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public static CacheStatistics GetCacheStatistics()
        {
            try
            {
                lock (_cacheLock)
                {
                    var expiredCount = _cacheTimestamps.Count(kvp => DateTime.Now >= kvp.Value);
                    var validCount = _cache.Count - expiredCount;
                    
                    return new CacheStatistics
                    {
                        TotalItems = _cache.Count,
                        ValidItems = validCount,
                        ExpiredItems = expiredCount,
                        MemoryUsage = _cache.Count * 100 // Rough estimate
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Cache Statistics");
                return new CacheStatistics();
            }
        }

        /// <summary>
        /// Cleanup expired cache entries
        /// </summary>
        private static void CleanupCache()
        {
            while (true)
            {
                try
                {
                    Task.Delay(TimeSpan.FromMinutes(1)).Wait();
                    
                    lock (_cacheLock)
                    {
                        var expiredKeys = _cacheTimestamps
                            .Where(kvp => DateTime.Now >= kvp.Value)
                            .Select(kvp => kvp.Key)
                            .ToList();
                        
                        foreach (var key in expiredKeys)
                        {
                            _cache.Remove(key);
                            _cacheTimestamps.Remove(key);
                        }
                        
                        if (expiredKeys.Count > 0)
                        {
                            ErrorHandler.LogInfo($"Cleaned up {expiredKeys.Count} expired cache entries", "Performance Optimizer");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleException(ex, "Cleanup Cache");
                }
            }
        }

        /// <summary>
        /// Measure execution time
        /// </summary>
        public static T MeasureExecutionTime<T>(Func<T> action, string operationName)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = action();
                stopwatch.Stop();
                
                ErrorHandler.LogInfo($"Operation '{operationName}' completed in {stopwatch.ElapsedMilliseconds}ms", "Performance Optimizer");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                ErrorHandler.HandleException(ex, $"Operation '{operationName}' failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }

        /// <summary>
        /// Measure execution time for async operations
        /// </summary>
        public static async Task<T> MeasureExecutionTimeAsync<T>(Func<Task<T>> action, string operationName)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = await action();
                stopwatch.Stop();
                
                ErrorHandler.LogInfo($"Async operation '{operationName}' completed in {stopwatch.ElapsedMilliseconds}ms", "Performance Optimizer");
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                ErrorHandler.HandleException(ex, $"Async operation '{operationName}' failed after {stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }

        /// <summary>
        /// Optimize query performance
        /// </summary>
        public static DataTable OptimizedQuery(string sql, params SQLiteParameter[] parameters)
        {
            return MeasureExecutionTime(() =>
            {
                var cacheKey = $"query_{sql}_{string.Join(",", parameters.Select(p => $"{p.ParameterName}={p.Value}"))}";
                var cachedResult = GetCachedData<DataTable>(cacheKey);
                
                if (cachedResult != null)
                {
                    return cachedResult;
                }
                
                var result = DatabaseHelper.GetDataTable(sql, parameters);
                CacheData(cacheKey, result, TimeSpan.FromMinutes(2));
                
                return result;
            }, $"Query: {sql.Substring(0, Math.Min(50, sql.Length))}...");
        }

        /// <summary>
        /// Optimize scalar query performance
        /// </summary>
        public static object OptimizedScalarQuery(string sql, params SQLiteParameter[] parameters)
        {
            return MeasureExecutionTime(() =>
            {
                var cacheKey = $"scalar_{sql}_{string.Join(",", parameters.Select(p => $"{p.ParameterName}={p.Value}"))}";
                var cachedResult = GetCachedData<object>(cacheKey);
                
                if (cachedResult != null)
                {
                    return cachedResult;
                }
                
                var result = DatabaseHelper.ExecuteScalar(sql, parameters);
                CacheData(cacheKey, result, TimeSpan.FromMinutes(1));
                
                return result;
            }, $"Scalar Query: {sql.Substring(0, Math.Min(50, sql.Length))}...");
        }

        /// <summary>
        /// Batch insert for better performance
        /// </summary>
        public static void BatchInsert<T>(string tableName, List<T> items, Func<T, Dictionary<string, object>> mapper)
        {
            MeasureExecutionTime(() =>
            {
                if (items == null || items.Count == 0)
                    return;

                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in items)
                            {
                                var data = mapper(item);
                                var columns = string.Join(", ", data.Keys);
                                var values = string.Join(", ", data.Keys.Select(k => $"@{k}"));
                                var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                                
                                using (var command = new SQLiteCommand(sql, connection, transaction))
                                {
                                    foreach (var kvp in data)
                                    {
                                        command.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                                    }
                                    command.ExecuteNonQuery();
                                }
                            }
                            
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                
                // Clear related cache
                ClearCache();
            }, $"Batch Insert: {tableName} ({items.Count} items)");
        }
        public static void MeasureExecutionTime(Action action, string description)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            Console.WriteLine($"{description} took {stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Optimize database connection
        /// </summary>
        public static SQLiteConnection GetOptimizedConnection()
        {
            var connection = DatabaseHelper.GetConnection();
            
            // Set connection-specific optimizations
            connection.Open();
            
            using (var command = new SQLiteCommand("PRAGMA foreign_keys=ON;", connection))
            {
                command.ExecuteNonQuery();
            }
            
            using (var command = new SQLiteCommand("PRAGMA busy_timeout=30000;", connection))
            {
                command.ExecuteNonQuery();
            }
            
            return connection;
        }

        /// <summary>
        /// Get performance statistics
        /// </summary>
        public static PerformanceStatistics GetPerformanceStatistics()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var cacheStats = GetCacheStatistics();
                
                return new PerformanceStatistics
                {
                    MemoryUsage = process.WorkingSet64,
                    CpuTime = process.TotalProcessorTime,
                    ThreadCount = process.Threads.Count,
                    HandleCount = process.HandleCount,
                    CacheStatistics = cacheStats,
                    Uptime = DateTime.Now - process.StartTime
                };
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Performance Statistics");
                return new PerformanceStatistics();
            }
        }

        /// <summary>
        /// Optimize memory usage
        /// </summary>
        public static void OptimizeMemory()
        {
            try
            {
                // Force garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                // Clear cache if memory usage is high
                var stats = GetPerformanceStatistics();
                if (stats.MemoryUsage > 100 * 1024 * 1024) // 100MB
                {
                    ClearCache();
                }
                
                ErrorHandler.LogInfo("Memory optimized", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Optimize Memory");
            }
        }

        /// <summary>
        /// Create database indexes for better performance
        /// </summary>
        public static void CreateIndexes()
        {
            try
            {
                var indexes = new[]
                {
                    "CREATE INDEX IF NOT EXISTS idx_items_code ON Items(Code);",
                    "CREATE INDEX IF NOT EXISTS idx_items_name ON Items(ItemName);",
                    "CREATE INDEX IF NOT EXISTS idx_items_category ON Items(CategoryId);",
                    "CREATE INDEX IF NOT EXISTS idx_clients_name ON Clients(Name);",
                    "CREATE INDEX IF NOT EXISTS idx_clients_phone ON Clients(Phone);",
                    "CREATE INDEX IF NOT EXISTS idx_invoices_number ON Invoices(InvoiceNumber);",
                    "CREATE INDEX IF NOT EXISTS idx_invoices_client ON Invoices(ClientId);",
                    "CREATE INDEX IF NOT EXISTS idx_invoices_date ON Invoices(InvoiceDate);",
                    "CREATE INDEX IF NOT EXISTS idx_categories_name ON Categories(Name);",
                    "CREATE INDEX IF NOT EXISTS idx_categories_parent ON Categories(ParentId);",
                    "CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username);",
                    "CREATE INDEX IF NOT EXISTS idx_expenses_date ON Expenses(ExpenseDate);",
                    "CREATE INDEX IF NOT EXISTS idx_purchases_date ON Purchases(PurchaseDate);"
                };
                
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    foreach (var indexSql in indexes)
                    {
                        using (var command = new SQLiteCommand(indexSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                
                ErrorHandler.LogInfo("Database indexes created successfully", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Indexes");
            }
        }

        /// <summary>
        /// Vacuum database to reclaim space
        /// </summary>
        public static void VacuumDatabase()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    
                    using (var command = new SQLiteCommand("VACUUM;", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                
                ErrorHandler.LogInfo("Database vacuumed successfully", "Performance Optimizer");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Vacuum Database");
            }
        }
    }

    /// <summary>
    /// Cache statistics class
    /// </summary>
    public class CacheStatistics
    {
        public int TotalItems { get; set; }
        public int ValidItems { get; set; }
        public int ExpiredItems { get; set; }
        public long MemoryUsage { get; set; }
    }

    /// <summary>
    /// Performance statistics class
    /// </summary>
    public class PerformanceStatistics
    {
        public long MemoryUsage { get; set; }
        public TimeSpan CpuTime { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public CacheStatistics CacheStatistics { get; set; } = new CacheStatistics();
        public TimeSpan Uptime { get; set; }
    }
}