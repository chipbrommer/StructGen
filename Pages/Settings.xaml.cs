using StructGen.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StructGen.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        public void SetThemeSelection(ThemeController.ThemeTypes value)
        {
            switch (value)
            {
                case ThemeController.ThemeTypes.Light: LightTheme.IsChecked = true; break;
                case ThemeController.ThemeTypes.Dark: DarkTheme.IsChecked = true; break;
                case ThemeController.ThemeTypes.Navy: NavyTheme.IsChecked = true; break;
            }
        }

        private void Light_Checked(object sender, RoutedEventArgs e)
        {
            ThemeController.SetTheme(ThemeController.ThemeTypes.Light);
        }

        private void Dark_Checked(object sender, RoutedEventArgs e)
        {
            ThemeController.SetTheme(ThemeController.ThemeTypes.Dark);
        }

        private void Navy_Checked(object sender, RoutedEventArgs e)
        {
            ThemeController.SetTheme(ThemeController.ThemeTypes.Navy);
        }

    }
}
