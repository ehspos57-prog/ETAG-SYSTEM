using ETAG_ERP.Models;
using System;
using System.Collections.Generic;

namespace ETAG_ERP.Helpers
{
    public static class SampleDataSeeder
    {
        public static void SeedAllSampleData()
        {
            try
            {
                ErrorHandler.LogInfo("بدء إدخال البيانات التجريبية", "Sample Data Seeding");

                // Seed Categories
                SeedCategories();
                
                // Seed Users
                SeedUsers();
                
                // Seed Items
                SeedItems();
                
                // Seed Clients
                SeedClients();
                
                // Seed Accounts
                SeedAccounts();
                
                // Seed Invoices
                SeedInvoices();
                
                // Seed Purchases
                SeedPurchases();
                
                // Seed Expenses
                SeedExpenses();

                ErrorHandler.LogInfo("تم إدخال جميع البيانات التجريبية بنجاح", "Sample Data Seeding");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Sample Data Seeding");
            }
        }

        private static void SeedCategories()
        {
            try
            {
                var categories = new List<Category>
                {
                    new Category { Name = "معدات هيدروليكية", Type = "Hydraulic", Description = "معدات الهيدروليك", IsActive = true, Level1 = "معدات", Level2 = "هيدروليك", Level3 = "", Level4 = "", Level5 = "", Code = "HYD001" },
                    new Category { Name = "معدات هوائية", Type = "Pneumatic", Description = "معدات الهواء المضغوط", IsActive = true, Level1 = "معدات", Level2 = "هوائية", Level3 = "", Level4 = "", Level5 = "", Code = "PNM001" },
                    new Category { Name = "معدات ميكانيكية", Type = "Mechanical", Description = "معدات ميكانيكية", IsActive = true, Level1 = "معدات", Level2 = "ميكانيكية", Level3 = "", Level4 = "", Level5 = "", Code = "MCH001" },
                    new Category { Name = "قطع غيار", Type = "Spare Parts", Description = "قطع غيار للمعدات", IsActive = true, Level1 = "قطع غيار", Level2 = "", Level3 = "", Level4 = "", Level5 = "", Code = "SPR001" },
                    new Category { Name = "أدوات", Type = "Tools", Description = "أدوات يدوية وآلية", IsActive = true, Level1 = "أدوات", Level2 = "", Level3 = "", Level4 = "", Level5 = "", Code = "TLS001" }
                };

                foreach (var category in categories)
                {
                    category.CreatedAt = DateTime.Now;
                    category.UpdatedAt = DateTime.Now;
                    DatabaseHelper.InsertCategory(category);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Categories");
            }
        }

        private static void SeedUsers()
        {
            try
            {
                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        FullName = "مدير النظام",
                        Email = "admin@etag.com",
                        Phone = "01234567890",
                        Role = "Admin",
                        IsAdmin = true,
                        IsActive = true,
                        PasswordHash = HashPassword("123456"),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "manager",
                        FullName = "مدير المبيعات",
                        Email = "manager@etag.com",
                        Phone = "01234567891",
                        Role = "Manager",
                        IsAdmin = false,
                        IsActive = true,
                        PasswordHash = HashPassword("123456"),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new User
                    {
                        Username = "cashier",
                        FullName = "أمين الصندوق",
                        Email = "cashier@etag.com",
                        Phone = "01234567892",
                        Role = "Cashier",
                        IsAdmin = false,
                        IsActive = true,
                        PasswordHash = HashPassword("123456"),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                foreach (var user in users)
                {
                    DatabaseHelper_Extensions.InsertUser(user);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Users");
            }
        }

        private static void SeedItems()
        {
            try
            {
                var categories = DatabaseHelper.GetAllCategories();
                var hydraulicCategory = categories.FirstOrDefault(c => c.Type == "Hydraulic");
                var pneumaticCategory = categories.FirstOrDefault(c => c.Type == "Pneumatic");
                var mechanicalCategory = categories.FirstOrDefault(c => c.Type == "Mechanical");

                var items = new List<Item>
                {
                    new Item
                    {
                        ItemCode = "HYD001",
                        ItemName = "مضخة هيدروليكية 10 لتر/دقيقة",
                        Description = "مضخة هيدروليكية عالية الضغط",
                        CategoryId = hydraulicCategory?.Id ?? 1,
                        CategoryName = hydraulicCategory?.Name ?? "معدات هيدروليكية",
                        SellingPrice1 = 2500.00m,
                        SellingPrice2 = 2300.00m,
                        SellingPrice3 = 2200.00m,
                        CostPrice = 1800.00m,
                        StockQuantity = 15,
                        Unit = "قطعة",
                        MinQuantity = 5,
                        Barcode = "1234567890123",
                        TaxRate = 14.00m,
                        DiscountRate = 5.00m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Item
                    {
                        ItemCode = "HYD002",
                        ItemName = "أسطوانة هيدروليكية 50 طن",
                        Description = "أسطوانة هيدروليكية للرفع الثقيل",
                        CategoryId = hydraulicCategory?.Id ?? 1,
                        CategoryName = hydraulicCategory?.Name ?? "معدات هيدروليكية",
                        SellingPrice1 = 3500.00m,
                        SellingPrice2 = 3200.00m,
                        SellingPrice3 = 3000.00m,
                        CostPrice = 2500.00m,
                        StockQuantity = 8,
                        Unit = "قطعة",
                        MinQuantity = 3,
                        Barcode = "1234567890124",
                        TaxRate = 14.00m,
                        DiscountRate = 5.00m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Item
                    {
                        ItemCode = "PNM001",
                        ItemName = "ضاغط هواء 100 لتر",
                        Description = "ضاغط هواء للاستخدام الصناعي",
                        CategoryId = pneumaticCategory?.Id ?? 2,
                        CategoryName = pneumaticCategory?.Name ?? "معدات هوائية",
                        SellingPrice1 = 1800.00m,
                        SellingPrice2 = 1650.00m,
                        SellingPrice3 = 1500.00m,
                        CostPrice = 1200.00m,
                        StockQuantity = 12,
                        Unit = "قطعة",
                        MinQuantity = 4,
                        Barcode = "1234567890125",
                        TaxRate = 14.00m,
                        DiscountRate = 5.00m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Item
                    {
                        ItemCode = "MCH001",
                        ItemName = "محرك كهربائي 5 حصان",
                        Description = "محرك كهربائي ثلاثي الطور",
                        CategoryId = mechanicalCategory?.Id ?? 3,
                        CategoryName = mechanicalCategory?.Name ?? "معدات ميكانيكية",
                        SellingPrice1 = 1200.00m,
                        SellingPrice2 = 1100.00m,
                        SellingPrice3 = 1000.00m,
                        CostPrice = 800.00m,
                        StockQuantity = 20,
                        Unit = "قطعة",
                        MinQuantity = 6,
                        Barcode = "1234567890126",
                        TaxRate = 14.00m,
                        DiscountRate = 5.00m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Item
                    {
                        ItemCode = "SPR001",
                        ItemName = "فلتر هيدروليكي",
                        Description = "فلتر تنقية الزيت الهيدروليكي",
                        CategoryId = categories.FirstOrDefault(c => c.Type == "Spare Parts")?.Id ?? 4,
                        CategoryName = "قطع غيار",
                        SellingPrice1 = 150.00m,
                        SellingPrice2 = 140.00m,
                        SellingPrice3 = 130.00m,
                        CostPrice = 80.00m,
                        StockQuantity = 50,
                        Unit = "قطعة",
                        MinQuantity = 10,
                        Barcode = "1234567890127",
                        TaxRate = 14.00m,
                        DiscountRate = 5.00m,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                foreach (var item in items)
                {
                    DatabaseHelper_Extensions.InsertItem(item);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Items");
            }
        }

        private static void SeedClients()
        {
            try
            {
                var clients = new List<Client>
                {
                    new Client
                    {
                        Name = "شركة البناء الحديث",
                        Phone = "01234567890",
                        Email = "info@modernconstruction.com",
                        Address = "شارع الملك فهد، الرياض",
                        Balance = 15000.00m,
                        TaxNumber = "123456789",
                        CommercialRecord = "CR123456",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Client
                    {
                        Name = "مؤسسة الصناعات الثقيلة",
                        Phone = "01234567891",
                        Email = "info@heavyindustries.com",
                        Address = "المنطقة الصناعية، جدة",
                        Balance = 25000.00m,
                        TaxNumber = "987654321",
                        CommercialRecord = "CR987654",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Client
                    {
                        Name = "شركة النقل السريع",
                        Phone = "01234567892",
                        Email = "info@fasttransport.com",
                        Address = "شارع العليا، الرياض",
                        Balance = 8000.00m,
                        TaxNumber = "456789123",
                        CommercialRecord = "CR456789",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                foreach (var client in clients)
                {
                    DatabaseHelper_Extensions.InsertClient(client);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Clients");
            }
        }

        private static void SeedAccounts()
        {
            try
            {
                var accounts = new List<Account>
                {
                    new Account
                    {
                        Name = "الصندوق",
                        Type = "Asset",
                        Balance = 50000.00m,
                        Description = "النقدية في الصندوق",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Account
                    {
                        Name = "البنك الأهلي",
                        Type = "Asset",
                        Balance = 100000.00m,
                        Description = "الحساب الجاري في البنك الأهلي",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Account
                    {
                        Name = "المبيعات",
                        Type = "Revenue",
                        Balance = 0.00m,
                        Description = "حساب المبيعات",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Account
                    {
                        Name = "المشتريات",
                        Type = "Expense",
                        Balance = 0.00m,
                        Description = "حساب المشتريات",
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                foreach (var account in accounts)
                {
                    DatabaseHelper_Extensions.InsertAccount(account);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Accounts");
            }
        }

        private static void SeedInvoices()
        {
            try
            {
                    var clients = DatabaseHelper_Extensions.GetAllClients();
                var client = clients.FirstOrDefault();

                if (client != null)
                {
                    var invoices = new List<Invoice>
                    {
                        new Invoice
                        {
                            InvoiceNumber = "INV001",
                            ClientId = client.Id,
                            ClientName = client.Name,
                            Date = DateTime.Now.AddDays(-5),
                            TotalAmount = 5000.00m,
                            PaidAmount = 3000.00m,
                            Status = "Partial",
                            Type = "Sale",
                            Notes = "فاتورة مبيعات",
                            CreatedAt = DateTime.Now.AddDays(-5),
                            UpdatedAt = DateTime.Now.AddDays(-5)
                        },
                        new Invoice
                        {
                            InvoiceNumber = "INV002",
                            ClientId = client.Id,
                            ClientName = client.Name,
                            Date = DateTime.Now.AddDays(-3),
                            TotalAmount = 7500.00m,
                            PaidAmount = 7500.00m,
                            Status = "Paid",
                            Type = "Sale",
                            Notes = "فاتورة مبيعات مدفوعة",
                            CreatedAt = DateTime.Now.AddDays(-3),
                            UpdatedAt = DateTime.Now.AddDays(-3)
                        }
                    };

                    foreach (var invoice in invoices)
                    {
                        DatabaseHelper_Extensions.InsertInvoice(invoice);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Invoices");
            }
        }

        private static void SeedPurchases()
        {
            try
            {
                var purchases = new List<Purchase>
                {
                    new Purchase
                    {
                        PurchaseNumber = "PUR001",
                        Supplier = "مورد المعدات الهيدروليكية",
                        Date = DateTime.Now.AddDays(-10),
                        Total = (double)15000.00m,
                        Paid = true,
                        Notes = "شراء معدات هيدروليكية",
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now.AddDays(-10)
                    },
                    new Purchase
                    {
                        PurchaseNumber = "PUR002",
                        Supplier = "شركة قطع الغيار",
                        Date = DateTime.Now.AddDays(-7),
                        Total = (double)5000.00m,
                        Paid = true,
                        Notes = "شراء قطع غيار",
                        CreatedAt = DateTime.Now.AddDays(-7),
                        UpdatedAt = DateTime.Now.AddDays(-7)
                    }
                };

                foreach (var purchase in purchases)
                {
                    DatabaseHelper_Extensions.InsertPurchase(purchase);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Purchases");
            }
        }

        private static void SeedExpenses()
        {
            try
            {
                var expenses = new List<Expense>
                {
                    new Expense
                    {
                        ExpenseType = "إيجار",
                        Description = "إيجار المستودع",
                        Amount = 3000.00m,
                        Category = "تشغيلية",
                        ExpenseDate = DateTime.Now.AddDays(-15),
                        CreatedAt = DateTime.Now.AddDays(-15),
                        UpdatedAt = DateTime.Now.AddDays(-15)
                    },
                    new Expense
                    {
                        ExpenseType = "كهرباء",
                        Description = "فاتورة الكهرباء",
                        Amount = 1500.00m,
                        Category = "تشغيلية",
                        ExpenseDate = DateTime.Now.AddDays(-10),
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now.AddDays(-10)
                    },
                    new Expense
                    {
                        ExpenseType = "راتب",
                        Description = "رواتب الموظفين",
                        Amount = 8000.00m,
                        Category = "رواتب",
                        ExpenseDate = DateTime.Now.AddDays(-5),
                        CreatedAt = DateTime.Now.AddDays(-5),
                        UpdatedAt = DateTime.Now.AddDays(-5)
                    }
                };

                foreach (var expense in expenses)
                {
                    DatabaseHelper_Extensions.InsertExpense(expense);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Expenses");
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        internal static void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
