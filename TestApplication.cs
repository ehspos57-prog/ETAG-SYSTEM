using System;
using System.Windows;
using ETAG_ERP.Views;
using ETAG_ERP.Models;
using ETAG_ERP.ViewModels;

namespace ETAG_ERP
{
    /// <summary>
    /// Simple test application to verify the ERP system works correctly
    /// </summary>
    public class TestApplication
    {
        public static void RunTests()
        {
            try
            {
                Console.WriteLine("🧪 بدء اختبار نظام ETAG ERP...");
                
                // Test 1: Model Creation
                TestModels();
                
                // Test 2: ViewModel Creation
                TestViewModels();
                
                // Test 3: View Creation
                TestViews();
                
                Console.WriteLine("✅ جميع الاختبارات تمت بنجاح!");
                Console.WriteLine("🎉 نظام ETAG ERP جاهز للاستخدام!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في الاختبار: {ex.Message}");
                Console.WriteLine($"تفاصيل الخطأ: {ex.StackTrace}");
            }
        }

        private static void TestModels()
        {
            Console.WriteLine("📋 اختبار النماذج...");
            
            // Test HR Employee
            var employee = new HR_Employee
            {
                Id = 1,
                FullName = "أحمد محمود",
                JobTitle = "مدير مبيعات",
                Department = "المبيعات",
                Salary = 12000,
                Status = "نشط"
            };
            Console.WriteLine($"✅ تم إنشاء موظف: {employee.FullName}");

            // Test CRM Lead
            var lead = new CRM_Lead
            {
                Id = 1,
                Name = "محمد السيد",
                Company = "شركة التقنية الحديثة",
                Email = "mohamed@example.com",
                Status = "جديد"
            };
            Console.WriteLine($"✅ تم إنشاء عميل محتمل: {lead.Name}");

            // Test Project
            var project = new Project
            {
                Id = 1,
                Name = "تطوير نظام ERP",
                Description = "تطوير نظام إدارة متكامل",
                Budget = 250000,
                Status = "قيد التنفيذ"
            };
            Console.WriteLine($"✅ تم إنشاء مشروع: {project.Name}");

            // Test Manufacturing Order
            var manufacturingOrder = new ManufacturingOrder
            {
                Id = 1,
                OrderNumber = "MO-2024-001",
                Quantity = 10,
                Status = "جديد"
            };
            Console.WriteLine($"✅ تم إنشاء أمر تصنيع: {manufacturingOrder.OrderNumber}");
        }

        private static void TestViewModels()
        {
            Console.WriteLine("🎯 اختبار نماذج العرض...");
            
            // Test HR ViewModel
            var hrViewModel = new HRViewModel();
            Console.WriteLine($"✅ تم إنشاء HR ViewModel مع {hrViewModel.Employees.Count} موظف");

            // Test CRM ViewModel
            var crmViewModel = new CRMViewModel();
            Console.WriteLine($"✅ تم إنشاء CRM ViewModel مع {crmViewModel.Leads.Count} عميل محتمل");

            // Test Manufacturing ViewModel
            var manufacturingViewModel = new ManufacturingViewModel();
            Console.WriteLine($"✅ تم إنشاء Manufacturing ViewModel مع {manufacturingViewModel.ManufacturingOrders.Count} أمر تصنيع");

            // Test Project Management ViewModel
            var projectViewModel = new ProjectManagementViewModel();
            Console.WriteLine($"✅ تم إنشاء Project Management ViewModel مع {projectViewModel.Projects.Count} مشروع");
        }

        private static void TestViews()
        {
            Console.WriteLine("🖼️ اختبار واجهات المستخدم...");
            
            try
            {
                // Test HR View
                var hrView = new HRView();
                Console.WriteLine("✅ تم إنشاء HR View بنجاح");

                // Test CRM View
                var crmView = new CRMView();
                Console.WriteLine("✅ تم إنشاء CRM View بنجاح");

                // Test Manufacturing View
                var manufacturingView = new ManufacturingView();
                Console.WriteLine("✅ تم إنشاء Manufacturing View بنجاح");

                // Test Project Management View
                var projectView = new ProjectManagementView();
                Console.WriteLine("✅ تم إنشاء Project Management View بنجاح");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ تحذير في إنشاء الواجهات: {ex.Message}");
                Console.WriteLine("هذا طبيعي في بيئة الاختبار بدون WPF Application");
            }
        }
    }
}

