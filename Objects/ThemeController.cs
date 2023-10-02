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
        public enum ThemeTypes
        {
            Light,
            Dark,
            Navy,
            Old,
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
            string themeName = theme switch
            {
                ThemeTypes.Dark => "Dark",
                ThemeTypes.Navy => "Navy",
                ThemeTypes.Old => "Old",
                // Intentional fall through 
                _ => "Light",
            };

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }

            MainWindow.settings.theme = theme;
            MainWindow.settingsFile.Save();
        }
    }
}
