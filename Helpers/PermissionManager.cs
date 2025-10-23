using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    public static class PermissionManager
    {
        // Permission constants
        public const string VIEW_DASHBOARD = "VIEW_DASHBOARD";
        public const string VIEW_ITEMS = "VIEW_ITEMS";
        public const string ADD_ITEMS = "ADD_ITEMS";
        public const string EDIT_ITEMS = "EDIT_ITEMS";
        public const string DELETE_ITEMS = "DELETE_ITEMS";
        public const string VIEW_CLIENTS = "VIEW_CLIENTS";
        public const string ADD_CLIENTS = "ADD_CLIENTS";
        public const string EDIT_CLIENTS = "EDIT_CLIENTS";
        public const string DELETE_CLIENTS = "DELETE_CLIENTS";
        public const string VIEW_INVOICES = "VIEW_INVOICES";
        public const string ADD_INVOICES = "ADD_INVOICES";
        public const string EDIT_INVOICES = "EDIT_INVOICES";
        public const string DELETE_INVOICES = "DELETE_INVOICES";
        public const string VIEW_PURCHASES = "VIEW_PURCHASES";
        public const string ADD_PURCHASES = "ADD_PURCHASES";
        public const string EDIT_PURCHASES = "EDIT_PURCHASES";
        public const string DELETE_PURCHASES = "DELETE_PURCHASES";
        public const string VIEW_EXPENSES = "VIEW_EXPENSES";
        public const string ADD_EXPENSES = "ADD_EXPENSES";
        public const string EDIT_EXPENSES = "EDIT_EXPENSES";
        public const string DELETE_EXPENSES = "DELETE_EXPENSES";
        public const string VIEW_ACCOUNTS = "VIEW_ACCOUNTS";
        public const string ADD_ACCOUNTS = "ADD_ACCOUNTS";
        public const string EDIT_ACCOUNTS = "EDIT_ACCOUNTS";
        public const string DELETE_ACCOUNTS = "DELETE_ACCOUNTS";
        public const string VIEW_REPORTS = "VIEW_REPORTS";
        public const string EXPORT_REPORTS = "EXPORT_REPORTS";
        public const string VIEW_USERS = "VIEW_USERS";
        public const string ADD_USERS = "ADD_USERS";
        public const string EDIT_USERS = "EDIT_USERS";
        public const string DELETE_USERS = "DELETE_USERS";
        public const string VIEW_SETTINGS = "VIEW_SETTINGS";
        public const string EDIT_SETTINGS = "EDIT_SETTINGS";
        public const string VIEW_CATEGORIES = "VIEW_CATEGORIES";
        public const string ADD_CATEGORIES = "ADD_CATEGORIES";
        public const string EDIT_CATEGORIES = "EDIT_CATEGORIES";
        public const string DELETE_CATEGORIES = "DELETE_CATEGORIES";
        public const string VIEW_EMPLOYEES = "VIEW_EMPLOYEES";
        public const string ADD_EMPLOYEES = "ADD_EMPLOYEES";
        public const string EDIT_EMPLOYEES = "EDIT_EMPLOYEES";
        public const string DELETE_EMPLOYEES = "DELETE_EMPLOYEES";
        public const string VIEW_BRANCHES = "VIEW_BRANCHES";
        public const string ADD_BRANCHES = "ADD_BRANCHES";
        public const string EDIT_BRANCHES = "EDIT_BRANCHES";
        public const string DELETE_BRANCHES = "DELETE_BRANCHES";
        public const string BACKUP_DATABASE = "BACKUP_DATABASE";
        public const string RESTORE_DATABASE = "RESTORE_DATABASE";

        private static User? _currentUser;
        private static List<Permission> _allPermissions = new List<Permission>();

        /// <summary>
        /// Initialize permission manager
        /// </summary>
        public static void Initialize()
        {
            try
            {
                SeedPermissions();
                _allPermissions = DatabaseHelper_Extensions.GetAllPermissions()
                    .Select(p => new Permission { Id = p.Id, Name = p.Name })
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initialize Permission Manager");
            }
        }

        /// <summary>
        /// Set current user
        /// </summary>
        public static void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        /// <summary>
        /// Get current user
        /// </summary>
        public static User? GetCurrentUser()
        {
            return _currentUser;
        }

        /// <summary>
        /// Check if current user has permission
        /// </summary>
        public static bool HasPermission(string permissionName)
        {
            if (_currentUser == null)
                return false;

            if (_currentUser.IsAdmin)
                return true;

            try
            {
                var userPermissions = DatabaseHelper_Extensions.GetUserPermissions(_currentUser.Id);
                var permission = _allPermissions.FirstOrDefault(p => p.Name == permissionName);
                
                if (permission == null)
                    return false;

                return userPermissions.Contains(permission.Id);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Check Permission");
                return false;
            }
        }

        /// <summary>
        /// Check if user has permission
        /// </summary>
        public static bool HasPermission(User user, string permissionName)
        {
            if (user == null)
                return false;

            if (user.IsAdmin)
                return true;

            try
            {
                var userPermissions = DatabaseHelper_Extensions.GetUserPermissions(user.Id);
                var permission = _allPermissions.FirstOrDefault(p => p.Name == permissionName);
                
                if (permission == null)
                    return false;

                return userPermissions.Contains(permission.Id);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Check User Permission");
                return false;
            }
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        public static List<Permission> GetUserPermissions(User user)
        {
            if (user == null)
                return new List<Permission>();

            if (user.IsAdmin)
                return _allPermissions;

            try
            {
                var userPermissionIds = DatabaseHelper_Extensions.GetUserPermissions(user.Id);
                return _allPermissions.Where(p => userPermissionIds.Contains(p.Id)).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get User Permissions");
                return new List<Permission>();
            }
        }

        /// <summary>
        /// Assign permission to user
        /// </summary>
        public static bool AssignPermissionToUser(User user, string permissionName)
        {
            if (user == null || string.IsNullOrWhiteSpace(permissionName))
                return false;

            try
            {
                var permission = _allPermissions.FirstOrDefault(p => p.Name == permissionName);
                if (permission == null)
                    return false;

                DatabaseHelper_Extensions.AssignPermissionToUser(user.Id, permission.Id);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Assign Permission to User");
                return false;
            }
        }

        /// <summary>
        /// Remove permission from user
        /// </summary>
        public static bool RemovePermissionFromUser(User user, string permissionName)
        {
            if (user == null || string.IsNullOrWhiteSpace(permissionName))
                return false;

            try
            {
                var permission = _allPermissions.FirstOrDefault(p => p.Name == permissionName);
                if (permission == null)
                    return false;

                DatabaseHelper_Extensions.RemovePermissionFromUser(user.Id, permission.Id);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Remove Permission from User");
                return false;
            }
        }

        /// <summary>
        /// Assign multiple permissions to user
        /// </summary>
        public static bool AssignPermissionsToUser(User user, List<string> permissionNames)
        {
            if (user == null || permissionNames == null)
                return false;

            try
            {
                foreach (var permissionName in permissionNames)
                {
                    AssignPermissionToUser(user, permissionName);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Assign Permissions to User");
                return false;
            }
        }

        /// <summary>
        /// Remove multiple permissions from user
        /// </summary>
        public static bool RemovePermissionsFromUser(User user, List<string> permissionNames)
        {
            if (user == null || permissionNames == null)
                return false;

            try
            {
                foreach (var permissionName in permissionNames)
                {
                    RemovePermissionFromUser(user, permissionName);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Remove Permissions from User");
                return false;
            }
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        public static List<Permission> GetAllPermissions()
        {
            return _allPermissions.ToList();
        }

        /// <summary>
        /// Get permission by name
        /// </summary>
        public static Permission? GetPermissionByName(string permissionName)
        {
            return _allPermissions.FirstOrDefault(p => p.Name == permissionName);
        }

        /// <summary>
        /// Create default roles
        /// </summary>
        public static void CreateDefaultRoles()
        {
            try
            {
                // Admin role - has all permissions
                var adminPermissions = GetAllPermissions().Select(p => p.Name).ToList();
                
                // Manager role - has most permissions except user management
                var managerPermissions = new List<string>
                {
                    VIEW_DASHBOARD, VIEW_ITEMS, ADD_ITEMS, EDIT_ITEMS, DELETE_ITEMS,
                    VIEW_CLIENTS, ADD_CLIENTS, EDIT_CLIENTS, DELETE_CLIENTS,
                    VIEW_INVOICES, ADD_INVOICES, EDIT_INVOICES, DELETE_INVOICES,
                    VIEW_PURCHASES, ADD_PURCHASES, EDIT_PURCHASES, DELETE_PURCHASES,
                    VIEW_EXPENSES, ADD_EXPENSES, EDIT_EXPENSES, DELETE_EXPENSES,
                    VIEW_ACCOUNTS, ADD_ACCOUNTS, EDIT_ACCOUNTS, DELETE_ACCOUNTS,
                    VIEW_REPORTS, EXPORT_REPORTS, VIEW_CATEGORIES, ADD_CATEGORIES,
                    EDIT_CATEGORIES, DELETE_CATEGORIES, VIEW_EMPLOYEES, ADD_EMPLOYEES,
                    EDIT_EMPLOYEES, DELETE_EMPLOYEES, VIEW_BRANCHES, ADD_BRANCHES,
                    EDIT_BRANCHES, DELETE_BRANCHES, VIEW_SETTINGS, EDIT_SETTINGS
                };

                // Cashier role - limited permissions
                var cashierPermissions = new List<string>
                {
                    VIEW_DASHBOARD, VIEW_ITEMS, VIEW_CLIENTS, VIEW_INVOICES,
                    ADD_INVOICES, EDIT_INVOICES, VIEW_CATEGORIES, VIEW_REPORTS
                };

                // Sales role - sales related permissions
                var salesPermissions = new List<string>
                {
                    VIEW_DASHBOARD, VIEW_ITEMS, VIEW_CLIENTS, ADD_CLIENTS, EDIT_CLIENTS,
                    VIEW_INVOICES, ADD_INVOICES, EDIT_INVOICES, VIEW_REPORTS,
                    EXPORT_REPORTS, VIEW_CATEGORIES
                };

                // Store role - inventory related permissions
                var storePermissions = new List<string>
                {
                    VIEW_DASHBOARD, VIEW_ITEMS, ADD_ITEMS, EDIT_ITEMS, DELETE_ITEMS,
                    VIEW_PURCHASES, ADD_PURCHASES, EDIT_PURCHASES, VIEW_CATEGORIES,
                    ADD_CATEGORIES, EDIT_CATEGORIES, DELETE_CATEGORIES, VIEW_REPORTS
                };

                ErrorHandler.LogInfo("تم إنشاء الأدوار الافتراضية", "Permission Manager");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Default Roles");
            }
        }

        /// <summary>
        /// Seed permissions in database
        /// </summary>
        private static void SeedPermissions()
        {
            try
            {
                var existingPermissions = DatabaseHelper_Extensions.GetAllPermissions();
                if (existingPermissions.Count > 0)
                    return; // Permissions already exist

                var permissions = new List<(string Name, string Description)>
                {
                    (VIEW_DASHBOARD, "عرض لوحة التحكم"),
                    (VIEW_ITEMS, "عرض الأصناف"),
                    (ADD_ITEMS, "إضافة أصناف"),
                    (EDIT_ITEMS, "تعديل الأصناف"),
                    (DELETE_ITEMS, "حذف الأصناف"),
                    (VIEW_CLIENTS, "عرض العملاء"),
                    (ADD_CLIENTS, "إضافة عملاء"),
                    (EDIT_CLIENTS, "تعديل العملاء"),
                    (DELETE_CLIENTS, "حذف العملاء"),
                    (VIEW_INVOICES, "عرض الفواتير"),
                    (ADD_INVOICES, "إضافة فواتير"),
                    (EDIT_INVOICES, "تعديل الفواتير"),
                    (DELETE_INVOICES, "حذف الفواتير"),
                    (VIEW_PURCHASES, "عرض المشتريات"),
                    (ADD_PURCHASES, "إضافة مشتريات"),
                    (EDIT_PURCHASES, "تعديل المشتريات"),
                    (DELETE_PURCHASES, "حذف المشتريات"),
                    (VIEW_EXPENSES, "عرض المصروفات"),
                    (ADD_EXPENSES, "إضافة مصروفات"),
                    (EDIT_EXPENSES, "تعديل المصروفات"),
                    (DELETE_EXPENSES, "حذف المصروفات"),
                    (VIEW_ACCOUNTS, "عرض الحسابات"),
                    (ADD_ACCOUNTS, "إضافة حسابات"),
                    (EDIT_ACCOUNTS, "تعديل الحسابات"),
                    (DELETE_ACCOUNTS, "حذف الحسابات"),
                    (VIEW_REPORTS, "عرض التقارير"),
                    (EXPORT_REPORTS, "تصدير التقارير"),
                    (VIEW_USERS, "عرض المستخدمين"),
                    (ADD_USERS, "إضافة مستخدمين"),
                    (EDIT_USERS, "تعديل المستخدمين"),
                    (DELETE_USERS, "حذف المستخدمين"),
                    (VIEW_SETTINGS, "عرض الإعدادات"),
                    (EDIT_SETTINGS, "تعديل الإعدادات"),
                    (VIEW_CATEGORIES, "عرض التصنيفات"),
                    (ADD_CATEGORIES, "إضافة تصنيفات"),
                    (EDIT_CATEGORIES, "تعديل التصنيفات"),
                    (DELETE_CATEGORIES, "حذف التصنيفات"),
                    (VIEW_EMPLOYEES, "عرض الموظفين"),
                    (ADD_EMPLOYEES, "إضافة موظفين"),
                    (EDIT_EMPLOYEES, "تعديل الموظفين"),
                    (DELETE_EMPLOYEES, "حذف الموظفين"),
                    (VIEW_BRANCHES, "عرض الفروع"),
                    (ADD_BRANCHES, "إضافة فروع"),
                    (EDIT_BRANCHES, "تعديل الفروع"),
                    (DELETE_BRANCHES, "حذف الفروع"),
                    (BACKUP_DATABASE, "نسخ احتياطي لقاعدة البيانات"),
                    (RESTORE_DATABASE, "استعادة قاعدة البيانات")
                };

                foreach (var (name, description) in permissions)
                {
                    DatabaseHelper_Extensions.InsertPermission(name, description);
                }

                ErrorHandler.LogInfo("تم إدخال الصلاحيات في قاعدة البيانات", "Permission Manager");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Seed Permissions");
            }
        }

        /// <summary>
        /// Check if user can access module
        /// </summary>
        public static bool CanAccessModule(string moduleName)
        {
            return moduleName switch
            {
                "Dashboard" => HasPermission(VIEW_DASHBOARD),
                "Items" => HasPermission(VIEW_ITEMS),
                "Clients" => HasPermission(VIEW_CLIENTS),
                "Invoices" => HasPermission(VIEW_INVOICES),
                "Purchases" => HasPermission(VIEW_PURCHASES),
                "Expenses" => HasPermission(VIEW_EXPENSES),
                "Accounts" => HasPermission(VIEW_ACCOUNTS),
                "Reports" => HasPermission(VIEW_REPORTS),
                "Users" => HasPermission(VIEW_USERS),
                "Settings" => HasPermission(VIEW_SETTINGS),
                "Categories" => HasPermission(VIEW_CATEGORIES),
                "Employees" => HasPermission(VIEW_EMPLOYEES),
                "Branches" => HasPermission(VIEW_BRANCHES),
                _ => false
            };
        }

        /// <summary>
        /// Check if user can perform action
        /// </summary>
        public static bool CanPerformAction(string action, string module)
        {
            var permissionName = $"{action}_{module}".ToUpper();
            return HasPermission(permissionName);
        }

        /// <summary>
        /// Get accessible modules for current user
        /// </summary>
        public static List<string> GetAccessibleModules()
        {
            var modules = new List<string>();
            
            if (HasPermission(VIEW_DASHBOARD))
                modules.Add("Dashboard");
            
            if (HasPermission(VIEW_ITEMS))
                modules.Add("Items");
            
            if (HasPermission(VIEW_CLIENTS))
                modules.Add("Clients");
            
            if (HasPermission(VIEW_INVOICES))
                modules.Add("Invoices");
            
            if (HasPermission(VIEW_PURCHASES))
                modules.Add("Purchases");
            
            if (HasPermission(VIEW_EXPENSES))
                modules.Add("Expenses");
            
            if (HasPermission(VIEW_ACCOUNTS))
                modules.Add("Accounts");
            
            if (HasPermission(VIEW_REPORTS))
                modules.Add("Reports");
            
            if (HasPermission(VIEW_USERS))
                modules.Add("Users");
            
            if (HasPermission(VIEW_SETTINGS))
                modules.Add("Settings");
            
            if (HasPermission(VIEW_CATEGORIES))
                modules.Add("Categories");
            
            if (HasPermission(VIEW_EMPLOYEES))
                modules.Add("Employees");
            
            if (HasPermission(VIEW_BRANCHES))
                modules.Add("Branches");

            return modules;
        }

        /// <summary>
        /// Log permission check
        /// </summary>
        public static void LogPermissionCheck(string permission, bool granted)
        {
            try
            {
                var message = $"Permission Check: {permission} - {(granted ? "Granted" : "Denied")}";
                ErrorHandler.LogInfo(message, "Permission Manager");
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}