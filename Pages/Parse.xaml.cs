using StructGen.Objects;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using static StructGen.MainWindow;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace StructGen.Pages
{
    /// <summary>
    /// Interaction logic for Parse.xaml
    /// </summary>
    public partial class Parse : Page
    {
        private DispatcherTimer notificationTimer;
        private HeaderFile parsedContent;
        private bool contentParsed;

        private enum Notification
        {
            Off,
            Failed,
            Success
        }

        public Parse()
        {
            InitializeComponent();

            notificationTimer = new DispatcherTimer();
            parsedContent = new HeaderFile();
            contentParsed = false;
        }

        public void Reset()
        {
            InputFilePathTextBox.Text = string.Empty;
            OutputFilePathTextBox.Text = string.Empty;
            CButton.IsChecked = false;
            CppButton.IsChecked = false;
            CSharpButton.IsChecked = false;
            FddButton.IsChecked = false;

            parsedContent = new HeaderFile();
            contentParsed = false;
            SetNotification(Notification.Off);
        }

        private void SetNotification(Notification noti)
        {
            switch (noti)
            {
                case Notification.Failed:
                    {
                        NotificationTextBlock.Visibility = Visibility.Visible;
                        NotificationTextBlock.SetResourceReference(TextBlock.BackgroundProperty, "PrimaryRedColor");
                        NotificationTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryTextColor");
                        NotificationTextBlock.Text = "Failed";
                        break;
                    }
                case Notification.Success:
                    {
                        NotificationTextBlock.Visibility = Visibility.Visible;
                        NotificationTextBlock.SetResourceReference(TextBlock.BackgroundProperty, "PrimaryGreenColor");
                        NotificationTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryTextColor");
                        NotificationTextBlock.Text = "Success";
                        break;
                    }
                // Intentional fall through
                default:
                case Notification.Off:
                    {
                        NotificationTextBlock.Visibility = Visibility.Hidden;
                        break;
                    }
            }
        }

        /// <summary> Parses the input file based on its type.</summary>
        /// <returns>0 if successful, else -1.</returns>
        private int ParseFileContent()
        {
            string filePath = InputFilePathTextBox.Text;

            // Determine the file type based on its extension
            string fileExtension = System.IO.Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".json":
                    parsedContent = GeneratorInterface.HandleJsonFile(filePath);
                    break;
                case ".csv":
                    parsedContent = GeneratorInterface.HandleCsvFile(filePath);
                    break;
                case ".xml":
                    parsedContent = GeneratorInterface.HandleXmlFile(filePath);
                    break;
                default:
                    if (InputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an input file.");
                    }
                    else if (OutputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an output location.");
                    }
                    else { ShowErrorMessage("Unsupported input file."); }
                    return -1;
            }

            contentParsed = true;
            return 0;
        }

        /// <summary>Generates the content based on desired output type</summary>
        /// <param name="outputType"> -[in]- type of the desired output</param>
        /// <returns>String containing the parsed content</returns>
        private string GetGeneratedContentForType(OutputType outputType)
        {
            return outputType switch
            {
                OutputType.C => GeneratorInterface.GenerateHeaderC(parsedContent),
                OutputType.Cpp => GeneratorInterface.GenerateHeaderCPP(parsedContent),
                OutputType.CSharp => GeneratorInterface.GenerateHeaderCSHARP(parsedContent),
                _ => string.Empty,
            };
        }

        /// <summary>Saves received content to a file with the file ending for the output type</summary>
        /// <param name="outputFolderPath">Folder path to save generated file to.</param>
        /// <param name="outputType">Type of the file to be created.</param>
        /// <param name="content">Content to be written to file.</param>
        /// <returns>0 if successful, else -1.</returns>
        private int SaveGeneratedContentToFile(string outputFolderPath, OutputType outputType, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show($"No content to save for {outputType}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }

            string outputExtension = "txt";

            switch (outputType)
            {
                case OutputType.C: outputExtension = "h"; break;
                case OutputType.Cpp: outputExtension = "h"; break;
                case OutputType.CSharp: outputExtension = "cs"; break;
            }

            string outputFilename;

            if (string.IsNullOrEmpty(parsedContent.FileInformation.FileName))
            { outputFilename = "DefaultFileName"; }
            else
            { outputFilename = parsedContent.FileInformation.FileName; }

            string fileName = $"{outputFilename}.{outputExtension}";
            string filePath = System.IO.Path.Combine(outputFolderPath, fileName);

            // Write the generated content to the file
            System.IO.File.WriteAllText(filePath, content);

            return 0;
        }

        /// <summary>Shows an error message</summary>
        /// <param name="message"> -[in]- message to be displayed</param>
        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>Displays a file selector box for the user to browse a file to read in</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file dialog
            OpenFileDialog openFileDialog = new()
            {
                // @todo - reallow csv files when a template gets created and implemented - CSV Files (*.csv)|*.csv|
                Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml*|All Files|*.*",
                Title = "Select an input file"
            };

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
            SetNotification(Notification.Off);
        }

        /// <summary>Displays a file selector box for the user to browse an output location</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using FolderBrowserDialog folderDialog = new();
            DialogResult result = folderDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                OutputFilePathTextBox.Text = folderDialog.SelectedPath;
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
            PreviewWindow previewWindow = new();

            if (CButton.IsChecked.HasValue && CButton.IsChecked.Value)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                previewWindow.AddPreviewTab("C", cContent);
            }

            if (CppButton.IsChecked.HasValue && CppButton.IsChecked.Value)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                previewWindow.AddPreviewTab("C++", cppContent);
            }

            if (CSharpButton.IsChecked.HasValue && CSharpButton.IsChecked.Value)
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
            PreviewWindow previewWindow = new();

            // Read the text and add preview tab to preview tabs. 
            string inputFileContent = File.ReadAllText(InputFilePathTextBox.Text);
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

            if (CButton.IsChecked.HasValue && CButton.IsChecked.Value)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.C, cContent);
            }

            if (CppButton.IsChecked.HasValue && CppButton.IsChecked.Value)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.Cpp, cppContent);
            }

            if (CSharpButton.IsChecked.HasValue && CSharpButton.IsChecked.Value)
            {
                string csharpContent = GetGeneratedContentForType(OutputType.CSharp);
                status += SaveGeneratedContentToFile(outputFolderPath, OutputType.CSharp, csharpContent);
            }

            if (FddButton.IsChecked.HasValue && FddButton.IsChecked.Value)
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
            notificationTimer = new()
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            notificationTimer.Tick += (s, args) =>
            {
                SetNotification(Notification.Off);  // Reset the notification after 3 seconds
                notificationTimer.Stop();           // Stop the timer
            };
            notificationTimer.Start();
        }

    }
}
