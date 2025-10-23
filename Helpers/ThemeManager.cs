using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace ETAG_ERP.Helpers
{
    public static class ThemeManager
    {
        public enum Theme
        {
            Light,
            Dark
        }

        private static Theme _currentTheme = Theme.Light;
        private static readonly Dictionary<Theme, ResourceDictionary> _themes = new Dictionary<Theme, ResourceDictionary>();

        static ThemeManager()
        {
            LoadThemes();
        }

        public static Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ApplyTheme(value);
                }
            }
        }

        public static void LoadThemes()
        {
            try
            {
                // Load Light Theme
                var lightTheme = new ResourceDictionary
                {
                    Source = new System.Uri("Themes/LightTheme.xaml", System.UriKind.Relative)
                };
                _themes[Theme.Light] = lightTheme;

                // Load Dark Theme
                var darkTheme = new ResourceDictionary
                {
                    Source = new System.Uri("Themes/DarkTheme.xaml", System.UriKind.Relative)
                };
                _themes[Theme.Dark] = darkTheme;
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"خطأ في تحميل الثيمات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ApplyTheme(Theme theme)
        {
            try
            {
                if (!_themes.ContainsKey(theme))
                {
                    LoadThemes();
                }

                var app = Application.Current;
                if (app == null) return;

                // Remove existing theme dictionaries
                var dictionariesToRemove = app.Resources.MergedDictionaries
                    .Where(d => d.Source?.ToString().Contains("Theme") == true)
                    .ToList();

                foreach (var dict in dictionariesToRemove)
                {
                    app.Resources.MergedDictionaries.Remove(dict);
                }

                // Add new theme
                if (_themes.ContainsKey(theme))
                {
                    app.Resources.MergedDictionaries.Add(_themes[theme]);
                }

                // Apply theme to all windows
                ApplyThemeToWindows(theme);

                // Save theme preference
                DatabaseHelper_Extensions.SetSetting("Theme", theme.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"خطأ في تطبيق الثيم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ApplyThemeToWindows(Theme theme)
        {
            foreach (Window window in Application.Current.Windows)
            {
                ApplyThemeToWindow(window, theme);
            }
        }

        private static void ApplyThemeToWindow(Window window, Theme theme)
        {
            if (window == null) return;

            // Apply theme to window
            if (theme == Theme.Dark)
            {
                window.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                window.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                window.Background = new SolidColorBrush(Colors.White);
                window.Foreground = new SolidColorBrush(Colors.Black);
            }

            // Apply theme to all child elements
            ApplyThemeToElement(window, theme);
        }

        private static void ApplyThemeToElement(DependencyObject element, Theme theme)
        {
            if (element == null) return;

            // Apply theme to specific element types
            if (element is UserControl userControl)
            {
                ApplyThemeToUserControl(userControl, theme);
            }
            else if (element is DataGrid dataGrid)
            {
                ApplyThemeToDataGrid(dataGrid, theme);
            }
            else if (element is Button button)
            {
                ApplyThemeToButton(button, theme);
            }
            else if (element is TextBox textBox)
            {
                ApplyThemeToTextBox(textBox, theme);
            }
            else if (element is ComboBox comboBox)
            {
                ApplyThemeToComboBox(comboBox, theme);
            }

            // Recursively apply to children
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                ApplyThemeToElement(child, theme);
            }
        }

        private static void ApplyThemeToUserControl(UserControl userControl, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                userControl.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                userControl.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                userControl.Background = new SolidColorBrush(Colors.White);
                userControl.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private static void ApplyThemeToDataGrid(DataGrid dataGrid, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                dataGrid.Background = new SolidColorBrush(Color.FromRgb(58, 58, 58));
                dataGrid.Foreground = new SolidColorBrush(Colors.White);
                dataGrid.AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                dataGrid.RowBackground = new SolidColorBrush(Color.FromRgb(58, 58, 58));
                dataGrid.GridLinesBrush = new SolidColorBrush(Color.FromRgb(74, 74, 74));
            }
            else
            {
                dataGrid.Background = new SolidColorBrush(Colors.White);
                dataGrid.Foreground = new SolidColorBrush(Colors.Black);
                dataGrid.AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                dataGrid.RowBackground = new SolidColorBrush(Colors.White);
                dataGrid.GridLinesBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            }
        }

        private static void ApplyThemeToButton(Button button, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(3, 218, 198));
                button.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                button.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private static void ApplyThemeToTextBox(TextBox textBox, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                textBox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                textBox.Foreground = new SolidColorBrush(Colors.White);
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(74, 74, 74));
            }
            else
            {
                textBox.Background = new SolidColorBrush(Colors.White);
                textBox.Foreground = new SolidColorBrush(Colors.Black);
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            }
        }

        private static void ApplyThemeToComboBox(ComboBox comboBox, Theme theme)
        {
            if (theme == Theme.Dark)
            {
                comboBox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                comboBox.Foreground = new SolidColorBrush(Colors.White);
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(74, 74, 74));
            }
            else
            {
                comboBox.Background = new SolidColorBrush(Colors.White);
                comboBox.Foreground = new SolidColorBrush(Colors.Black);
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            }
        }

        public static void InitializeTheme()
        {
            try
            {
                var savedTheme = DatabaseHelper_Extensions.GetSetting("Theme");
                if (!string.IsNullOrEmpty(savedTheme) && Enum.TryParse<Theme>(savedTheme, out var theme))
                {
                    CurrentTheme = theme;
                }
                else
                {
                    CurrentTheme = Theme.Light;
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"خطأ في تهيئة الثيم: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentTheme = Theme.Light;
            }
        }

        public static void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == Theme.Light ? Theme.Dark : Theme.Light;
        }

        public static void SetTheme(Theme theme)
        {
            CurrentTheme = theme;
        }

        public static bool IsDarkTheme => CurrentTheme == Theme.Dark;

        public static Color GetThemeColor(string colorName)
        {
            try
            {
                var app = Application.Current;
                if (app?.Resources[colorName] is Color color)
                {
                    return color;
                }
            }
            catch
            {
                // Return default colors if theme color not found
            }

            // Default colors
            return colorName switch
            {
                "Background" => IsDarkTheme ? Color.FromRgb(30, 30, 30) : Colors.White,
                "Surface" => IsDarkTheme ? Color.FromRgb(45, 45, 45) : Color.FromRgb(248, 249, 250),
                "Text" => IsDarkTheme ? Colors.White : Colors.Black,
                "Primary" => IsDarkTheme ? Color.FromRgb(3, 218, 198) : Color.FromRgb(52, 152, 219),
                "Accent" => IsDarkTheme ? Color.FromRgb(187, 134, 252) : Color.FromRgb(155, 89, 182),
                _ => Colors.Black
            };
        }

        public static Brush GetThemeBrush(string brushName)
        {
            var color = GetThemeColor(brushName);
            return new SolidColorBrush(color);
        }
    }
}
