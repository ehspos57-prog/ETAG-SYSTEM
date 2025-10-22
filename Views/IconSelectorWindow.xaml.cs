using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ETAG_ERP.Views
{
    public partial class IconSelectorWindow : Window
    {
        public string SelectedIcon { get; private set; } = "📁";

        public IconSelectorWindow()
        {
            InitializeComponent();
            LoadIcons();
        }

        private void LoadIcons()
        {
            var icons = new ObservableCollection<string>
            {
                "📁", "📂", "📄", "📋", "📊", "📈", "📉", "📌", "📍", "🔖",
                "🏷️", "📦", "📫", "📪", "📬", "📭", "📮", "🗳️", "✏️", "✒️",
                "🖋️", "🖊️", "🖌️", "🖍️", "📝", "💼", "📁", "🗂️", "🗃️", "🗄️",
                "🏢", "🏬", "🏪", "🏫", "🏩", "🏨", "🏦", "🏥", "🏤", "🏣",
                "🍎", "🍊", "🍋", "🍌", "🍉", "🍇", "🍓", "🍈", "🍒", "🍑",
                "🥭", "🍍", "🥥", "🥝", "🍅", "🍆", "🥑", "🥦", "🥬", "🥒",
                "🌶️", "🌽", "🥕", "🥔", "🍠", "🥐", "🥖", "🍞", "🥨", "🥯",
                "🧀", "🥚", "🍳", "🧈", "🥞", "🧇", "🥓", "🥩", "🍗", "🍖",
                "🦴", "🌭", "🍔", "🍟", "🍕", "🌮", "🌯", "🥙", "🧆", "🥚",
                "🍱", "🍘", "🍙", "🍚", "🍛", "🍜", "🍝", "🍠", "🍢", "🍣",
                "🍤", "🍥", "🥮", "🍡", "🥟", "🥠", "🥡", "🦀", "🦞", "🦐",
                "🦑", "🦪", "🍦", "🍧", "🍨", "🍩", "🍪", "🎂", "🍰", "🧁",
                "🥧", "🍫", "🍬", "🍭", "🍮", "🍯", "🍼", "🥛", "☕", "🍵",
                "🧃", "🥤", "🍶", "🍺", "🍻", "🥂", "🍷", "🥃", "🍸", "🍹",
                "🧉", "🍾", "🧊", "🥄", "🍴", "🍽️", "🥣", "🥡", "🥢", "🧂"
            };

            IconsContainer.ItemsSource = icons;
        }

        private void Icon_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string icon)
            {
                SelectedIcon = icon;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
