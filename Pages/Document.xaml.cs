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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StructGen.Pages
{
    /// <summary>
    /// Interaction logic for Document.xaml
    /// </summary>
    public partial class Document : Page
    {
        public Document()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            InputFilePathTextBox.Text = string.Empty;
            OutputFilePathTextBox.Text = string.Empty;
        }

        public void SetNotification()
        {

        }

        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // @todo - reallow csv files when a template gets created and implemented - CSV Files (*.csv)|*.csv|
            openFileDialog.Filter = "C/C++ Headder (*.h)|*.h|C# Files (*.cs)|*.cs|All Files|*.*";
            openFileDialog.Title = "Select an input file";

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                string filePath = openFileDialog.FileName;

                // Display the file path in the TextBox
                InputFilePathTextBox.Text = filePath;

                // reset parsed flag
                contentParsed = false;
            }
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            // Create the preview window
            DocumentWindow docWindow = new DocumentWindow();

            // Send it the document filepath
            docWindow.UpdateDocumentContent(InputFilePathTextBox.Text);

            // Show the preview window
            docWindow.ShowDialog();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            string outputFolderPath = this.OutputFilePathTextBox.Text;

            int status = 0;

            // Generate the File Description Document
            status += GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, outputFolderPath);

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
