using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ETAG_ERP.Helpers
{
    public static class PerformanceOptimizer
    {
        private static readonly Dictionary<string, Stopwatch> _timers = new Dictionary<string, Stopwatch>();
        private static readonly List<PerformanceMetric> _metrics = new List<PerformanceMetric>();

        public static void StartTimer(string operationName)
        {
            var stopwatch = Stopwatch.StartNew();
            _timers[operationName] = stopwatch;
        }

        public static void EndTimer(string operationName)
        {
            if (_timers.ContainsKey(operationName))
            {
                _timers[operationName].Stop();
                var elapsed = _timers[operationName].ElapsedMilliseconds;
                
                _metrics.Add(new PerformanceMetric
                {
                    OperationName = operationName,
                    ElapsedMilliseconds = elapsed,
                    Timestamp = DateTime.Now
                });

                _timers.Remove(operationName);
            }
        }

        public static void OptimizeDataGrid(DataGrid dataGrid)
        {
            if (dataGrid == null) return;

            // تحسين الأداء للـ DataGrid
            dataGrid.VirtualizingPanel.IsVirtualizing = true;
            dataGrid.VirtualizingPanel.VirtualizationMode = VirtualizationMode.Recycling;
            dataGrid.EnableRowVirtualization = true;
            dataGrid.EnableColumnVirtualization = true;
            
            // تحسين التحديث
            dataGrid.AutoGenerateColumns = false;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserDeleteRows = false;
            dataGrid.IsReadOnly = true;
        }

        public static void OptimizeListView(ListView listView)
        {
            if (listView == null) return;

            // تحسين الأداء للـ ListView
            listView.VirtualizingPanel.IsVirtualizing = true;
            listView.VirtualizingPanel.VirtualizationMode = VirtualizationMode.Recycling;
        }

        public static void OptimizeTreeView(TreeView treeView)
        {
            if (treeView == null) return;

            // تحسين الأداء للـ TreeView
            treeView.VirtualizingPanel.IsVirtualizing = true;
            treeView.VirtualizingPanel.VirtualizationMode = VirtualizationMode.Recycling;
        }

        public static async Task<T> ExecuteWithPerformanceMonitoring<T>(string operationName, Func<T> operation)
        {
            StartTimer(operationName);
            try
            {
                var result = await Task.Run(operation);
                return result;
            }
            finally
            {
                EndTimer(operationName);
            }
        }

        public static async Task ExecuteWithPerformanceMonitoring(string operationName, Action operation)
        {
            StartTimer(operationName);
            try
            {
                await Task.Run(operation);
            }
            finally
            {
                EndTimer(operationName);
            }
        }

        public static void OptimizeDatabaseQueries()
        {
            // تحسين استعلامات قاعدة البيانات
            // يمكن إضافة فهرسة وتحسين الاستعلامات هنا
        }

        public static void OptimizeMemoryUsage()
        {
            // تحسين استخدام الذاكرة
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static void OptimizeUIResponsiveness()
        {
            // تحسين استجابة واجهة المستخدم
            Application.Current.Dispatcher.Invoke(() =>
            {
                // إجبار تحديث واجهة المستخدم
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
            }, DispatcherPriority.Background);
        }

        public static List<PerformanceMetric> GetPerformanceMetrics()
        {
            return _metrics.ToList();
        }

        public static List<PerformanceMetric> GetPerformanceMetrics(string operationName)
        {
            return _metrics.Where(m => m.OperationName == operationName).ToList();
        }

        public static double GetAverageExecutionTime(string operationName)
        {
            var metrics = GetPerformanceMetrics(operationName);
            if (metrics.Count == 0) return 0;

            return metrics.Average(m => m.ElapsedMilliseconds);
        }

        public static long GetTotalExecutionTime(string operationName)
        {
            var metrics = GetPerformanceMetrics(operationName);
            return metrics.Sum(m => m.ElapsedMilliseconds);
        }

        public static void ClearPerformanceMetrics()
        {
            _metrics.Clear();
        }

        public static void GeneratePerformanceReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== تقرير الأداء ===");
            report.AppendLine($"تاريخ التقرير: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            var groupedMetrics = _metrics.GroupBy(m => m.OperationName);
            
            foreach (var group in groupedMetrics)
            {
                var operationName = group.Key;
                var metrics = group.ToList();
                var averageTime = metrics.Average(m => m.ElapsedMilliseconds);
                var totalTime = metrics.Sum(m => m.ElapsedMilliseconds);
                var minTime = metrics.Min(m => m.ElapsedMilliseconds);
                var maxTime = metrics.Max(m => m.ElapsedMilliseconds);

                report.AppendLine($"العملية: {operationName}");
                report.AppendLine($"  عدد المرات: {metrics.Count}");
                report.AppendLine($"  متوسط الوقت: {averageTime:F2} ms");
                report.AppendLine($"  إجمالي الوقت: {totalTime} ms");
                report.AppendLine($"  أقل وقت: {minTime} ms");
                report.AppendLine($"  أطول وقت: {maxTime} ms");
                report.AppendLine();
            }

            // حفظ التقرير
            var reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PerformanceReport.txt");
            System.IO.File.WriteAllText(reportPath, report.ToString(), System.Text.Encoding.UTF8);
        }

        public static void OptimizeApplicationStartup()
        {
            // تحسين بدء تشغيل التطبيق
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                // تحميل البيانات في الخلفية
                Task.Run(() =>
                {
                    // تحميل البيانات الأساسية
                    DatabaseHelper.GetAllCategories();
                    DatabaseHelper.GetAllItems();
                    DatabaseHelper.GetAllClients();
                });
            }));
        }

        public static void OptimizeDataLoading<T>(IEnumerable<T> data, Action<IEnumerable<T>> updateUI)
        {
            // تحسين تحميل البيانات
            var batchSize = 100;
            var batches = data.Chunk(batchSize);

            foreach (var batch in batches)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    updateUI(batch);
                }));

                // تأخير صغير لتجنب تجميد واجهة المستخدم
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void OptimizeSearch(string searchText, IEnumerable<object> data, Func<object, bool> filter, Action<IEnumerable<object>> updateResults)
        {
            // تحسين البحث
            if (string.IsNullOrWhiteSpace(searchText))
            {
                updateResults(data);
                return;
            }

            var filteredData = data.Where(filter).Take(1000); // تحديد النتائج
            updateResults(filteredData);
        }

        public static void OptimizeExport<T>(IEnumerable<T> data, string filePath, Action<IEnumerable<T>, string> exportAction)
        {
            // تحسين التصدير
            Task.Run(() =>
            {
                try
                {
                    exportAction(data, filePath);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ErrorHandler.HandleException(ex, "Export Operation");
                    });
                }
            });
        }

        public static void MonitorMemoryUsage()
        {
            var memoryUsage = GC.GetTotalMemory(false);
            var memoryUsageMB = memoryUsage / 1024.0 / 1024.0;
            
            if (memoryUsageMB > 500) // إذا تجاوزت الذاكرة 500 ميجابايت
            {
                OptimizeMemoryUsage();
            }
        }

        public static void OptimizeThreading()
        {
            // تحسين استخدام الخيوط
            System.Threading.ThreadPool.SetMinThreads(Environment.ProcessorCount, Environment.ProcessorCount);
            System.Threading.ThreadPool.SetMaxThreads(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        }
    }

    public class PerformanceMetric
    {
        public string OperationName { get; set; } = "";
        public long ElapsedMilliseconds { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
