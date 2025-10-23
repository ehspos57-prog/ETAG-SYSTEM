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
        /// <summary>
        /// Create all database tables
        /// </summary>
        public static bool CreateAllTables()
        {
            try
            {
                var tables = new[]
                {
                    CreateUsersTable(),
                    CreatePermissionsTable(),
                    CreateBranchesTable(),
                    CreateEmployeesTable(),
                    CreateAccountsTable(),
                    CreateCategoriesTable(),
                    CreateItemsTable(),
                    CreateClientsTable(),
                    CreateInvoicesTable(),
                    CreateInvoiceItemsTable(),
                    CreateReturnsTable(),
                    CreateReturnItemsTable(),
                    CreatePurchasesTable(),
                    CreatePurchaseItemsTable(),
                    CreateExpensesTable(),
                    CreateSafesTable(),
                    CreateSettingsTable(),
                    CreateLogsTable()
                };

                var allCreated = tables.All(t => t);
                
                if (allCreated)
                {
                    ErrorHandler.LogInfo("تم إنشاء جميع الجداول بنجاح", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("فشل في إنشاء بعض الجداول", "Schema Creator");
                }

                return allCreated;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create All Tables");
                return false;
            }
        }

        /// <summary>
        /// Create users table
        /// </summary>
        public static bool CreateUsersTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Users Table");
                return false;
            }
        }

        /// <summary>
        /// Create permissions table
        /// </summary>
        public static bool CreatePermissionsTable()
        {
            try
            {
                var query = @"
                    CREATE TABLE IF NOT EXISTS Permissions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        PermissionName TEXT NOT NULL,
                        IsGranted BOOLEAN DEFAULT 1,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Permissions Table");
                return false;
            }
        }

        /// <summary>
        /// Create branches table
        /// </summary>
        public static bool CreateBranchesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Branches Table");
                return false;
            }
        }

        /// <summary>
        /// Create employees table
        /// </summary>
        public static bool CreateEmployeesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Employees Table");
                return false;
            }
        }

        /// <summary>
        /// Create accounts table
        /// </summary>
        public static bool CreateAccountsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Accounts Table");
                return false;
            }
        }

        /// <summary>
        /// Create categories table
        /// </summary>
        public static bool CreateCategoriesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Categories Table");
                return false;
            }
        }

        /// <summary>
        /// Create items table
        /// </summary>
        public static bool CreateItemsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Items Table");
                return false;
            }
        }

        /// <summary>
        /// Create clients table
        /// </summary>
        public static bool CreateClientsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Clients Table");
                return false;
            }
        }

        /// <summary>
        /// Create invoices table
        /// </summary>
        public static bool CreateInvoicesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Invoices Table");
                return false;
            }
        }

        /// <summary>
        /// Create invoice items table
        /// </summary>
        public static bool CreateInvoiceItemsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Invoice Items Table");
                return false;
            }
        }

        /// <summary>
        /// Create returns table
        /// </summary>
        public static bool CreateReturnsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Returns Table");
                return false;
            }
        }

        /// <summary>
        /// Create return items table
        /// </summary>
        public static bool CreateReturnItemsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Return Items Table");
                return false;
            }
        }

        /// <summary>
        /// Create purchases table
        /// </summary>
        public static bool CreatePurchasesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Purchases Table");
                return false;
            }
        }

        /// <summary>
        /// Create purchase items table
        /// </summary>
        public static bool CreatePurchaseItemsTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Purchase Items Table");
                return false;
            }
        }

        /// <summary>
        /// Create expenses table
        /// </summary>
        public static bool CreateExpensesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Expenses Table");
                return false;
            }
        }

        /// <summary>
        /// Create safes table
        /// </summary>
        public static bool CreateSafesTable()
        {
            try
            {
                var query = @"
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
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Safes Table");
                return false;
            }
        }

        /// <summary>
        /// Create settings table
        /// </summary>
        public static bool CreateSettingsTable()
        {
            try
            {
                var query = @"
                    CREATE TABLE IF NOT EXISTS Settings (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Key TEXT UNIQUE NOT NULL,
                        Value TEXT,
                        Description TEXT,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Settings Table");
                return false;
            }
        }

        /// <summary>
        /// Create logs table
        /// </summary>
        public static bool CreateLogsTable()
        {
            try
            {
                var query = @"
                    CREATE TABLE IF NOT EXISTS Logs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Level TEXT NOT NULL,
                        Message TEXT NOT NULL,
                        Exception TEXT,
                        Source TEXT,
                        UserId INTEGER,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    )";

                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Logs Table");
                return false;
            }
        }

        /// <summary>
        /// Create indexes for better performance
        /// </summary>
        public static bool CreateIndexes()
        {
            try
            {
                var indexes = new[]
                {
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

                var allCreated = indexes.All(index => DatabaseHelper.ExecuteNonQuery(index));
                
                if (allCreated)
                {
                    ErrorHandler.LogInfo("تم إنشاء جميع الفهارس بنجاح", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("فشل في إنشاء بعض الفهارس", "Schema Creator");
                }

                return allCreated;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Indexes");
                return false;
            }
        }

        /// <summary>
        /// Create triggers for automatic timestamp updates
        /// </summary>
        public static bool CreateTriggers()
        {
            try
            {
                var triggers = new[]
                {
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_users_timestamp 
                    AFTER UPDATE ON Users
                    BEGIN
                        UPDATE Users SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_branches_timestamp 
                    AFTER UPDATE ON Branches
                    BEGIN
                        UPDATE Branches SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_employees_timestamp 
                    AFTER UPDATE ON Employees
                    BEGIN
                        UPDATE Employees SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_accounts_timestamp 
                    AFTER UPDATE ON Accounts
                    BEGIN
                        UPDATE Accounts SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_categories_timestamp 
                    AFTER UPDATE ON Categories
                    BEGIN
                        UPDATE Categories SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_items_timestamp 
                    AFTER UPDATE ON Items
                    BEGIN
                        UPDATE Items SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_clients_timestamp 
                    AFTER UPDATE ON Clients
                    BEGIN
                        UPDATE Clients SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_invoices_timestamp 
                    AFTER UPDATE ON Invoices
                    BEGIN
                        UPDATE Invoices SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_returns_timestamp 
                    AFTER UPDATE ON Returns
                    BEGIN
                        UPDATE Returns SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_purchases_timestamp 
                    AFTER UPDATE ON Purchases
                    BEGIN
                        UPDATE Purchases SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_expenses_timestamp 
                    AFTER UPDATE ON Expenses
                    BEGIN
                        UPDATE Expenses SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END",
                    @"
                    CREATE TRIGGER IF NOT EXISTS update_safes_timestamp 
                    AFTER UPDATE ON Safes
                    BEGIN
                        UPDATE Safes SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
                    END"
                };

                var allCreated = triggers.All(trigger => DatabaseHelper.ExecuteNonQuery(trigger));
                
                if (allCreated)
                {
                    ErrorHandler.LogInfo("تم إنشاء جميع المحفزات بنجاح", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("فشل في إنشاء بعض المحفزات", "Schema Creator");
                }

                return allCreated;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Triggers");
                return false;
            }
        }

        /// <summary>
        /// Create views for common queries
        /// </summary>
        public static bool CreateViews()
        {
            try
            {
                var views = new[]
                {
                    @"
                    CREATE VIEW IF NOT EXISTS vw_items_with_category AS
                    SELECT 
                        i.*,
                        c.Name as CategoryName,
                        c.Type as CategoryType
                    FROM Items i
                    LEFT JOIN Categories c ON i.CategoryId = c.Id",
                    @"
                    CREATE VIEW IF NOT EXISTS vw_invoices_with_client AS
                    SELECT 
                        inv.*,
                        c.Name as ClientName,
                        c.Phone as ClientPhone,
                        c.Email as ClientEmail
                    FROM Invoices inv
                    LEFT JOIN Clients c ON inv.ClientId = c.Id",
                    @"
                    CREATE VIEW IF NOT EXISTS vw_returns_with_client AS
                    SELECT 
                        r.*,
                        c.Name as ClientName,
                        c.Phone as ClientPhone,
                        c.Email as ClientEmail
                    FROM Returns r
                    LEFT JOIN Clients c ON r.ClientId = c.Id",
                    @"
                    CREATE VIEW IF NOT EXISTS vw_low_stock_items AS
                    SELECT 
                        i.*,
                        c.Name as CategoryName
                    FROM Items i
                    LEFT JOIN Categories c ON i.CategoryId = c.Id
                    WHERE i.Quantity <= i.MinStock AND i.IsActive = 1",
                    @"
                    CREATE VIEW IF NOT EXISTS vw_sales_summary AS
                    SELECT 
                        DATE(InvoiceDate) as SaleDate,
                        COUNT(*) as InvoiceCount,
                        SUM(TotalAmount) as TotalSales,
                        SUM(PaidAmount) as TotalPaid,
                        SUM(TotalAmount - PaidAmount) as TotalUnpaid
                    FROM Invoices
                    WHERE Type = 'Sale'
                    GROUP BY DATE(InvoiceDate)"
                };

                var allCreated = views.All(view => DatabaseHelper.ExecuteNonQuery(view));
                
                if (allCreated)
                {
                    ErrorHandler.LogInfo("تم إنشاء جميع العروض بنجاح", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("فشل في إنشاء بعض العروض", "Schema Creator");
                }

                return allCreated;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Views");
                return false;
            }
        }

        /// <summary>
        /// Initialize complete database schema
        /// </summary>
        public static bool InitializeCompleteSchema()
        {
            try
            {
                ErrorHandler.LogInfo("بدء تهيئة قاعدة البيانات الكاملة", "Schema Creator");

                var steps = new[]
                {
                    ("إنشاء الجداول", CreateAllTables),
                    ("إنشاء الفهارس", CreateIndexes),
                    ("إنشاء المحفزات", CreateTriggers),
                    ("إنشاء العروض", CreateViews)
                };

                var allSuccessful = true;
                foreach (var (stepName, stepAction) in steps)
                {
                    ErrorHandler.LogInfo($"تنفيذ: {stepName}", "Schema Creator");
                    if (!stepAction())
                    {
                        ErrorHandler.LogError($"فشل في: {stepName}", "Schema Creator");
                        allSuccessful = false;
                    }
                }

                if (allSuccessful)
                {
                    ErrorHandler.LogInfo("تم تهيئة قاعدة البيانات الكاملة بنجاح", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("تم تهيئة قاعدة البيانات مع بعض الأخطاء", "Schema Creator");
                }

                return allSuccessful;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initialize Complete Schema");
                return false;
            }
        }

        /// <summary>
        /// Check if table exists
        /// </summary>
        public static bool TableExists(string tableName)
        {
            try
            {
                var query = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                var result = DatabaseHelper.ExecuteScalar(query, new Dictionary<string, object> { { "@tableName", tableName } });
                return result != null;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Check Table Exists: {tableName}");
                return false;
            }
        }

        /// <summary>
        /// Get all table names
        /// </summary>
        public static List<string> GetAllTableNames()
        {
            try
            {
                var query = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";
                var dataTable = DatabaseHelper.GetDataTable(query);
                return dataTable.Rows.Cast<DataRow>().Select(row => row["name"].ToString()).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get All Table Names");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get table schema
        /// </summary>
        public static DataTable GetTableSchema(string tableName)
        {
            try
            {
                var query = $"PRAGMA table_info({tableName})";
                return DatabaseHelper.GetDataTable(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Get Table Schema: {tableName}");
                return new DataTable();
            }
        }

        /// <summary>
        /// Drop table
        /// </summary>
        public static bool DropTable(string tableName)
        {
            try
            {
                var query = $"DROP TABLE IF EXISTS {tableName}";
                return DatabaseHelper.ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, $"Drop Table: {tableName}");
                return false;
            }
        }

        /// <summary>
        /// Backup database schema
        /// </summary>
        public static bool BackupSchema(string backupPath)
        {
            try
            {
                var schema = new List<string>();
                var tables = GetAllTableNames();
                
                foreach (var table in tables)
                {
                    var createTableQuery = $"SELECT sql FROM sqlite_master WHERE type='table' AND name='{table}'";
                    var result = DatabaseHelper.ExecuteScalar(createTableQuery);
                    if (result != null)
                    {
                        schema.Add(result.ToString());
                    }
                }

                File.WriteAllLines(backupPath, schema);
                ErrorHandler.LogInfo($"تم إنشاء نسخة احتياطية من هيكل قاعدة البيانات في: {backupPath}", "Schema Creator");
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Backup Schema");
                return false;
            }
        }

        /// <summary>
        /// Restore database schema
        /// </summary>
        public static bool RestoreSchema(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    ErrorHandler.LogError($"ملف النسخة الاحتياطية غير موجود: {backupPath}", "Schema Creator");
                    return false;
                }

                var schema = File.ReadAllLines(backupPath);
                var allSuccessful = true;

                foreach (var sql in schema)
                {
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        if (!DatabaseHelper.ExecuteNonQuery(sql))
                        {
                            allSuccessful = false;
                        }
                    }
                }

                if (allSuccessful)
                {
                    ErrorHandler.LogInfo($"تم استعادة هيكل قاعدة البيانات من: {backupPath}", "Schema Creator");
                }
                else
                {
                    ErrorHandler.LogWarning("تم استعادة هيكل قاعدة البيانات مع بعض الأخطاء", "Schema Creator");
                }

                return allSuccessful;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Restore Schema");
                return false;
            }
        }
    }
}