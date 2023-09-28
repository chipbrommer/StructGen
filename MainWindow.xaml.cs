using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;

namespace StructGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Displays a file selector box for the user to browse a file to read in</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml*";
            openFileDialog.Title = "Select an input file";

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = openFileDialog.FileName;

                // Display the file path in the TextBox
                InputFilePathTextBox.Text = filePath;
            }
        }

        /// <summary>Displays a file selector box for the user to browse an output location</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the selected folder path
                    string selectedFolderPath = folderDialog.SelectedPath;

                    // Display the selected folder path in the TextBox
                    OutputFilePathTextBox.Text = selectedFolderPath;
                }
            }
        }
    }
}
