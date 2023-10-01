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
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void CreateHeaderFile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeView(MainWindow.View.Parse);
        }

        private void CreateHeaderDocumentation_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeView(MainWindow.View.Document);
        }
    }
}
