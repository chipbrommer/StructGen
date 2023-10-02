using StructGen.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace StructGen.Pages
{
    /// <summary>
    /// Interaction logic for Document.xaml
    /// </summary>
    public partial class Document : Page
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

        public Document()
        {
            InitializeComponent();

            notificationTimer = new DispatcherTimer();
            parsedContent = new HeaderFile();
            contentParsed = false;

            Reset();
        }

        public void Reset()
        {
            parsedContent = new();
            contentParsed = false;
            InputFilePathTextBox.Text = string.Empty;
            OutputFilePathTextBox.Text = string.Empty;
            SetNotification(Notification.Off);
        }

        private void SetNotification(Notification noti)
        {
            switch(noti)
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

        /// <summary>Shows an error message</summary>
        /// <param name="message"> -[in]- message to be displayed</param>
        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>Parse the input header file based on its type.</summary>
        /// <returns>0 if successful, else -1.</returns>
        private int ParseHeaderContent()
        {
            string filePath = InputFilePathTextBox.Text;

            // Determine the file type based on its extension
            string fileExtension = System.IO.Path.GetExtension(filePath);

            switch (fileExtension.ToLower())
            {
                case ".h":
                    parsedContent = GeneratorInterface.ParseCppHeaderFile(filePath);
                    break;
                case ".cs":
                    parsedContent = GeneratorInterface.ParseCsharpHeaderFile(filePath);
                    break;
                default:
                    if (InputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an input file.");
                    }
                    else 
                    { 
                        ShowErrorMessage("Unsupported input file."); 
                    }
                    return -1;
            }

            contentParsed = true;
            return 0;
        }

        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file dialog
            OpenFileDialog openFileDialog = new()
            {
                // Set the filter
                Filter = "C/C++ Headder (*.h)|*.h|C# Files (*.cs)|*.cs|All Files|*.*",
                Title = "Select an input file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = openFileDialog.FileName;

                // Display the file path in the TextBox
                InputFilePathTextBox.Text = filePath;

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

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            // Verify Output location
            if (OutputFilePathTextBox.Text == string.Empty)
            {
                ShowErrorMessage("Please select an output location.");
                return;
            }

            // Create the temporary output file
            string filename = $"{parsedContent.FileInformation.FileName} - File Description Document.docx";
            GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, OutputFilePathTextBox.Text, filename);

            // Create the preview window
            DocumentWindow docWindow = new();

            // Send it the document filepath
            string filePath = System.IO.Path.Combine(OutputFilePathTextBox.Text, filename);
            docWindow.UpdateDocumentContent(filePath);

            // Show the preview window
            docWindow.ShowDialog();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify Output location
            if (OutputFilePathTextBox.Text == string.Empty)
            {
                ShowErrorMessage("Please select an output location.");
                return;
            }

            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            string outputFolderPath = OutputFilePathTextBox.Text;

            int status = 0;

            // Generate the File Description Document
            string filename = $"{parsedContent.FileInformation.FileName} - File Description Document";
            status += GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, outputFolderPath, filename);

            if (status == 0)
            {
                SetNotification(Notification.Success);
            }
            else
            {
                SetNotification(Notification.Failed);
            }

            // Create and start the timer
            notificationTimer = new()
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            notificationTimer.Tick += (s, args) =>
            {
                SetNotification(Notification.Off); // Reset the notification after 3 seconds
                notificationTimer.Stop(); // Stop the timer
            };
            notificationTimer.Start();
        }
    }
}
