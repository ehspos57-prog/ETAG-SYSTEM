using ETAG_ERP.Models;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    public static class PermissionManager
    {
        public enum Permission
        {
            // Sales Permissions
            ViewSales,
            CreateSales,
            EditSales,
            DeleteSales,
            PrintSales,
            ExportSales,

            // Purchase Permissions
            ViewPurchases,
            CreatePurchases,
            EditPurchases,
            DeletePurchases,
            PrintPurchases,
            ExportPurchases,

            // Customer Permissions
            ViewCustomers,
            CreateCustomers,
            EditCustomers,
            DeleteCustomers,
            PrintCustomers,
            ExportCustomers,

            // Items Permissions
            ViewItems,
            CreateItems,
            EditItems,
            DeleteItems,
            PrintItems,
            ExportItems,

            // Accounts Permissions
            ViewAccounts,
            CreateAccounts,
            EditAccounts,
            DeleteAccounts,
            PrintAccounts,
            ExportAccounts,

            // Reports Permissions
            ViewReports,
            CreateReports,
            PrintReports,
            ExportReports,

            // Settings Permissions
            ViewSettings,
            EditSettings,
            ManageUsers,
            ManagePermissions,

            // Ledger Permissions
            ViewLedger,
            CreateLedger,
            EditLedger,
            DeleteLedger,
            PrintLedger,
            ExportLedger
        }

        public enum Role
        {
            Admin,
            Manager,
            Cashier,
            Viewer
        }

        private static readonly Dictionary<Role, List<Permission>> RolePermissions = new Dictionary<Role, List<Permission>>
        {
            [Role.Admin] = new List<Permission>
            {
                // Admin has all permissions
                Permission.ViewSales, Permission.CreateSales, Permission.EditSales, Permission.DeleteSales, Permission.PrintSales, Permission.ExportSales,
                Permission.ViewPurchases, Permission.CreatePurchases, Permission.EditPurchases, Permission.DeletePurchases, Permission.PrintPurchases, Permission.ExportPurchases,
                Permission.ViewCustomers, Permission.CreateCustomers, Permission.EditCustomers, Permission.DeleteCustomers, Permission.PrintCustomers, Permission.ExportCustomers,
                Permission.ViewItems, Permission.CreateItems, Permission.EditItems, Permission.DeleteItems, Permission.PrintItems, Permission.ExportItems,
                Permission.ViewAccounts, Permission.CreateAccounts, Permission.EditAccounts, Permission.DeleteAccounts, Permission.PrintAccounts, Permission.ExportAccounts,
                Permission.ViewReports, Permission.CreateReports, Permission.PrintReports, Permission.ExportReports,
                Permission.ViewSettings, Permission.EditSettings, Permission.ManageUsers, Permission.ManagePermissions,
                Permission.ViewLedger, Permission.CreateLedger, Permission.EditLedger, Permission.DeleteLedger, Permission.PrintLedger, Permission.ExportLedger
            },
            [Role.Manager] = new List<Permission>
            {
                Permission.ViewSales, Permission.CreateSales, Permission.EditSales, Permission.PrintSales, Permission.ExportSales,
                Permission.ViewPurchases, Permission.CreatePurchases, Permission.EditPurchases, Permission.PrintPurchases, Permission.ExportPurchases,
                Permission.ViewCustomers, Permission.CreateCustomers, Permission.EditCustomers, Permission.PrintCustomers, Permission.ExportCustomers,
                Permission.ViewItems, Permission.CreateItems, Permission.EditItems, Permission.PrintItems, Permission.ExportItems,
                Permission.ViewAccounts, Permission.CreateAccounts, Permission.EditAccounts, Permission.PrintAccounts, Permission.ExportAccounts,
                Permission.ViewReports, Permission.CreateReports, Permission.PrintReports, Permission.ExportReports,
                Permission.ViewSettings, Permission.EditSettings,
                Permission.ViewLedger, Permission.CreateLedger, Permission.EditLedger, Permission.PrintLedger, Permission.ExportLedger
            },
            [Role.Cashier] = new List<Permission>
            {
                Permission.ViewSales, Permission.CreateSales, Permission.EditSales, Permission.PrintSales, Permission.ExportSales,
                Permission.ViewCustomers, Permission.CreateCustomers, Permission.EditCustomers, Permission.PrintCustomers, Permission.ExportCustomers,
                Permission.ViewItems, Permission.PrintItems, Permission.ExportItems,
                Permission.ViewAccounts, Permission.PrintAccounts, Permission.ExportAccounts,
                Permission.ViewReports, Permission.PrintReports, Permission.ExportReports,
                Permission.ViewLedger, Permission.PrintLedger, Permission.ExportLedger
            },
            [Role.Viewer] = new List<Permission>
            {
                Permission.ViewSales, Permission.PrintSales, Permission.ExportSales,
                Permission.ViewPurchases, Permission.PrintPurchases, Permission.ExportPurchases,
                Permission.ViewCustomers, Permission.PrintCustomers, Permission.ExportCustomers,
                Permission.ViewItems, Permission.PrintItems, Permission.ExportItems,
                Permission.ViewAccounts, Permission.PrintAccounts, Permission.ExportAccounts,
                Permission.ViewReports, Permission.PrintReports, Permission.ExportReports,
                Permission.ViewLedger, Permission.PrintLedger, Permission.ExportLedger
            }
        };

        public static bool HasPermission(User user, Permission permission)
        {
            if (user == null) return false;

            // Get user role
            var role = GetUserRole(user);
            if (role == null) return false;

            // Check if role has permission
            return RolePermissions.ContainsKey(role.Value) && 
                   RolePermissions[role.Value].Contains(permission);
        }

        public static bool HasPermission(Permission permission)
        {
            return HasPermission(SessionManager.CurrentUser, permission);
        }

        public static Role? GetUserRole(User user)
        {
            if (user == null) return null;

            if (user.IsAdmin) return Role.Admin;

            // Parse role from user role string
            if (Enum.TryParse<Role>(user.Role, true, out var role))
            {
                return role;
            }

            // Default to Viewer if role is not recognized
            return Role.Viewer;
        }

        public static List<Permission> GetUserPermissions(User user)
        {
            var role = GetUserRole(user);
            if (role == null) return new List<Permission>();

            return RolePermissions.ContainsKey(role.Value) ? 
                   RolePermissions[role.Value] : 
                   new List<Permission>();
        }

        public static List<Permission> GetCurrentUserPermissions()
        {
            return GetUserPermissions(SessionManager.CurrentUser);
        }

        public static bool CanAccessModule(string moduleName)
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return false;

            switch (moduleName.ToLower())
            {
                case "sales":
                case "فاتورة":
                    return HasPermission(user, Permission.ViewSales);
                case "purchases":
                case "مشتريات":
                    return HasPermission(user, Permission.ViewPurchases);
                case "customers":
                case "عملاء":
                    return HasPermission(user, Permission.ViewCustomers);
                case "items":
                case "أصناف":
                    return HasPermission(user, Permission.ViewItems);
                case "accounts":
                case "حسابات":
                    return HasPermission(user, Permission.ViewAccounts);
                case "reports":
                case "تقارير":
                    return HasPermission(user, Permission.ViewReports);
                case "settings":
                case "إعدادات":
                    return HasPermission(user, Permission.ViewSettings);
                case "ledger":
                case "دفتر الأستاذ":
                    return HasPermission(user, Permission.ViewLedger);
                default:
                    return false;
            }
        }

        public static void CheckPermission(Permission permission)
        {
            if (!HasPermission(permission))
            {
                throw new UnauthorizedAccessException("ليس لديك صلاحية للوصول إلى هذه الوظيفة");
            }
        }

        public static void CheckModuleAccess(string moduleName)
        {
            if (!CanAccessModule(moduleName))
            {
                throw new UnauthorizedAccessException("ليس لديك صلاحية للوصول إلى هذه الوحدة");
            }
        }

        public static string GetPermissionDisplayName(Permission permission)
        {
            return permission switch
            {
                Permission.ViewSales => "عرض المبيعات",
                Permission.CreateSales => "إضافة مبيعات",
                Permission.EditSales => "تعديل المبيعات",
                Permission.DeleteSales => "حذف المبيعات",
                Permission.PrintSales => "طباعة المبيعات",
                Permission.ExportSales => "تصدير المبيعات",
                Permission.ViewPurchases => "عرض المشتريات",
                Permission.CreatePurchases => "إضافة مشتريات",
                Permission.EditPurchases => "تعديل المشتريات",
                Permission.DeletePurchases => "حذف المشتريات",
                Permission.PrintPurchases => "طباعة المشتريات",
                Permission.ExportPurchases => "تصدير المشتريات",
                Permission.ViewCustomers => "عرض العملاء",
                Permission.CreateCustomers => "إضافة عملاء",
                Permission.EditCustomers => "تعديل العملاء",
                Permission.DeleteCustomers => "حذف العملاء",
                Permission.PrintCustomers => "طباعة العملاء",
                Permission.ExportCustomers => "تصدير العملاء",
                Permission.ViewItems => "عرض الأصناف",
                Permission.CreateItems => "إضافة أصناف",
                Permission.EditItems => "تعديل الأصناف",
                Permission.DeleteItems => "حذف الأصناف",
                Permission.PrintItems => "طباعة الأصناف",
                Permission.ExportItems => "تصدير الأصناف",
                Permission.ViewAccounts => "عرض الحسابات",
                Permission.CreateAccounts => "إضافة حسابات",
                Permission.EditAccounts => "تعديل الحسابات",
                Permission.DeleteAccounts => "حذف الحسابات",
                Permission.PrintAccounts => "طباعة الحسابات",
                Permission.ExportAccounts => "تصدير الحسابات",
                Permission.ViewReports => "عرض التقارير",
                Permission.CreateReports => "إنشاء تقارير",
                Permission.PrintReports => "طباعة التقارير",
                Permission.ExportReports => "تصدير التقارير",
                Permission.ViewSettings => "عرض الإعدادات",
                Permission.EditSettings => "تعديل الإعدادات",
                Permission.ManageUsers => "إدارة المستخدمين",
                Permission.ManagePermissions => "إدارة الصلاحيات",
                Permission.ViewLedger => "عرض دفتر الأستاذ",
                Permission.CreateLedger => "إضافة قيود محاسبية",
                Permission.EditLedger => "تعديل القيود المحاسبية",
                Permission.DeleteLedger => "حذف القيود المحاسبية",
                Permission.PrintLedger => "طباعة دفتر الأستاذ",
                Permission.ExportLedger => "تصدير دفتر الأستاذ",
                _ => permission.ToString()
            };
        }

        public static string GetRoleDisplayName(Role role)
        {
            return role switch
            {
                Role.Admin => "مدير النظام",
                Role.Manager => "مدير",
                Role.Cashier => "أمين صندوق",
                Role.Viewer => "مشاهد",
                _ => role.ToString()
            };
        }
    }
}
