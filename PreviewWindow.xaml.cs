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
using System.Windows.Shapes;

namespace StructGen
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        /// <summary>Default Constructor</summary>
        public PreviewWindow()
        {
            InitializeComponent();
        }

        /// <summary>Adds a tab to the windows tab control</summary>
        /// <param name="outputType">Content type to be displayed on the tab </param>
        /// <param name="content">Content to be displayed inside the tab.</param>
        public void AddPreviewTab(string outputType, string content)
        {
            TabItem tabItem = new TabItem();
            tabItem.Header = outputType; // Display the output type as the tab header

            TextBox textBox = new TextBox();
            textBox.Text = content;
            textBox.IsReadOnly = true;
            textBox.TextWrapping = TextWrapping.Wrap;

            tabItem.Content = textBox;

            PreviewTabControl.Items.Add(tabItem);
        }
    }
}
