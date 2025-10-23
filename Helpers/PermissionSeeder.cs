using ETAG_ERP.Models;
using System.Collections.Generic;

namespace ETAG_ERP.Helpers
{
    /// <summary>
    /// Permission seeder for ETAG ERP System
    /// </summary>
    public static class PermissionSeeder
    {
        /// <summary>
        /// Get all permissions for the system
        /// </summary>
        public static List<Permission> GetAllPermissions()
        {
            return new List<Permission>
            {
                // System Administration Permissions
                new Permission { PermissionName = "SystemAdmin", Description = "إدارة النظام الكاملة", Category = "System" },
                new Permission { PermissionName = "UserManagement", Description = "إدارة المستخدمين", Category = "System" },
                new Permission { PermissionName = "RoleManagement", Description = "إدارة الأدوار", Category = "System" },
                new Permission { PermissionName = "PermissionManagement", Description = "إدارة الصلاحيات", Category = "System" },
                new Permission { PermissionName = "SystemSettings", Description = "إعدادات النظام", Category = "System" },
                new Permission { PermissionName = "DatabaseBackup", Description = "نسخ احتياطي لقاعدة البيانات", Category = "System" },
                new Permission { PermissionName = "DatabaseRestore", Description = "استعادة قاعدة البيانات", Category = "System" },
                new Permission { PermissionName = "SystemLogs", Description = "عرض سجلات النظام", Category = "System" },
                new Permission { PermissionName = "SystemMaintenance", Description = "صيانة النظام", Category = "System" },

                // Inventory Management Permissions
                new Permission { PermissionName = "InventoryView", Description = "عرض المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryAdd", Description = "إضافة أصناف للمخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryEdit", Description = "تعديل أصناف المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryDelete", Description = "حذف أصناف من المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryAdjust", Description = "تعديل كميات المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryTransfer", Description = "تحويل المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryReports", Description = "تقارير المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryExport", Description = "تصدير بيانات المخزون", Category = "Inventory" },
                new Permission { PermissionName = "InventoryImport", Description = "استيراد بيانات المخزون", Category = "Inventory" },
                new Permission { PermissionName = "ViewPurchasePrice", Description = "عرض سعر الشراء", Category = "Inventory" },
                new Permission { PermissionName = "EditPurchasePrice", Description = "تعديل سعر الشراء", Category = "Inventory" },
                new Permission { PermissionName = "ViewCostPrice", Description = "عرض سعر التكلفة", Category = "Inventory" },
                new Permission { PermissionName = "EditCostPrice", Description = "تعديل سعر التكلفة", Category = "Inventory" },

                // Category Management Permissions
                new Permission { PermissionName = "CategoryView", Description = "عرض التصنيفات", Category = "Category" },
                new Permission { PermissionName = "CategoryAdd", Description = "إضافة تصنيفات", Category = "Category" },
                new Permission { PermissionName = "CategoryEdit", Description = "تعديل التصنيفات", Category = "Category" },
                new Permission { PermissionName = "CategoryDelete", Description = "حذف التصنيفات", Category = "Category" },
                new Permission { PermissionName = "CategoryReports", Description = "تقارير التصنيفات", Category = "Category" },

                // Sales Management Permissions
                new Permission { PermissionName = "SalesView", Description = "عرض المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesAdd", Description = "إضافة مبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesEdit", Description = "تعديل المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesDelete", Description = "حذف المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesCancel", Description = "إلغاء المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesReturn", Description = "مرتجعات المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesReports", Description = "تقارير المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesExport", Description = "تصدير بيانات المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesDiscount", Description = "تطبيق خصومات على المبيعات", Category = "Sales" },
                new Permission { PermissionName = "SalesCredit", Description = "المبيعات الآجلة", Category = "Sales" },
                new Permission { PermissionName = "SalesCash", Description = "المبيعات النقدية", Category = "Sales" },
                new Permission { PermissionName = "SalesPOS", Description = "نقاط البيع", Category = "Sales" },

                // Customer Management Permissions
                new Permission { PermissionName = "CustomerView", Description = "عرض العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerAdd", Description = "إضافة عملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerEdit", Description = "تعديل بيانات العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerDelete", Description = "حذف العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerCredit", Description = "إدارة ائتمان العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerReports", Description = "تقارير العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerExport", Description = "تصدير بيانات العملاء", Category = "Customer" },
                new Permission { PermissionName = "CustomerImport", Description = "استيراد بيانات العملاء", Category = "Customer" },

                // Purchase Management Permissions
                new Permission { PermissionName = "PurchaseView", Description = "عرض المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseAdd", Description = "إضافة مشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseEdit", Description = "تعديل المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseDelete", Description = "حذف المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseCancel", Description = "إلغاء المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseReturn", Description = "مرتجعات المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseReports", Description = "تقارير المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseExport", Description = "تصدير بيانات المشتريات", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseOrder", Description = "أوامر الشراء", Category = "Purchase" },
                new Permission { PermissionName = "PurchaseReceive", Description = "استلام المشتريات", Category = "Purchase" },

                // Supplier Management Permissions
                new Permission { PermissionName = "SupplierView", Description = "عرض الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierAdd", Description = "إضافة موردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierEdit", Description = "تعديل بيانات الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierDelete", Description = "حذف الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierCredit", Description = "إدارة ائتمان الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierReports", Description = "تقارير الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierExport", Description = "تصدير بيانات الموردين", Category = "Supplier" },
                new Permission { PermissionName = "SupplierImport", Description = "استيراد بيانات الموردين", Category = "Supplier" },

                // Financial Management Permissions
                new Permission { PermissionName = "FinancialView", Description = "عرض البيانات المالية", Category = "Financial" },
                new Permission { PermissionName = "FinancialAdd", Description = "إضافة معاملات مالية", Category = "Financial" },
                new Permission { PermissionName = "FinancialEdit", Description = "تعديل المعاملات المالية", Category = "Financial" },
                new Permission { PermissionName = "FinancialDelete", Description = "حذف المعاملات المالية", Category = "Financial" },
                new Permission { PermissionName = "FinancialReports", Description = "التقارير المالية", Category = "Financial" },
                new Permission { PermissionName = "FinancialExport", Description = "تصدير البيانات المالية", Category = "Financial" },
                new Permission { PermissionName = "ExpenseManagement", Description = "إدارة المصروفات", Category = "Financial" },
                new Permission { PermissionName = "RevenueManagement", Description = "إدارة الإيرادات", Category = "Financial" },
                new Permission { PermissionName = "PaymentManagement", Description = "إدارة المدفوعات", Category = "Financial" },
                new Permission { PermissionName = "TaxManagement", Description = "إدارة الضرائب", Category = "Financial" },

                // Account Management Permissions
                new Permission { PermissionName = "AccountView", Description = "عرض الحسابات", Category = "Account" },
                new Permission { PermissionName = "AccountAdd", Description = "إضافة حسابات", Category = "Account" },
                new Permission { PermissionName = "AccountEdit", Description = "تعديل الحسابات", Category = "Account" },
                new Permission { PermissionName = "AccountDelete", Description = "حذف الحسابات", Category = "Account" },
                new Permission { PermissionName = "AccountReports", Description = "تقارير الحسابات", Category = "Account" },
                new Permission { PermissionName = "AccountExport", Description = "تصدير بيانات الحسابات", Category = "Account" },

                // Employee Management Permissions
                new Permission { PermissionName = "EmployeeView", Description = "عرض الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeAdd", Description = "إضافة موظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeEdit", Description = "تعديل بيانات الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeDelete", Description = "حذف الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeReports", Description = "تقارير الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeExport", Description = "تصدير بيانات الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeImport", Description = "استيراد بيانات الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeSalary", Description = "إدارة رواتب الموظفين", Category = "Employee" },
                new Permission { PermissionName = "EmployeeAttendance", Description = "إدارة حضور الموظفين", Category = "Employee" },

                // Branch Management Permissions
                new Permission { PermissionName = "BranchView", Description = "عرض الفروع", Category = "Branch" },
                new Permission { PermissionName = "BranchAdd", Description = "إضافة فروع", Category = "Branch" },
                new Permission { PermissionName = "BranchEdit", Description = "تعديل بيانات الفروع", Category = "Branch" },
                new Permission { PermissionName = "BranchDelete", Description = "حذف الفروع", Category = "Branch" },
                new Permission { PermissionName = "BranchReports", Description = "تقارير الفروع", Category = "Branch" },
                new Permission { PermissionName = "BranchTransfer", Description = "تحويل بين الفروع", Category = "Branch" },

                // Reporting Permissions
                new Permission { PermissionName = "ReportView", Description = "عرض التقارير", Category = "Report" },
                new Permission { PermissionName = "ReportCreate", Description = "إنشاء تقارير", Category = "Report" },
                new Permission { PermissionName = "ReportEdit", Description = "تعديل التقارير", Category = "Report" },
                new Permission { PermissionName = "ReportDelete", Description = "حذف التقارير", Category = "Report" },
                new Permission { PermissionName = "ReportExport", Description = "تصدير التقارير", Category = "Report" },
                new Permission { PermissionName = "ReportPrint", Description = "طباعة التقارير", Category = "Report" },
                new Permission { PermissionName = "ReportSchedule", Description = "جدولة التقارير", Category = "Report" },

                // Dashboard Permissions
                new Permission { PermissionName = "DashboardView", Description = "عرض لوحة التحكم", Category = "Dashboard" },
                new Permission { PermissionName = "DashboardCustomize", Description = "تخصيص لوحة التحكم", Category = "Dashboard" },
                new Permission { PermissionName = "DashboardExport", Description = "تصدير بيانات لوحة التحكم", Category = "Dashboard" },

                // Backup and Maintenance Permissions
                new Permission { PermissionName = "BackupCreate", Description = "إنشاء نسخ احتياطية", Category = "Backup" },
                new Permission { PermissionName = "BackupRestore", Description = "استعادة النسخ الاحتياطية", Category = "Backup" },
                new Permission { PermissionName = "BackupSchedule", Description = "جدولة النسخ الاحتياطية", Category = "Backup" },
                new Permission { PermissionName = "BackupDelete", Description = "حذف النسخ الاحتياطية", Category = "Backup" },
                new Permission { PermissionName = "MaintenanceRun", Description = "تشغيل الصيانة", Category = "Maintenance" },
                new Permission { PermissionName = "MaintenanceSchedule", Description = "جدولة الصيانة", Category = "Maintenance" },

                // Security Permissions
                new Permission { PermissionName = "SecurityView", Description = "عرض إعدادات الأمان", Category = "Security" },
                new Permission { PermissionName = "SecurityEdit", Description = "تعديل إعدادات الأمان", Category = "Security" },
                new Permission { PermissionName = "SecurityAudit", Description = "مراجعة الأمان", Category = "Security" },
                new Permission { PermissionName = "SecurityLogs", Description = "عرض سجلات الأمان", Category = "Security" },

                // Network and Integration Permissions
                new Permission { PermissionName = "NetworkView", Description = "عرض إعدادات الشبكة", Category = "Network" },
                new Permission { PermissionName = "NetworkEdit", Description = "تعديل إعدادات الشبكة", Category = "Network" },
                new Permission { PermissionName = "IntegrationView", Description = "عرض التكاملات", Category = "Integration" },
                new Permission { PermissionName = "IntegrationEdit", Description = "تعديل التكاملات", Category = "Integration" },

                // Advanced Features Permissions
                new Permission { PermissionName = "AdvancedFeatures", Description = "الميزات المتقدمة", Category = "Advanced" },
                new Permission { PermissionName = "API Access", Description = "الوصول للواجهة البرمجية", Category = "Advanced" },
                new Permission { PermissionName = "BulkOperations", Description = "العمليات المجمعة", Category = "Advanced" },
                new Permission { PermissionName = "DataMigration", Description = "نقل البيانات", Category = "Advanced" },
                new Permission { PermissionName = "SystemIntegration", Description = "تكامل النظام", Category = "Advanced" }
            };
        }

        /// <summary>
        /// Get default role permissions
        /// </summary>
        public static Dictionary<string, List<string>> GetDefaultRolePermissions()
        {
            return new Dictionary<string, List<string>>
            {
                ["Administrator"] = new List<string>
                {
                    // All permissions for administrator
                    "SystemAdmin", "UserManagement", "RoleManagement", "PermissionManagement", "SystemSettings",
                    "DatabaseBackup", "DatabaseRestore", "SystemLogs", "SystemMaintenance",
                    "InventoryView", "InventoryAdd", "InventoryEdit", "InventoryDelete", "InventoryAdjust", "InventoryTransfer",
                    "InventoryReports", "InventoryExport", "InventoryImport", "ViewPurchasePrice", "EditPurchasePrice",
                    "ViewCostPrice", "EditCostPrice",
                    "CategoryView", "CategoryAdd", "CategoryEdit", "CategoryDelete", "CategoryReports",
                    "SalesView", "SalesAdd", "SalesEdit", "SalesDelete", "SalesCancel", "SalesReturn",
                    "SalesReports", "SalesExport", "SalesDiscount", "SalesCredit", "SalesCash", "SalesPOS",
                    "CustomerView", "CustomerAdd", "CustomerEdit", "CustomerDelete", "CustomerCredit",
                    "CustomerReports", "CustomerExport", "CustomerImport",
                    "PurchaseView", "PurchaseAdd", "PurchaseEdit", "PurchaseDelete", "PurchaseCancel",
                    "PurchaseReturn", "PurchaseReports", "PurchaseExport", "PurchaseOrder", "PurchaseReceive",
                    "SupplierView", "SupplierAdd", "SupplierEdit", "SupplierDelete", "SupplierCredit",
                    "SupplierReports", "SupplierExport", "SupplierImport",
                    "FinancialView", "FinancialAdd", "FinancialEdit", "FinancialDelete", "FinancialReports",
                    "FinancialExport", "ExpenseManagement", "RevenueManagement", "PaymentManagement", "TaxManagement",
                    "AccountView", "AccountAdd", "AccountEdit", "AccountDelete", "AccountReports", "AccountExport",
                    "EmployeeView", "EmployeeAdd", "EmployeeEdit", "EmployeeDelete", "EmployeeReports",
                    "EmployeeExport", "EmployeeImport", "EmployeeSalary", "EmployeeAttendance",
                    "BranchView", "BranchAdd", "BranchEdit", "BranchDelete", "BranchReports", "BranchTransfer",
                    "ReportView", "ReportCreate", "ReportEdit", "ReportDelete", "ReportExport", "ReportPrint", "ReportSchedule",
                    "DashboardView", "DashboardCustomize", "DashboardExport",
                    "BackupCreate", "BackupRestore", "BackupSchedule", "BackupDelete", "MaintenanceRun", "MaintenanceSchedule",
                    "SecurityView", "SecurityEdit", "SecurityAudit", "SecurityLogs",
                    "NetworkView", "NetworkEdit", "IntegrationView", "IntegrationEdit",
                    "AdvancedFeatures", "API Access", "BulkOperations", "DataMigration", "SystemIntegration"
                },

                ["Manager"] = new List<string>
                {
                    "InventoryView", "InventoryAdd", "InventoryEdit", "InventoryAdjust", "InventoryTransfer",
                    "InventoryReports", "InventoryExport", "ViewPurchasePrice", "EditPurchasePrice",
                    "CategoryView", "CategoryAdd", "CategoryEdit", "CategoryReports",
                    "SalesView", "SalesAdd", "SalesEdit", "SalesCancel", "SalesReturn",
                    "SalesReports", "SalesExport", "SalesDiscount", "SalesCredit", "SalesCash", "SalesPOS",
                    "CustomerView", "CustomerAdd", "CustomerEdit", "CustomerCredit",
                    "CustomerReports", "CustomerExport",
                    "PurchaseView", "PurchaseAdd", "PurchaseEdit", "PurchaseCancel",
                    "PurchaseReturn", "PurchaseReports", "PurchaseExport", "PurchaseOrder", "PurchaseReceive",
                    "SupplierView", "SupplierAdd", "SupplierEdit", "SupplierCredit",
                    "SupplierReports", "SupplierExport",
                    "FinancialView", "FinancialAdd", "FinancialEdit", "FinancialReports",
                    "FinancialExport", "ExpenseManagement", "RevenueManagement", "PaymentManagement",
                    "AccountView", "AccountAdd", "AccountEdit", "AccountReports", "AccountExport",
                    "EmployeeView", "EmployeeAdd", "EmployeeEdit", "EmployeeReports",
                    "EmployeeExport", "EmployeeSalary", "EmployeeAttendance",
                    "BranchView", "BranchAdd", "BranchEdit", "BranchReports", "BranchTransfer",
                    "ReportView", "ReportCreate", "ReportEdit", "ReportExport", "ReportPrint",
                    "DashboardView", "DashboardCustomize", "DashboardExport",
                    "BackupCreate", "BackupSchedule"
                },

                ["Sales"] = new List<string>
                {
                    "InventoryView", "InventoryReports",
                    "CategoryView", "CategoryReports",
                    "SalesView", "SalesAdd", "SalesEdit", "SalesCancel", "SalesReturn",
                    "SalesReports", "SalesExport", "SalesDiscount", "SalesCredit", "SalesCash", "SalesPOS",
                    "CustomerView", "CustomerAdd", "CustomerEdit", "CustomerCredit",
                    "CustomerReports", "CustomerExport",
                    "ReportView", "ReportCreate", "ReportExport", "ReportPrint",
                    "DashboardView", "DashboardExport"
                },

                ["Purchase"] = new List<string>
                {
                    "InventoryView", "InventoryAdd", "InventoryEdit", "InventoryAdjust",
                    "InventoryReports", "InventoryExport", "ViewPurchasePrice", "EditPurchasePrice",
                    "CategoryView", "CategoryAdd", "CategoryEdit", "CategoryReports",
                    "PurchaseView", "PurchaseAdd", "PurchaseEdit", "PurchaseCancel",
                    "PurchaseReturn", "PurchaseReports", "PurchaseExport", "PurchaseOrder", "PurchaseReceive",
                    "SupplierView", "SupplierAdd", "SupplierEdit", "SupplierCredit",
                    "SupplierReports", "SupplierExport",
                    "ReportView", "ReportCreate", "ReportExport", "ReportPrint",
                    "DashboardView", "DashboardExport"
                },

                ["Inventory"] = new List<string>
                {
                    "InventoryView", "InventoryAdd", "InventoryEdit", "InventoryAdjust", "InventoryTransfer",
                    "InventoryReports", "InventoryExport", "InventoryImport", "ViewPurchasePrice", "EditPurchasePrice",
                    "CategoryView", "CategoryAdd", "CategoryEdit", "CategoryReports",
                    "ReportView", "ReportCreate", "ReportExport", "ReportPrint",
                    "DashboardView", "DashboardExport"
                },

                ["Cashier"] = new List<string>
                {
                    "InventoryView", "InventoryReports",
                    "CategoryView", "CategoryReports",
                    "SalesView", "SalesAdd", "SalesEdit", "SalesCancel", "SalesReturn",
                    "SalesReports", "SalesDiscount", "SalesCash", "SalesPOS",
                    "CustomerView", "CustomerAdd", "CustomerEdit",
                    "ReportView", "ReportCreate", "ReportExport", "ReportPrint",
                    "DashboardView"
                },

                ["Viewer"] = new List<string>
                {
                    "InventoryView", "InventoryReports",
                    "CategoryView", "CategoryReports",
                    "SalesView", "SalesReports",
                    "CustomerView", "CustomerReports",
                    "PurchaseView", "PurchaseReports",
                    "SupplierView", "SupplierReports",
                    "FinancialView", "FinancialReports",
                    "AccountView", "AccountReports",
                    "EmployeeView", "EmployeeReports",
                    "BranchView", "BranchReports",
                    "ReportView", "ReportExport", "ReportPrint",
                    "DashboardView"
                }
            };
        }
    }
}
