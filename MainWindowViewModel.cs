using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ETAG_ERP.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _currentViewTitle = "الرئيسية";

        /// <summary>
        /// اسم الشاشة المعروضة حاليًا (يظهر في الشريط العلوي).
        /// </summary>
        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set
            {
                if (_currentViewTitle != value)
                {
                    _currentViewTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        // 📌 هنا ممكن نضيف خصائص أخرى لاحقًا مثل:
        // - اسم المستخدم الحالي
        // - حالة الاتصال بالداتابيس
        // - إشعارات أو رسائل حالة

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
