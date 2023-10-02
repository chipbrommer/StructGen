using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StructGen.Objects
{
    public class ThemeController
    {
        public static ThemeTypes CurrentTheme { get; set; }

        public enum ThemeTypes
        {
            Light,
            Dark,
            Traditional,
            Navy,
        }

        public static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }

        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }

        public static void SetTheme(ThemeTypes theme)
        {
            // Save the theme. 
            //MainWindow.settings.theme = theme;
            MainWindow.Instance.settingsFile.Save();

            string themeName = string.Empty;
            CurrentTheme = theme;

            switch (theme)
            {
                case ThemeTypes.Light: themeName = "Light"; break;
                case ThemeTypes.Navy: themeName = "Navy"; break;
                // Intentional fall through 
                case ThemeTypes.Dark:
                default: themeName = "Dark"; break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }
        }
    }
}
