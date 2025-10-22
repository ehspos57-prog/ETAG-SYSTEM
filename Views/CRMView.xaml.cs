using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ETAG_ERP.Models;
using ETAG_ERP.ViewModels;

namespace ETAG_ERP.Views
{
    public partial class CRMView : UserControl
    {
        public CRMView()
        {
            InitializeComponent();
            this.DataContext = new CRMViewModel();
        }
    }

    public class CRMViewModel : BaseViewModel
    {
        public ObservableCollection<CRM_Lead> Leads { get; set; }
        public ObservableCollection<CRM_Opportunity> Opportunities { get; set; }

        public CRMViewModel()
        {
            // Dummy Leads Data
            Leads = new ObservableCollection<CRM_Lead>
            {
                new CRM_Lead { Id = 1, Name = "محمد السيد", Company = "شركة التقنية الحديثة", Email = "mohamed.elsayed@example.com", Phone = "01012345678", Source = "موقع إلكتروني", Status = "جديد" },
                new CRM_Lead { Id = 2, Name = "سارة أحمد", Company = "مؤسسة البناء السريع", Email = "sara.ahmed@example.com", Phone = "01123456789", Source = "إحالة", Status = "تم التواصل" }
            };

            // Dummy Opportunities Data
            Opportunities = new ObservableCollection<CRM_Opportunity>
            {
                new CRM_Opportunity { Id = 1, Name = "مشروع توريد مضخات", Client = new Client { Name = "شركة الأمل للمقاولات" }, ExpectedRevenue = 150000, ExpectedCloseDate = new DateTime(2024, 9, 30), Stage = "تقديم عرض" },
                new CRM_Opportunity { Id = 2, Name = "عقد صيانة سنوي", Client = new Client { Name = "شركة الصناعات المتطورة" }, ExpectedRevenue = 50000, ExpectedCloseDate = new DateTime(2024, 7, 15), Stage = "تفاوض" }
            };
        }
    }
}