using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using ETAG_ERP.Models;
using ETAG_ERP.ViewModels;

namespace ETAG_ERP.Views
{
    public partial class HRView : UserControl
    {
        public HRView()
        {
            InitializeComponent();
            this.DataContext = new HRViewModel();
        }
    }

    public class HRViewModel : BaseViewModel
    {
        public ObservableCollection<HR_Employee> Employees { get; set; }
        public ObservableCollection<HR_Payroll> Payrolls { get; set; }
        public ObservableCollection<AttendanceRecord> Attendances { get; set; }

        public HRViewModel()
        {
            // Dummy Employee Data
            Employees = new ObservableCollection<HR_Employee>
            {
                new HR_Employee { Id = 1, FullName = "أحمد محمود", JobTitle = "مدير مبيعات", Department = "المبيعات", HireDate = new DateTime(2020, 1, 15), Salary = 12000, Status = "نشط" },
                new HR_Employee { Id = 2, FullName = "فاطمة علي", JobTitle = "محاسب", Department = "المالية", HireDate = new DateTime(2021, 3, 1), Salary = 9000, Status = "نشط" },
                new HR_Employee { Id = 3, FullName = "خالد حسن", JobTitle = "مهندس إنتاج", Department = "الإنتاج", HireDate = new DateTime(2019, 7, 10), Salary = 11000, Status = "نشط" }
            };

            // Dummy Payroll Data
            Payrolls = new ObservableCollection<HR_Payroll>
            {
                new HR_Payroll { Id = 1, Employee = Employees[0], PayPeriodStart = new DateTime(2024, 5, 1), PayPeriodEnd = new DateTime(2024, 5, 31), GrossSalary = 12000, Deductions = 1000, NetSalary = 11000, PayDate = new DateTime(2024, 6, 5) },
                new HR_Payroll { Id = 2, Employee = Employees[1], PayPeriodStart = new DateTime(2024, 5, 1), PayPeriodEnd = new DateTime(2024, 5, 31), GrossSalary = 9000, Deductions = 750, NetSalary = 8250, PayDate = new DateTime(2024, 6, 5) }
            };

            // Dummy Attendance Data
            Attendances = new ObservableCollection<AttendanceRecord>
            {
                new AttendanceRecord { Id = 1, Employee = Employees[0], Date = new DateTime(2024, 6, 10), CheckInTime = new TimeSpan(8, 0, 0), CheckOutTime = new TimeSpan(17, 0, 0), Status = "حاضر" },
                new AttendanceRecord { Id = 2, Employee = Employees[1], Date = new DateTime(2024, 6, 10), CheckInTime = new TimeSpan(8, 30, 0), CheckOutTime = new TimeSpan(17, 30, 0), Status = "حاضر" },
                new AttendanceRecord { Id = 3, Employee = Employees[2], Date = new DateTime(2024, 6, 10), CheckInTime = new TimeSpan(9, 0, 0), CheckOutTime = new TimeSpan(18, 0, 0), Status = "حاضر" }
            };
        }
    }
}