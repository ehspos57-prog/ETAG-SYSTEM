using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    public static class DatabaseHelper_Extensions
    {
        // ================== Settings ==================
        public static void SetSetting(string key, string value)
        {
            DatabaseHelper.SetSetting(key, value);
        }

        public static string GetSetting(string key)
        {
            return DatabaseHelper.GetSetting(key) ?? "";
        }

        // ================== Users ==================
        public static int InsertUser(User user)
        {
            return DatabaseHelper.InsertUser(user);
        }

        public static void UpdateUser(User user)
        {
            DatabaseHelper.UpdateUser(user);
        }

        public static void DeleteUser(int userId)
        {
            DatabaseHelper.DeleteUser(userId);
        }

        public static List<User> GetAllUsers()
        {
            return DatabaseHelper.GetAllUsers();
        }

        // ================== Items ==================
        public static bool InsertItem(Item item)
        {
            return DatabaseHelper.InsertItem(item);
        }

        public static bool UpdateItem(Item item)
        {
            return DatabaseHelper.UpdateItem(item);
        }

        public static bool DeleteItem(int itemId)
        {
            return DatabaseHelper.DeleteItem(itemId);
        }

        public static List<Item> GetAllItems()
        {
            return DatabaseHelper.GetAllItems();
        }

        // ================== Clients ==================
        public static int InsertClient(Client client)
        {
            return DatabaseHelper.InsertClient(client);
        }

        public static void UpdateClient(Client client)
        {
            DatabaseHelper.UpdateClient(client);
        }

        public static void DeleteClient(int clientId)
        {
            DatabaseHelper.DeleteClient(clientId);
        }

        public static List<Client> GetAllClients()
        {
            return DatabaseHelper.GetAllClients();
        }

        // ================== Accounts ==================
        public static int InsertAccount(Account account)
        {
            return DatabaseHelper.InsertAccount(account);
        }

        public static void UpdateAccount(Account account)
        {
            DatabaseHelper.UpdateAccount(account);
        }

        public static void DeleteAccount(int accountId)
        {
            DatabaseHelper.DeleteAccount(accountId);
        }

        public static List<Account> GetAllAccounts()
        {
            return DatabaseHelper.GetAllAccounts();
        }

        // ================== Invoices ==================
        public static int InsertInvoice(Invoice invoice)
        {
            return DatabaseHelper.InsertInvoice(invoice);
        }

        public static void UpdateInvoice(Invoice invoice)
        {
            DatabaseHelper.UpdateInvoice(invoice);
        }

        public static void DeleteInvoice(int invoiceId)
        {
            DatabaseHelper.DeleteInvoice(invoiceId);
        }

        public static List<Invoice> GetAllInvoices()
        {
            return DatabaseHelper.GetAllInvoices();
        }

        // ================== Purchases ==================
        public static int InsertPurchase(Purchase purchase)
        {
            return DatabaseHelper.InsertPurchase(purchase);
        }

        public static void UpdatePurchase(Purchase purchase)
        {
            DatabaseHelper.UpdatePurchase(purchase);
        }

        public static void DeletePurchase(int purchaseId)
        {
            DatabaseHelper.DeletePurchase(purchaseId);
        }

        public static List<Purchase> GetAllPurchases()
        {
            return DatabaseHelper.GetAllPurchases();
        }

        // ================== Expenses ==================
        public static int InsertExpense(Expense expense)
        {
            return DatabaseHelper.InsertExpense(expense);
        }

        public static void UpdateExpense(Expense expense)
        {
            DatabaseHelper.UpdateExpense(expense);
        }

        public static void DeleteExpense(int expenseId)
        {
            DatabaseHelper.DeleteExpense(expenseId);
        }

        public static List<Expense> GetAllExpenses()
        {
            return DatabaseHelper.GetAllExpenses();
        }

        // ================== Categories ==================
        public static int InsertCategory(Category category)
        {
            return DatabaseHelper.InsertCategory(category);
        }

        public static void UpdateCategory(Category category)
        {
            DatabaseHelper.UpdateCategory(category);
        }

        public static void DeleteCategory(int categoryId)
        {
            DatabaseHelper.DeleteCategory(categoryId);
        }

        public static List<Category> GetAllCategories()
        {
            return DatabaseHelper.GetAllCategories();
        }

        // ================== Permissions ==================
        public static int InsertPermission(string name, string description = "")
        {
            return DatabaseHelper.InsertPermission(name, description);
        }

        public static List<(int Id, string Name)> GetAllPermissions()
        {
            return DatabaseHelper.GetAllPermissions();
        }

        public static void AssignPermissionToUser(int userId, int permissionId)
        {
            DatabaseHelper.AssignPermissionToUser(userId, permissionId);
        }

        public static void RemovePermissionFromUser(int userId, int permissionId)
        {
            DatabaseHelper.RemovePermissionFromUser(userId, permissionId);
        }

        public static List<int> GetUserPermissions(int userId)
        {
            return DatabaseHelper.GetUserPermissions(userId);
        }

        // ================== Branches ==================
        public static int InsertBranch(Branch branch)
        {
            return DatabaseHelper.InsertBranch(branch);
        }

        public static void UpdateBranch(Branch branch)
        {
            DatabaseHelper.UpdateBranch(branch);
        }

        public static void DeleteBranch(int branchId)
        {
            DatabaseHelper.DeleteBranch(branchId);
        }

        public static List<Branch> GetAllBranches()
        {
            return DatabaseHelper.GetAllBranches();
        }

        // ================== Employees ==================
        public static int InsertEmployee(Employee employee)
        {
            return DatabaseHelper.InsertEmployee(employee);
        }

        public static void UpdateEmployee(Employee employee)
        {
            DatabaseHelper.UpdateEmployee(employee);
        }

        public static void DeleteEmployee(int employeeId)
        {
            DatabaseHelper.DeleteEmployee(employeeId);
        }

        public static List<Employee> GetAllEmployees()
        {
            return DatabaseHelper.GetAllEmployees();
        }

        // ================== Utility Methods ==================
        public static DataTable GetDataTable(string sql, params SQLiteParameter[] parameters)
        {
            return DatabaseHelper.GetDataTable(sql, parameters);
        }

        public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            return DatabaseHelper.ExecuteScalar(sql, parameters);
        }

        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            return DatabaseHelper.ExecuteNonQuery(sql, parameters);
        }

        // ================== Search Methods ==================
        public static List<Item> SearchItems(string searchTerm)
        {
            var sql = @"SELECT * FROM Items 
                       WHERE ItemName LIKE @search 
                       OR Code LIKE @search 
                       OR Description LIKE @search
                       OR Barcode LIKE @search";
            
            var dt = GetDataTable(sql, new SQLiteParameter("@search", $"%{searchTerm}%"));
            var items = new List<Item>();
            
            foreach (DataRow row in dt.Rows)
            {
                items.Add(MapItem(row));
            }
            
            return items;
        }

        public static List<Client> SearchClients(string searchTerm)
        {
            var sql = @"SELECT * FROM Clients 
                       WHERE Name LIKE @search 
                       OR Phone LIKE @search 
                       OR Email LIKE @search
                       OR Address LIKE @search";
            
            var dt = GetDataTable(sql, new SQLiteParameter("@search", $"%{searchTerm}%"));
            var clients = new List<Client>();
            
            foreach (DataRow row in dt.Rows)
            {
                clients.Add(MapClient(row));
            }
            
            return clients;
        }

        // ================== Mapping Methods ==================
        private static Item MapItem(DataRow row)
        {
            return new Item
            {
                Id = Convert.ToInt32(row["Id"]),
                ItemName = row["ItemName"]?.ToString() ?? "",
                Code = row["Code"]?.ToString() ?? "",
                Quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0,
                SellingPrice = row["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(row["SellingPrice"]) : 0,
                PurchasePrice = row["PurchasePrice"] != DBNull.Value ? Convert.ToDecimal(row["PurchasePrice"]) : 0,
                Description = row["Description"]?.ToString() ?? "",
                Unit = row["Unit"]?.ToString() ?? "قطعة",
                Barcode = row["Barcode"]?.ToString(),
                Tax = row["Tax"] != DBNull.Value ? Convert.ToDecimal(row["Tax"]) : 0,
                Discount = row["Discount"] != DBNull.Value ? Convert.ToDecimal(row["Discount"]) : 0,
                ImagePath = row["ImagePath"]?.ToString()
            };
        }

        private static Client MapClient(DataRow row)
        {
            return new Client
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"]?.ToString() ?? "",
                Phone = row["Phone"]?.ToString(),
                Email = row["Email"]?.ToString(),
                Address = row["Address"]?.ToString(),
                Notes = row["Notes"]?.ToString(),
                Balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0,
                TaxCard = row["TaxCard"]?.ToString(),
                CommercialRecord = row["CommercialRecord"]?.ToString()
            };
        }

        // ================== Statistics Methods ==================
        public static decimal GetTotalSales()
        {
            var result = ExecuteScalar("SELECT SUM(TotalAmount) FROM Invoices WHERE Status = 'Paid'");
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public static decimal GetTotalPurchases()
        {
            var result = ExecuteScalar("SELECT SUM(TotalAmount) FROM Purchases");
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public static decimal GetTotalExpenses()
        {
            var result = ExecuteScalar("SELECT SUM(Amount) FROM Expenses");
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public static int GetTotalItems()
        {
            var result = ExecuteScalar("SELECT COUNT(*) FROM Items");
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public static int GetTotalClients()
        {
            var result = ExecuteScalar("SELECT COUNT(*) FROM Clients");
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public static int GetLowStockItems()
        {
            var result = ExecuteScalar("SELECT COUNT(*) FROM Items WHERE Quantity <= MinStock");
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // ================== Backup and Restore ==================
        public static void BackupDatabase(string backupPath)
        {
            try
            {
                var sourcePath = "ETAG_ERP.db";
                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Copy(sourcePath, backupPath, true);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Database Backup");
            }
        }

        public static void RestoreDatabase(string backupPath)
        {
            try
            {
                var targetPath = "ETAG_ERP.db";
                if (System.IO.File.Exists(backupPath))
                {
                    System.IO.File.Copy(backupPath, targetPath, true);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Database Restore");
            }
        }

        // ================== Data Validation ==================
        public static bool ValidateItem(Item item)
        {
            if (string.IsNullOrWhiteSpace(item.ItemName))
                return false;
            
            if (item.SellingPrice <= 0)
                return false;
            
            if (item.Quantity < 0)
                return false;
            
            return true;
        }

        public static bool ValidateClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Name))
                return false;
            
            return true;
        }

        public static bool ValidateInvoice(Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                return false;
            
            if (invoice.TotalAmount <= 0)
                return false;
            
            return true;
        }
    }
}