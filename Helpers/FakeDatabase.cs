using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    /// <summary>
    /// Fake database for testing and development purposes
    /// </summary>
    public static class FakeDatabase
    {
        private static readonly List<Item> _items = new List<Item>();
        private static readonly List<Client> _clients = new List<Client>();
        private static readonly List<Invoice> _invoices = new List<Invoice>();
        private static readonly List<Category> _categories = new List<Category>();
        private static readonly List<User> _users = new List<User>();
        private static readonly List<Expense> _expenses = new List<Expense>();
        private static readonly List<Purchase> _purchases = new List<Purchase>();
        private static readonly List<Account> _accounts = new List<Account>();
        private static readonly List<Employee> _employees = new List<Employee>();
        private static readonly List<Branch> _branches = new List<Branch>();

        /// <summary>
        /// Initialize fake database with sample data
        /// </summary>
        public static void Initialize()
        {
            try
            {
                SeedFakeData();
                ErrorHandler.LogInfo("تم تهيئة قاعدة البيانات الوهمية", "Fake Database");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initialize Fake Database");
            }
        }

        /// <summary>
        /// Seed fake data
        /// </summary>
        private static void SeedFakeData()
        {
            // Seed categories
            _categories.AddRange(new[]
            {
                new Category { Id = 1, Name = "معدات هيدروليكية", Type = "Hydraulic", IsActive = true },
                new Category { Id = 2, Name = "معدات هوائية", Type = "Pneumatic", IsActive = true },
                new Category { Id = 3, Name = "معدات ميكانيكية", Type = "Mechanical", IsActive = true },
                new Category { Id = 4, Name = "قطع غيار", Type = "Spare Parts", IsActive = true }
            });

            // Seed items
            _items.AddRange(new[]
            {
                new Item { Id = 1, ItemName = "مضخة هيدروليكية 10 لتر/دقيقة", Code = "HYD001", Quantity = 15, SellingPrice = 2500, PurchasePrice = 1800, MinStock = 5, Unit = "قطعة", CategoryId = 1 },
                new Item { Id = 2, ItemName = "أسطوانة هيدروليكية 50 طن", Code = "HYD002", Quantity = 8, SellingPrice = 3500, PurchasePrice = 2500, MinStock = 3, Unit = "قطعة", CategoryId = 1 },
                new Item { Id = 3, ItemName = "ضاغط هواء 100 لتر", Code = "PNM001", Quantity = 12, SellingPrice = 1800, PurchasePrice = 1200, MinStock = 4, Unit = "قطعة", CategoryId = 2 },
                new Item { Id = 4, ItemName = "محرك كهربائي 5 حصان", Code = "MCH001", Quantity = 20, SellingPrice = 1200, PurchasePrice = 800, MinStock = 6, Unit = "قطعة", CategoryId = 3 },
                new Item { Id = 5, ItemName = "فلتر هيدروليكي", Code = "SPR001", Quantity = 50, SellingPrice = 150, PurchasePrice = 80, MinStock = 10, Unit = "قطعة", CategoryId = 4 }
            });

            // Seed clients
            _clients.AddRange(new[]
            {
                new Client { Id = 1, Name = "شركة البناء الحديث", Phone = "01234567890", Email = "info@modernconstruction.com", Address = "شارع الملك فهد، الرياض", Balance = 15000, TaxNumber = "123456789", CommercialRecord = "CR123456" },
                new Client { Id = 2, Name = "مؤسسة الصناعات الثقيلة", Phone = "01234567891", Email = "info@heavyindustries.com", Address = "المنطقة الصناعية، جدة", Balance = 25000, TaxNumber = "987654321", CommercialRecord = "CR987654" },
                new Client { Id = 3, Name = "شركة النقل السريع", Phone = "01234567892", Email = "info@fasttransport.com", Address = "شارع العليا، الرياض", Balance = 8000, TaxNumber = "456789123", CommercialRecord = "CR456789" }
            });

            // Seed users
            _users.AddRange(new[]
            {
                new User { Id = 1, Username = "admin", FullName = "مدير النظام", Email = "admin@etag.com", Phone = "01234567890", Role = "Admin", IsAdmin = true, IsActive = true },
                new User { Id = 2, Username = "manager", FullName = "مدير المبيعات", Email = "manager@etag.com", Phone = "01234567891", Role = "Manager", IsAdmin = false, IsActive = true },
                new User { Id = 3, Username = "cashier", FullName = "أمين الصندوق", Email = "cashier@etag.com", Phone = "01234567892", Role = "Cashier", IsAdmin = false, IsActive = true }
            });

            // Seed accounts
            _accounts.AddRange(new[]
            {
                new Account { Id = 1, Name = "الصندوق", Type = "Asset", Balance = 50000, Description = "النقدية في الصندوق" },
                new Account { Id = 2, Name = "البنك الأهلي", Type = "Asset", Balance = 100000, Description = "الحساب الجاري في البنك الأهلي" },
                new Account { Id = 3, Name = "المبيعات", Type = "Revenue", Balance = 0, Description = "حساب المبيعات" },
                new Account { Id = 4, Name = "المشتريات", Type = "Expense", Balance = 0, Description = "حساب المشتريات" }
            });

            // Seed employees
            _employees.AddRange(new[]
            {
                new Employee { Id = 1, FullName = "أحمد محمد", JobTitle = "مدير المبيعات", Salary = 8000, HireDate = DateTime.Now.AddYears(-2), Phone = "01234567890", Email = "ahmed@etag.com" },
                new Employee { Id = 2, FullName = "فاطمة علي", JobTitle = "محاسبة", Salary = 6000, HireDate = DateTime.Now.AddYears(-1), Phone = "01234567891", Email = "fatima@etag.com" },
                new Employee { Id = 3, FullName = "محمد حسن", JobTitle = "مندوب مبيعات", Salary = 5000, HireDate = DateTime.Now.AddMonths(-6), Phone = "01234567892", Email = "mohamed@etag.com" }
            });

            // Seed branches
            _branches.AddRange(new[]
            {
                new Branch { Id = 1, Name = "الفرع الرئيسي", Address = "شارع الملك فهد، الرياض", Phone = "01234567890", Type = "Main", IsActive = true },
                new Branch { Id = 2, Name = "فرع جدة", Address = "المنطقة الصناعية، جدة", Phone = "01234567891", Type = "Branch", IsActive = true },
                new Branch { Id = 3, Name = "فرع الدمام", Address = "شارع الملك عبدالعزيز، الدمام", Phone = "01234567892", Type = "Branch", IsActive = true }
            });

            // Seed expenses
            _expenses.AddRange(new[]
            {
                new Expense { Id = 1, ExpenseType = "إيجار", Description = "إيجار المستودع", Amount = 3000, ExpenseDate = DateTime.Now.AddDays(-15), Category = "تشغيلية" },
                new Expense { Id = 2, ExpenseType = "كهرباء", Description = "فاتورة الكهرباء", Amount = 1500, ExpenseDate = DateTime.Now.AddDays(-10), Category = "تشغيلية" },
                new Expense { Id = 3, ExpenseType = "راتب", Description = "رواتب الموظفين", Amount = 8000, ExpenseDate = DateTime.Now.AddDays(-5), Category = "رواتب" }
            });

            // Seed purchases
            _purchases.AddRange(new[]
            {
                new Purchase { Id = 1, PurchaseNumber = "PUR001", Supplier = "مورد المعدات الهيدروليكية", PurchaseDate = DateTime.Now.AddDays(-10), Total = 15000, Paid = true, Notes = "شراء معدات هيدروليكية" },
                new Purchase { Id = 2, PurchaseNumber = "PUR002", Supplier = "شركة قطع الغيار", PurchaseDate = DateTime.Now.AddDays(-7), Total = 5000, Paid = true, Notes = "شراء قطع غيار" }
            });

            // Seed invoices
            var client1 = _clients.First();
            var client2 = _clients.Skip(1).First();
            
            _invoices.AddRange(new[]
            {
                new Invoice { Id = 1, InvoiceNumber = "INV001", ClientId = client1.Id, ClientName = client1.Name, InvoiceDate = DateTime.Now.AddDays(-5), TotalAmount = 5000, PaidAmount = 3000, Status = "Partial", Type = "Sale", Notes = "فاتورة مبيعات" },
                new Invoice { Id = 2, InvoiceNumber = "INV002", ClientId = client2.Id, ClientName = client2.Name, InvoiceDate = DateTime.Now.AddDays(-3), TotalAmount = 7500, PaidAmount = 7500, Status = "Paid", Type = "Sale", Notes = "فاتورة مبيعات مدفوعة" }
            });
        }

        /// <summary>
        /// Get all items
        /// </summary>
        public static List<Item> GetAllItems()
        {
            return _items.ToList();
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        public static List<Client> GetAllClients()
        {
            return _clients.ToList();
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        public static List<Invoice> GetAllInvoices()
        {
            return _invoices.ToList();
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        public static List<Category> GetAllCategories()
        {
            return _categories.ToList();
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public static List<User> GetAllUsers()
        {
            return _users.ToList();
        }

        /// <summary>
        /// Get all expenses
        /// </summary>
        public static List<Expense> GetAllExpenses()
        {
            return _expenses.ToList();
        }

        /// <summary>
        /// Get all purchases
        /// </summary>
        public static List<Purchase> GetAllPurchases()
        {
            return _purchases.ToList();
        }

        /// <summary>
        /// Get all accounts
        /// </summary>
        public static List<Account> GetAllAccounts()
        {
            return _accounts.ToList();
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        public static List<Employee> GetAllEmployees()
        {
            return _employees.ToList();
        }

        /// <summary>
        /// Get all branches
        /// </summary>
        public static List<Branch> GetAllBranches()
        {
            return _branches.ToList();
        }

        /// <summary>
        /// Add item
        /// </summary>
        public static bool AddItem(Item item)
        {
            try
            {
                item.Id = _items.Count + 1;
                _items.Add(item);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Item to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add client
        /// </summary>
        public static bool AddClient(Client client)
        {
            try
            {
                client.Id = _clients.Count + 1;
                _clients.Add(client);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Client to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add invoice
        /// </summary>
        public static bool AddInvoice(Invoice invoice)
        {
            try
            {
                invoice.Id = _invoices.Count + 1;
                _invoices.Add(invoice);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Invoice to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add category
        /// </summary>
        public static bool AddCategory(Category category)
        {
            try
            {
                category.Id = _categories.Count + 1;
                _categories.Add(category);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Category to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add user
        /// </summary>
        public static bool AddUser(User user)
        {
            try
            {
                user.Id = _users.Count + 1;
                _users.Add(user);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add User to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add expense
        /// </summary>
        public static bool AddExpense(Expense expense)
        {
            try
            {
                expense.Id = _expenses.Count + 1;
                _expenses.Add(expense);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Expense to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add purchase
        /// </summary>
        public static bool AddPurchase(Purchase purchase)
        {
            try
            {
                purchase.Id = _purchases.Count + 1;
                _purchases.Add(purchase);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Purchase to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add account
        /// </summary>
        public static bool AddAccount(Account account)
        {
            try
            {
                account.Id = _accounts.Count + 1;
                _accounts.Add(account);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Account to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add employee
        /// </summary>
        public static bool AddEmployee(Employee employee)
        {
            try
            {
                employee.Id = _employees.Count + 1;
                _employees.Add(employee);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Employee to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Add branch
        /// </summary>
        public static bool AddBranch(Branch branch)
        {
            try
            {
                branch.Id = _branches.Count + 1;
                _branches.Add(branch);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Add Branch to Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Update item
        /// </summary>
        public static bool UpdateItem(Item item)
        {
            try
            {
                var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    var index = _items.IndexOf(existingItem);
                    _items[index] = item;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Update Item in Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Update client
        /// </summary>
        public static bool UpdateClient(Client client)
        {
            try
            {
                var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
                if (existingClient != null)
                {
                    var index = _clients.IndexOf(existingClient);
                    _clients[index] = client;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Update Client in Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        public static bool DeleteItem(int itemId)
        {
            try
            {
                var item = _items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    _items.Remove(item);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Delete Item from Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Delete client
        /// </summary>
        public static bool DeleteClient(int clientId)
        {
            try
            {
                var client = _clients.FirstOrDefault(c => c.Id == clientId);
                if (client != null)
                {
                    _clients.Remove(client);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Delete Client from Fake Database");
                return false;
            }
        }

        /// <summary>
        /// Search items
        /// </summary>
        public static List<Item> SearchItems(string searchTerm)
        {
            try
            {
                return _items.Where(i => 
                    i.ItemName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Search Items in Fake Database");
                return new List<Item>();
            }
        }

        /// <summary>
        /// Search clients
        /// </summary>
        public static List<Client> SearchClients(string searchTerm)
        {
            try
            {
                return _clients.Where(c => 
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Search Clients in Fake Database");
                return new List<Client>();
            }
        }

        /// <summary>
        /// Get item by ID
        /// </summary>
        public static Item? GetItemById(int itemId)
        {
            return _items.FirstOrDefault(i => i.Id == itemId);
        }

        /// <summary>
        /// Get client by ID
        /// </summary>
        public static Client? GetClientById(int clientId)
        {
            return _clients.FirstOrDefault(c => c.Id == clientId);
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        public static Invoice? GetInvoiceById(int invoiceId)
        {
            return _invoices.FirstOrDefault(i => i.Id == invoiceId);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        public static Category? GetCategoryById(int categoryId)
        {
            return _categories.FirstOrDefault(c => c.Id == categoryId);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        public static User? GetUserById(int userId)
        {
            return _users.FirstOrDefault(u => u.Id == userId);
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        public static User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public static void ClearAllData()
        {
            try
            {
                _items.Clear();
                _clients.Clear();
                _invoices.Clear();
                _categories.Clear();
                _users.Clear();
                _expenses.Clear();
                _purchases.Clear();
                _accounts.Clear();
                _employees.Clear();
                _branches.Clear();
                
                ErrorHandler.LogInfo("تم مسح جميع البيانات من قاعدة البيانات الوهمية", "Fake Database");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Clear All Data from Fake Database");
            }
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        public static FakeDatabaseStatistics GetStatistics()
        {
            return new FakeDatabaseStatistics
            {
                ItemsCount = _items.Count,
                ClientsCount = _clients.Count,
                InvoicesCount = _invoices.Count,
                CategoriesCount = _categories.Count,
                UsersCount = _users.Count,
                ExpensesCount = _expenses.Count,
                PurchasesCount = _purchases.Count,
                AccountsCount = _accounts.Count,
                EmployeesCount = _employees.Count,
                BranchesCount = _branches.Count
            };
        }
    }

    /// <summary>
    /// Fake database statistics class
    /// </summary>
    public class FakeDatabaseStatistics
    {
        public int ItemsCount { get; set; }
        public int ClientsCount { get; set; }
        public int InvoicesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int UsersCount { get; set; }
        public int ExpensesCount { get; set; }
        public int PurchasesCount { get; set; }
        public int AccountsCount { get; set; }
        public int EmployeesCount { get; set; }
        public int BranchesCount { get; set; }
    }
}