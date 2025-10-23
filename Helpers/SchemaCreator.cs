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
    /// Database schema creator and manager
    /// </summary>
    public static class SchemaCreator
    {
        #region Tables

        public static bool CreateUsersTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                FullName TEXT NOT NULL,
                Email TEXT,
                Phone TEXT,
                Role TEXT NOT NULL,
                IsAdmin BOOLEAN DEFAULT 0,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                LastLogin DATETIME,
                PasswordChangedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            )") > 0;

        public static bool CreatePermissionsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Permissions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                PermissionName TEXT NOT NULL,
                IsGranted BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )") > 0;

        public static bool CreateBranchesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Branches (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT,
                Phone TEXT,
                Type TEXT DEFAULT 'Branch',
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateEmployeesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Employees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                JobTitle TEXT,
                Salary DECIMAL(10,2) DEFAULT 0,
                HireDate DATE,
                Phone TEXT,
                Email TEXT,
                Address TEXT,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateAccountsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Accounts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                Balance DECIMAL(10,2) DEFAULT 0,
                Description TEXT,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateCategoriesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT,
                Description TEXT,
                ParentId INTEGER,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (ParentId) REFERENCES Categories(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateItemsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ItemName TEXT NOT NULL,
                Code TEXT UNIQUE,
                Description TEXT,
                Quantity INTEGER DEFAULT 0,
                MinStock INTEGER DEFAULT 0,
                MaxStock INTEGER DEFAULT 0,
                SellingPrice DECIMAL(10,2) DEFAULT 0,
                PurchasePrice DECIMAL(10,2) DEFAULT 0,
                Unit TEXT DEFAULT 'قطعة',
                CategoryId INTEGER,
                Barcode TEXT,
                ImagePath TEXT,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateClientsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Phone TEXT,
                Email TEXT,
                Address TEXT,
                Balance DECIMAL(10,2) DEFAULT 0,
                TaxNumber TEXT,
                CommercialRecord TEXT,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateInvoicesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Invoices (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceNumber TEXT UNIQUE NOT NULL,
                ClientId INTEGER,
                ClientName TEXT,
                InvoiceDate DATE NOT NULL,
                DueDate DATE,
                TotalAmount DECIMAL(10,2) DEFAULT 0,
                PaidAmount DECIMAL(10,2) DEFAULT 0,
                Status TEXT DEFAULT 'Pending',
                Type TEXT DEFAULT 'Sale',
                Notes TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateInvoiceItemsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS InvoiceItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                ItemName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice DECIMAL(10,2) NOT NULL,
                TotalPrice DECIMAL(10,2) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE,
                FOREIGN KEY (ItemId) REFERENCES Items(Id)
            )") > 0;

        public static bool CreateReturnsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Returns (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReturnNumber TEXT UNIQUE NOT NULL,
                ClientId INTEGER,
                ClientName TEXT,
                ReturnDate DATE NOT NULL,
                TotalAmount DECIMAL(10,2) DEFAULT 0,
                Status TEXT DEFAULT 'Pending',
                Reason TEXT,
                Notes TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateReturnItemsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS ReturnItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReturnId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                ItemName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice DECIMAL(10,2) NOT NULL,
                TotalPrice DECIMAL(10,2) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (ReturnId) REFERENCES Returns(Id) ON DELETE CASCADE,
                FOREIGN KEY (ItemId) REFERENCES Items(Id)
            )") > 0;

        public static bool CreatePurchasesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Purchases (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PurchaseNumber TEXT UNIQUE NOT NULL,
                Supplier TEXT NOT NULL,
                PurchaseDate DATE NOT NULL,
                Total DECIMAL(10,2) DEFAULT 0,
                Paid BOOLEAN DEFAULT 0,
                Notes TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreatePurchaseItemsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS PurchaseItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PurchaseId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                ItemName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice DECIMAL(10,2) NOT NULL,
                TotalPrice DECIMAL(10,2) NOT NULL,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (PurchaseId) REFERENCES Purchases(Id) ON DELETE CASCADE,
                FOREIGN KEY (ItemId) REFERENCES Items(Id)
            )") > 0;

        public static bool CreateExpensesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ExpenseType TEXT NOT NULL,
                Description TEXT,
                Amount DECIMAL(10,2) NOT NULL,
                ExpenseDate DATE NOT NULL,
                Category TEXT,
                AccountId INTEGER,
                Notes TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateSafesTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Safes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Balance DECIMAL(10,2) DEFAULT 0,
                IsActive BOOLEAN DEFAULT 1,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                CreatedBy INTEGER,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedBy INTEGER,
                FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
                FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateSettingsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Settings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Key TEXT UNIQUE NOT NULL,
                Value TEXT,
                Description TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
            )") > 0;

        public static bool CreateLogsTable() => DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Logs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Level TEXT NOT NULL,
                Message TEXT NOT NULL,
                Exception TEXT,
                Source TEXT,
                UserId INTEGER,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            )") > 0;

        public static bool CreateAllTables()
        {
            var tables = new Func<bool>[] {
                CreateUsersTable, CreatePermissionsTable, CreateBranchesTable,
                CreateEmployeesTable, CreateAccountsTable, CreateCategoriesTable,
                CreateItemsTable, CreateClientsTable, CreateInvoicesTable,
                CreateInvoiceItemsTable, CreateReturnsTable, CreateReturnItemsTable,
                CreatePurchasesTable, CreatePurchaseItemsTable, CreateExpensesTable,
                CreateSafesTable, CreateSettingsTable, CreateLogsTable
            };

            return tables.All(f => f());
        }

        #endregion

        #region Indexes

        public static bool CreateIndexes()
        {
            var indexes = new string[] {
                "CREATE INDEX IF NOT EXISTS idx_users_username ON Users(Username)",
                "CREATE INDEX IF NOT EXISTS idx_users_email ON Users(Email)",
                "CREATE INDEX IF NOT EXISTS idx_permissions_userid ON Permissions(UserId)",
                "CREATE INDEX IF NOT EXISTS idx_items_code ON Items(Code)",
                "CREATE INDEX IF NOT EXISTS idx_items_categoryid ON Items(CategoryId)",
                "CREATE INDEX IF NOT EXISTS idx_clients_name ON Clients(Name)",
                "CREATE INDEX IF NOT EXISTS idx_clients_phone ON Clients(Phone)",
                "CREATE INDEX IF NOT EXISTS idx_invoices_number ON Invoices(InvoiceNumber)",
                "CREATE INDEX IF NOT EXISTS idx_invoices_clientid ON Invoices(ClientId)",
                "CREATE INDEX IF NOT EXISTS idx_invoices_date ON Invoices(InvoiceDate)",
                "CREATE INDEX IF NOT EXISTS idx_returns_number ON Returns(ReturnNumber)",
                "CREATE INDEX IF NOT EXISTS idx_returns_clientid ON Returns(ClientId)",
                "CREATE INDEX IF NOT EXISTS idx_purchases_number ON Purchases(PurchaseNumber)",
                "CREATE INDEX IF NOT EXISTS idx_purchases_date ON Purchases(PurchaseDate)",
                "CREATE INDEX IF NOT EXISTS idx_expenses_date ON Expenses(ExpenseDate)",
                "CREATE INDEX IF NOT EXISTS idx_expenses_type ON Expenses(ExpenseType)",
                "CREATE INDEX IF NOT EXISTS idx_logs_level ON Logs(Level)",
                "CREATE INDEX IF NOT EXISTS idx_logs_createdat ON Logs(CreatedAt)"
            };

            return indexes.All(sql => DatabaseHelper.ExecuteNonQuery(sql) > 0);
        }

        #endregion

        #region Triggers

        public static bool CreateTriggers()
        {
            var triggers = new string[] {
                @"CREATE TRIGGER IF NOT EXISTS update_users_timestamp 
                    AFTER UPDATE ON Users
                    BEGIN
                        UPDATE Users SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_branches_timestamp 
                    AFTER UPDATE ON Branches
                    BEGIN
                        UPDATE Branches SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_employees_timestamp 
                    AFTER UPDATE ON Employees
                    BEGIN
                        UPDATE Employees SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_accounts_timestamp 
                    AFTER UPDATE ON Accounts
                    BEGIN
                        UPDATE Accounts SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_categories_timestamp 
                    AFTER UPDATE ON Categories
                    BEGIN
                        UPDATE Categories SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_items_timestamp 
                    AFTER UPDATE ON Items
                    BEGIN
                        UPDATE Items SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_clients_timestamp 
                    AFTER UPDATE ON Clients
                    BEGIN
                        UPDATE Clients SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_invoices_timestamp 
                    AFTER UPDATE ON Invoices
                    BEGIN
                        UPDATE Invoices SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_returns_timestamp 
                    AFTER UPDATE ON Returns
                    BEGIN
                        UPDATE Returns SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_purchases_timestamp 
                    AFTER UPDATE ON Purchases
                    BEGIN
                        UPDATE Purchases SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_expenses_timestamp 
                    AFTER UPDATE ON Expenses
                    BEGIN
                        UPDATE Expenses SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                @"CREATE TRIGGER IF NOT EXISTS update_safes_timestamp 
                    AFTER UPDATE ON Safes
                    BEGIN
                        UPDATE Safes SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END"
            };

            return triggers.All(sql => DatabaseHelper.ExecuteNonQuery(sql) > 0);
        }

        #endregion

        #region Views

        public static bool CreateViews()
        {
            var views = new string[] {
                @"CREATE VIEW IF NOT EXISTS vw_items_with_category AS
                  SELECT i.*, c.Name as CategoryName, c.Type as CategoryType
                  FROM Items i LEFT JOIN Categories c ON i.CategoryId = c.Id",
                @"CREATE VIEW IF NOT EXISTS vw_invoices_with_client AS
                  SELECT inv.*, c.Name as ClientName, c.Phone as ClientPhone, c.Email as ClientEmail
                  FROM Invoices inv LEFT JOIN Clients c ON inv.ClientId = c.Id",
                @"CREATE VIEW IF NOT EXISTS vw_returns_with_client AS
                  SELECT r.*, c.Name as ClientName, c.Phone as ClientPhone, c.Email as ClientEmail
                  FROM Returns r LEFT JOIN Clients c ON r.ClientId = c.Id",
                @"CREATE VIEW IF NOT EXISTS vw_low_stock_items AS
                  SELECT i.*, c.Name as CategoryName
                  FROM Items i LEFT JOIN Categories c ON i.CategoryId = c.Id
                  WHERE i.Quantity <= i.MinStock AND i.IsActive = 1",
                @"CREATE VIEW IF NOT EXISTS vw_sales_summary AS
                  SELECT DATE(InvoiceDate) as SaleDate,
                         COUNT(*) as InvoiceCount,
                         SUM(TotalAmount) as TotalSales,
                         SUM(PaidAmount) as TotalPaid,
                         SUM(TotalAmount - PaidAmount) as TotalUnpaid
                  FROM Invoices
                  WHERE Type = 'Sale'
                  GROUP BY DATE(InvoiceDate)"
            };

            return views.All(sql => DatabaseHelper.ExecuteNonQuery(sql) > 0);
        }

        #endregion

        #region Initialize Complete Schema

        public static bool InitializeCompleteSchema()
        {
            var steps = new (string stepName, Func<bool> stepAction)[] {
                ("إنشاء الجداول", CreateAllTables),
                ("إنشاء الفهارس", CreateIndexes),
                ("إنشاء المحفزات", CreateTriggers),
                ("إنشاء العروض", CreateViews)
            };

            bool allSuccessful = true;
            foreach (var (stepName, stepAction) in steps)
            {
                MeasureExecutionTime(() =>
                {
                    if (!stepAction())
                    {
                        allSuccessful = false;
                        ErrorHandler.LogError($"فشل في: {stepName}", "Schema Creator");
                    }
                    else
                        ErrorHandler.LogInfo($"تم بنجاح: {stepName}", "Schema Creator");
                }, stepName);
            }

            return allSuccessful;
        }

        #endregion

        #region MeasureExecutionTime

        /// <summary>
        /// Measures execution time of a Func<T> and logs it
        /// </summary>
        public static T MeasureExecutionTime<T>(Func<T> func, string operationName = "")
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            T result = func();
            sw.Stop();
            if (!string.IsNullOrEmpty(operationName))
                ErrorHandler.LogInfo($"{operationName} انتهت في: {sw.ElapsedMilliseconds} ms", "Schema Creator");
            return result;
        }

        /// <summary>
        /// Overload: Measures execution time of an Action
        /// </summary>
        public static void MeasureExecutionTime(Action action, string operationName = "")
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            action();
            sw.Stop();
            if (!string.IsNullOrEmpty(operationName))
                ErrorHandler.LogInfo($"{operationName} انتهت في: {sw.ElapsedMilliseconds} ms", "Schema Creator");
        }

        #endregion
    }
}
