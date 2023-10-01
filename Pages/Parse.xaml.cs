using StructGen.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using static StructGen.MainWindow;

namespace StructGen.Pages
{
    /// <summary>
    /// Interaction logic for Parse.xaml
    /// </summary>
    public partial class Parse : Page
    {
        public Parse()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            InputFilePathTextBox.Text = string.Empty;
            OutputFilePathTextBox.Text = string.Empty;
            CButton.IsChecked = false;
            CppButton.IsChecked = false;
            CSharpButton.IsChecked = false;
            FddButton.IsChecked = false;
        }

        public void SetNotification()
        {

        }

        /// <summary>Displays a file selector box for the user to browse a file to read in</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // @todo - reallow csv files when a template gets created and implemented - CSV Files (*.csv)|*.csv|
            openFileDialog.Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml*|All Files|*.*";
            openFileDialog.Title = "Select an input file";

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = openFileDialog.FileName;

                // Display the file path in the TextBox
                InputFilePathTextBox.Text = filePath;
                InputPreviewButton.Visibility = Visibility.Visible;

                // reset parsed flag
                contentParsed = false;
            }

            // Reset notification if its showing
            ResetNotification();
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
                    OutputFilePathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        /// <summary>Handles preview button click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseFileContent() < 0) { return; } }

            // Create the preview window
            PreviewWindow previewWindow = new PreviewWindow();

            if ((bool)CButton.IsChecked)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                previewWindow.AddPreviewTab("C", cContent);
            }

            if ((bool)CppButton.IsChecked)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                previewWindow.AddPreviewTab("C++", cppContent);
            }

            if ((bool)CSharpButton.IsChecked)
            {
                string csharpContent = GetGeneratedContentForType(OutputType.CSharp);
                previewWindow.AddPreviewTab("C#", csharpContent);
            }

            // Show the preview window
            previewWindow.ShowDialog();
        }

        /// <summary>Handles preview button click for input file</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the preview window
            PreviewWindow previewWindow = new PreviewWindow();

            // Read the text and add preview tab to preview tabs. 
            string inputFileContent = File.ReadAllText(parseView.InputFilePathTextBox.Text);
            previewWindow.AddPreviewTab("Input File", inputFileContent);

            // Show the preview window
            previewWindow.ShowDialog();
        }

        /// <summary>Handles generate button click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseFileContent() < 0) { return; } }

            string outputFolderPath = OutputFilePathTextBox.Text;

            int status = 0;

            if ((bool)CButton.IsChecked)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.C, cContent);
            }

            if ((bool)CppButton.IsChecked)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.Cpp, cppContent);
            }

            if ((bool)CSharpButton.IsChecked)
            {
                string csharpContent = GetGeneratedContentForType(OutputType.CSharp);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.CSharp, csharpContent);
            }

            if ((bool)FddButton.IsChecked)
            {
                status += GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, outputFolderPath);
            }

            if (status == 0)
            {
                NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryGreenColor");
                NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                NotificationTextBlock.Text = "COMPLETE";
            }
            else
            {
                NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryRedColor");
                NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                NotificationTextBlock.Text = "FAILED";
            }

            // Create and start the timer
            notificationTimer = new DispatcherTimer();
            notificationTimer.Interval = TimeSpan.FromSeconds(3);
            notificationTimer.Tick += (s, args) =>
            {
                ResetNotification(); // Reset the notification after 3 seconds
                notificationTimer.Stop(); // Stop the timer
            };
            notificationTimer.Start();
        }

    }
}
