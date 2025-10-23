using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    public static class DashboardHelper
    {
        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public static DashboardStatistics GetDashboardStatistics()
        {
            var stats = new DashboardStatistics();
            
            try
            {
                // Get basic counts
                stats.TotalItems = DatabaseHelper_Extensions.GetTotalItems();
                stats.TotalClients = DatabaseHelper_Extensions.GetTotalClients();
                stats.TotalInvoices = DatabaseHelper_Extensions.GetAllInvoices().Count;
                stats.TotalPurchases = DatabaseHelper_Extensions.GetAllPurchases().Count;
                
                // Get financial data
                stats.TotalSales = DatabaseHelper_Extensions.GetTotalSales();
                stats.TotalPurchases = DatabaseHelper_Extensions.GetTotalPurchases();
                stats.TotalExpenses = DatabaseHelper_Extensions.GetTotalExpenses();
                stats.NetProfit = stats.TotalSales - stats.TotalPurchases - stats.TotalExpenses;
                
                // Get low stock items
                stats.LowStockItems = DatabaseHelper_Extensions.GetLowStockItems();
                
                // Get recent activities
                stats.RecentInvoices = GetRecentInvoices();
                stats.RecentPurchases = GetRecentPurchases();
                stats.RecentExpenses = GetRecentExpenses();
                
                // Get monthly sales data
                stats.MonthlySales = GetMonthlySales();
                
                // Get top selling items
                stats.TopSellingItems = GetTopSellingItems();
                
                // Get client statistics
                stats.ClientStatistics = GetClientStatistics();
                
                ErrorHandler.LogInfo("تم تحديث إحصائيات لوحة التحكم", "Dashboard Helper");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Dashboard Statistics");
            }
            
            return stats;
        }

        /// <summary>
        /// Get recent invoices
        /// </summary>
        private static List<Invoice> GetRecentInvoices()
        {
            try
            {
                return DatabaseHelper_Extensions.GetAllInvoices()
                    .OrderByDescending(i => i.InvoiceDate)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Recent Invoices");
                return new List<Invoice>();
            }
        }

        /// <summary>
        /// Get recent purchases
        /// </summary>
        private static List<Purchase> GetRecentPurchases()
        {
            try
            {
                return DatabaseHelper_Extensions.GetAllPurchases()
                    .OrderByDescending(p => p.PurchaseDate)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Recent Purchases");
                return new List<Purchase>();
            }
        }

        /// <summary>
        /// Get recent expenses
        /// </summary>
        private static List<Expense> GetRecentExpenses()
        {
            try
            {
                return DatabaseHelper_Extensions.GetAllExpenses()
                    .OrderByDescending(e => e.ExpenseDate)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Recent Expenses");
                return new List<Expense>();
            }
        }

        /// <summary>
        /// Get monthly sales data
        /// </summary>
        private static List<MonthlySalesData> GetMonthlySales()
        {
            var monthlySales = new List<MonthlySalesData>();
            
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                var currentDate = DateTime.Now;
                
                // Get last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = currentDate.AddMonths(-i);
                    var monthInvoices = invoices.Where(i => 
                        i.InvoiceDate.Year == monthDate.Year && 
                        i.InvoiceDate.Month == monthDate.Month).ToList();
                    
                    monthlySales.Add(new MonthlySalesData
                    {
                        Month = monthDate.ToString("yyyy-MM"),
                        MonthName = monthDate.ToString("MMM yyyy"),
                        Sales = monthInvoices.Sum(i => i.TotalAmount),
                        InvoiceCount = monthInvoices.Count
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Monthly Sales");
            }
            
            return monthlySales;
        }

        /// <summary>
        /// Get top selling items
        /// </summary>
        private static List<TopSellingItem> GetTopSellingItems()
        {
            var topItems = new List<TopSellingItem>();
            
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                var items = DatabaseHelper_Extensions.GetAllItems();
                
                var itemSales = new Dictionary<int, decimal>();
                
                foreach (var invoice in invoices)
                {
                    foreach (var item in invoice.Items)
                    {
                        if (itemSales.ContainsKey(item.ItemId ?? 0))
                        {
                            itemSales[item.ItemId ?? 0] += item.UnitPrice * item.Quantity;
                        }
                        else
                        {
                            itemSales[item.ItemId ?? 0] = item.UnitPrice * item.Quantity;
                        }
                    }
                }
                
                topItems = itemSales
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(10)
                    .Select(kvp => new TopSellingItem
                    {
                        ItemId = kvp.Key,
                        ItemName = items.FirstOrDefault(i => i.Id == kvp.Key)?.ItemName ?? "غير معروف",
                        TotalSales = kvp.Value
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Top Selling Items");
            }
            
            return topItems;
        }

        /// <summary>
        /// Get client statistics
        /// </summary>
        private static ClientStatistics GetClientStatistics()
        {
            var stats = new ClientStatistics();
            
            try
            {
                var clients = DatabaseHelper_Extensions.GetAllClients();
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                
                stats.TotalClients = clients.Count;
                stats.ActiveClients = clients.Count(c => c.IsActive);
                stats.TopClients = GetTopClients(clients, invoices);
                stats.ClientGrowth = GetClientGrowth(clients);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Client Statistics");
            }
            
            return stats;
        }

        /// <summary>
        /// Get top clients by sales
        /// </summary>
        private static List<TopClient> GetTopClients(List<Client> clients, List<Invoice> invoices)
        {
            var topClients = new List<TopClient>();
            
            try
            {
                var clientSales = new Dictionary<int, decimal>();
                
                foreach (var invoice in invoices)
                {
                    if (invoice.ClientId.HasValue)
                    {
                        if (clientSales.ContainsKey(invoice.ClientId.Value))
                        {
                            clientSales[invoice.ClientId.Value] += invoice.TotalAmount;
                        }
                        else
                        {
                            clientSales[invoice.ClientId.Value] = invoice.TotalAmount;
                        }
                    }
                }
                
                topClients = clientSales
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(5)
                    .Select(kvp => new TopClient
                    {
                        ClientId = kvp.Key,
                        ClientName = clients.FirstOrDefault(c => c.Id == kvp.Key)?.Name ?? "غير معروف",
                        TotalSales = kvp.Value,
                        InvoiceCount = invoices.Count(i => i.ClientId == kvp.Key)
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Top Clients");
            }
            
            return topClients;
        }

        /// <summary>
        /// Get client growth data
        /// </summary>
        private static List<ClientGrowthData> GetClientGrowth(List<Client> clients)
        {
            var growthData = new List<ClientGrowthData>();
            
            try
            {
                var currentDate = DateTime.Now;
                
                // Get last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = currentDate.AddMonths(-i);
                    var monthClients = clients.Where(c => 
                        c.CreatedAt.Year == monthDate.Year && 
                        c.CreatedAt.Month == monthDate.Month).Count();
                    
                    growthData.Add(new ClientGrowthData
                    {
                        Month = monthDate.ToString("yyyy-MM"),
                        MonthName = monthDate.ToString("MMM yyyy"),
                        NewClients = monthClients
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Client Growth");
            }
            
            return growthData;
        }

        /// <summary>
        /// Get inventory alerts
        /// </summary>
        public static List<InventoryAlert> GetInventoryAlerts()
        {
            var alerts = new List<InventoryAlert>();
            
            try
            {
                var items = DatabaseHelper_Extensions.GetAllItems();
                
                foreach (var item in items)
                {
                    if (item.Quantity <= item.MinStock)
                    {
                        alerts.Add(new InventoryAlert
                        {
                            ItemId = item.Id,
                            ItemName = item.ItemName,
                            CurrentQuantity = item.Quantity,
                            MinQuantity = item.MinStock,
                            AlertType = item.Quantity == 0 ? "نفاد المخزون" : "مخزون منخفض"
                        });
                    }
                }
                
                alerts = alerts.OrderBy(a => a.CurrentQuantity).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Inventory Alerts");
            }
            
            return alerts;
        }

        /// <summary>
        /// Get financial summary
        /// </summary>
        public static FinancialSummary GetFinancialSummary()
        {
            var summary = new FinancialSummary();
            
            try
            {
                var invoices = DatabaseHelper_Extensions.GetAllInvoices();
                var purchases = DatabaseHelper_Extensions.GetAllPurchases();
                var expenses = DatabaseHelper_Extensions.GetAllExpenses();
                
                // Current month data
                var currentMonth = DateTime.Now;
                var currentMonthInvoices = invoices.Where(i => 
                    i.InvoiceDate.Year == currentMonth.Year && 
                    i.InvoiceDate.Month == currentMonth.Month).ToList();
                
                var currentMonthPurchases = purchases.Where(p => 
                    p.PurchaseDate.Year == currentMonth.Year && 
                    p.PurchaseDate.Month == currentMonth.Month).ToList();
                
                var currentMonthExpenses = expenses.Where(e => 
                    e.ExpenseDate?.Year == currentMonth.Year && 
                    e.ExpenseDate?.Month == currentMonth.Month).ToList();
                
                summary.CurrentMonthSales = currentMonthInvoices.Sum(i => i.TotalAmount);
                summary.CurrentMonthPurchases = currentMonthPurchases.Sum(p => p.TotalAmount);
                summary.CurrentMonthExpenses = currentMonthExpenses.Sum(e => e.Amount);
                summary.CurrentMonthProfit = summary.CurrentMonthSales - summary.CurrentMonthPurchases - summary.CurrentMonthExpenses;
                
                // Previous month data for comparison
                var previousMonth = currentMonth.AddMonths(-1);
                var previousMonthInvoices = invoices.Where(i => 
                    i.InvoiceDate.Year == previousMonth.Year && 
                    i.InvoiceDate.Month == previousMonth.Month).ToList();
                
                var previousMonthSales = previousMonthInvoices.Sum(i => i.TotalAmount);
                
                // Calculate growth percentage
                if (previousMonthSales > 0)
                {
                    summary.SalesGrowthPercentage = ((summary.CurrentMonthSales - previousMonthSales) / previousMonthSales) * 100;
                }
                
                // Outstanding amounts
                summary.OutstandingAmount = invoices.Sum(i => i.TotalAmount - i.PaidAmount);
                summary.OverdueInvoices = invoices.Count(i => 
                    i.TotalAmount > i.PaidAmount && 
                    i.InvoiceDate < DateTime.Now.AddDays(-30));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Financial Summary");
            }
            
            return summary;
        }

        /// <summary>
        /// Get system health status
        /// </summary>
        public static SystemHealthStatus GetSystemHealthStatus()
        {
            var status = new SystemHealthStatus();
            
            try
            {
                // Database health
                status.DatabaseStatus = DatabaseInitializer.CheckDatabaseIntegrity() ? "سليم" : "مشكلة";
                
                // Performance status
                var perfStats = PerformanceOptimizer.GetPerformanceStatistics();
                status.MemoryUsage = perfStats.MemoryUsage;
                status.CpuUsage = perfStats.CpuTime.TotalMilliseconds;
                
                // Cache status
                var cacheStats = PerformanceOptimizer.GetCacheStatistics();
                status.CacheStatus = cacheStats.ValidItems > 0 ? "نشط" : "غير نشط";
                
                // Session status
                status.SessionStatus = SessionManager.IsLoggedIn ? "نشط" : "غير نشط";
                
                // Overall status
                status.OverallStatus = status.DatabaseStatus == "سليم" ? "سليم" : "يحتاج انتباه";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get System Health Status");
                status.OverallStatus = "خطأ";
            }
            
            return status;
        }
    }

    /// <summary>
    /// Dashboard statistics class
    /// </summary>
    public class DashboardStatistics
    {
        public int TotalItems { get; set; }
        public int TotalClients { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int LowStockItems { get; set; }
        public List<Invoice> RecentInvoices { get; set; } = new List<Invoice>();
        public List<Purchase> RecentPurchases { get; set; } = new List<Purchase>();
        public List<Expense> RecentExpenses { get; set; } = new List<Expense>();
        public List<MonthlySalesData> MonthlySales { get; set; } = new List<MonthlySalesData>();
        public List<TopSellingItem> TopSellingItems { get; set; } = new List<TopSellingItem>();
        public ClientStatistics ClientStatistics { get; set; } = new ClientStatistics();
    }

    /// <summary>
    /// Monthly sales data class
    /// </summary>
    public class MonthlySalesData
    {
        public string Month { get; set; } = "";
        public string MonthName { get; set; } = "";
        public decimal Sales { get; set; }
        public int InvoiceCount { get; set; }
    }

    /// <summary>
    /// Top selling item class
    /// </summary>
    public class TopSellingItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = "";
        public decimal TotalSales { get; set; }
    }

    /// <summary>
    /// Client statistics class
    /// </summary>
    public class ClientStatistics
    {
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public List<TopClient> TopClients { get; set; } = new List<TopClient>();
        public List<ClientGrowthData> ClientGrowth { get; set; } = new List<ClientGrowthData>();
    }

    /// <summary>
    /// Top client class
    /// </summary>
    public class TopClient
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = "";
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
    }

    /// <summary>
    /// Client growth data class
    /// </summary>
    public class ClientGrowthData
    {
        public string Month { get; set; } = "";
        public string MonthName { get; set; } = "";
        public int NewClients { get; set; }
    }

    /// <summary>
    /// Inventory alert class
    /// </summary>
    public class InventoryAlert
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = "";
        public int CurrentQuantity { get; set; }
        public int MinQuantity { get; set; }
        public string AlertType { get; set; } = "";
    }

    /// <summary>
    /// Financial summary class
    /// </summary>
    public class FinancialSummary
    {
        public decimal CurrentMonthSales { get; set; }
        public decimal CurrentMonthPurchases { get; set; }
        public decimal CurrentMonthExpenses { get; set; }
        public decimal CurrentMonthProfit { get; set; }
        public decimal SalesGrowthPercentage { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int OverdueInvoices { get; set; }
    }

    /// <summary>
    /// System health status class
    /// </summary>
    public class SystemHealthStatus
    {
        public string DatabaseStatus { get; set; } = "";
        public long MemoryUsage { get; set; }
        public double CpuUsage { get; set; }
        public string CacheStatus { get; set; } = "";
        public string SessionStatus { get; set; } = "";
        public string OverallStatus { get; set; } = "";
    }
}