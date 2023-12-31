﻿using StructGen.Objects;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Window = System.Windows.Window;


namespace StructGen
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HeaderFile parsedContent;
        bool contentParsed;
        private DispatcherTimer notificationTimer;
        string programDataPath;

        /// <summary>Enum for available output types</summary>
        public enum OutputType
        {
            C,
            Cpp,
            CSharp
        }

        /// <summary>Default constructor</summary>
        public MainWindow()
        {
            InitializeComponent();

            StartupTasks();

            parsedContent = new HeaderFile();
            contentParsed = false;
        }

        /// <summary>A function to handle specific startup tasks.</summary>
        private void StartupTasks()
        {
            string companyFolder = "InnovativeConcepts";
            string applicationFolder = "StructGen";

            programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), companyFolder, applicationFolder);

            // Ensure the ProgramData folder exists, create it if it doesn't
            if (!Directory.Exists(programDataPath))
            {
                Directory.CreateDirectory(programDataPath);
            }
        }

        /// <summary> Parses the input file based on its type.</summary>
        /// <returns>0 if successful, else -1.</returns>
        private int ParseFileContent()
        {
            string filePath = PV_InputFilePathTextBox.Text;

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
                    if (PV_InputFilePathTextBox.Text == string.Empty) 
                    { 
                        ShowErrorMessage("Please select an input file."); 
                    }
                    else if(PV_OutputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an output location.");
                    }
                    else { ShowErrorMessage("Unsupported input file."); }
                    return -1;
            }

            contentParsed = true;
            return 0;
        }

        /// <summary>Parse the input header file based on its type.</summary>
        /// <returns>0 if successful, else -1.</returns>
        private int ParseHeaderContent()
        {
            string filePath = DV_InputFilePathTextBox.Text;

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
                    if (DV_InputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an input file.");
                    }
                    else if (DV_OutputFilePathTextBox.Text == string.Empty)
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
            switch (outputType)
            {
                case OutputType.C:
                    return GeneratorInterface.GenerateHeaderC(parsedContent);

                case OutputType.Cpp:
                    return GeneratorInterface.GenerateHeaderCPP(parsedContent);

                case OutputType.CSharp:
                    return GeneratorInterface.GenerateHeaderCSHARP(parsedContent);

                default:
                    return string.Empty;
            }
        }

        /// <summary>Resets the notification to starting state.</summary>
        private void ResetNotification()
        {
            PV_NotificationTextBlock.Background = Brushes.Transparent;
            PV_NotificationTextBlock.Foreground = Brushes.Black;
            PV_NotificationTextBlock.Text = "";
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
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //////////////////////////////////////////////////////
        /// Click Event Handlers
        //////////////////////////////////////////////////////

        /// <summary>Displays a file selector box for the user to browse a file to read in</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PV_InputBrowseButton_Click(object sender, RoutedEventArgs e)
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
                PV_InputFilePathTextBox.Text = filePath;
                PV_InputPreviewButton.Visibility = Visibility.Visible;

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
                    Button clickedButton = sender as Button;

                    // Update the output field text based on which area button was clicked. 
                    if (clickedButton != null)
                    {
                        switch (clickedButton.Name)
                        {
                            case "PV_BrowseOutputButton":
                                PV_OutputFilePathTextBox.Text = folderDialog.SelectedPath;
                                break;

                            case "DV_BrowseOutputButton":
                                DV_OutputFilePathTextBox.Text = folderDialog.SelectedPath;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>Handles preview button click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PV_PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if(!contentParsed) { if(ParseFileContent() < 0) { return; } }

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
        private void PV_InputPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the preview window
            PreviewWindow previewWindow = new PreviewWindow();

            // Read the text and add preview tab to preview tabs. 
            string inputFileContent = File.ReadAllText(PV_InputFilePathTextBox.Text);
            previewWindow.AddPreviewTab("Input File", inputFileContent);

            // Show the preview window
            previewWindow.ShowDialog();
        }

        /// <summary>Handles generate button click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PV_GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseFileContent() < 0) { return; } }

            string outputFolderPath = PV_OutputFilePathTextBox.Text;

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

            if((bool)FddButton.IsChecked)
            {
                status += GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, outputFolderPath);
            }

            if (status == 0)
            {
                PV_NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryGreenColor");
                PV_NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                PV_NotificationTextBlock.Text = "COMPLETE";
            }
            else
            {
                PV_NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryRedColor");
                PV_NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                PV_NotificationTextBlock.Text = "FAILED";
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

        /// <summary>Handles download button click event.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadLayouts_Click(object sender, RoutedEventArgs e)
        {
            // Create a SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = "layout"; // Default file name

            // Show the SaveFileDialog and get the result
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Get the selected file name
                string filePath = saveFileDialog.FileName;

                // Determine the file format based on the selected filter
                string fileFormat = Path.GetExtension(filePath).ToLower();

                // Prepare the layout data based on the selected format
                string layoutData = "";

                // Get the directory where the executable is located
                string exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (fileFormat == ".xml")
                {
                    // Construct the full path to the XML layout file
                    string xmlLayoutPath = Path.Combine(exeDirectory, "Resources", "xmlLayout.xml");

                    // Load the layout data from the file
                    layoutData = File.ReadAllText(xmlLayoutPath);
                }
                else if (fileFormat == ".json")
                {
                    // Construct the full path to the XML layout file
                    string jsonLayoutPath = Path.Combine(exeDirectory, "Resources", "jsonLayout.json");

                    // Load the layout data from the file
                    layoutData = File.ReadAllText(jsonLayoutPath);
                }

                // Save the layout data to the selected file
                File.WriteAllText(filePath, layoutData);

                // Display a success message
                System.Windows.MessageBox.Show("Layout downloaded successfully!");
            }
        }

        private void CreateHeaderFile_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Views
            homeView.Visibility = Visibility.Hidden;
            parseView.Visibility = Visibility.Visible;
            documentView.Visibility = Visibility.Hidden;
        }

        private void CreateHeaderDocumentation_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Views
            homeView.Visibility = Visibility.Hidden;
            parseView.Visibility = Visibility.Hidden;
            documentView.Visibility = Visibility.Visible;
        }

        private void HomeView_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Views
            homeView.Visibility = Visibility.Visible;
            parseView.Visibility = Visibility.Hidden;
            documentView.Visibility = Visibility.Hidden;

            // Clear old data in the parseView
            parsedContent = new HeaderFile();
            contentParsed = false;
            PV_InputFilePathTextBox.Text = string.Empty;
            PV_OutputFilePathTextBox.Text = string.Empty;
            CButton.IsChecked = false;
            CppButton.IsChecked = false;
            CSharpButton.IsChecked = false;
            FddButton.IsChecked = false;

            // Clear old data in docView
            DV_InputFilePathTextBox.Text = string.Empty;
            DV_OutputFilePathTextBox.Text = string.Empty;

        }

        private void DV_InputBrowseButton_Click(object sender, RoutedEventArgs e)
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
                DV_InputFilePathTextBox.Text = filePath;

                // reset parsed flag
                contentParsed = false;
            }
        }

        private void DV_PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            // Create the preview window
            DocumentWindow docWindow = new DocumentWindow();

            // Send it the document filepath
            docWindow.UpdateDocumentContent(DV_InputFilePathTextBox.Text); 

            // Show the preview window
            docWindow.ShowDialog();
        }

        private void DV_GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if (!contentParsed) { if (ParseHeaderContent() < 0) { return; } }

            string outputFolderPath = DV_OutputFilePathTextBox.Text;

            int status = 0;

            // Generate the File Description Document
            status += GeneratorInterface.GenerateFileDescriptionDocument(parsedContent, outputFolderPath);

            if (status == 0)
            {
                DV_NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryGreenColor");
                DV_NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                DV_NotificationTextBlock.Text = "COMPLETE";
            }
            else
            {
                DV_NotificationTextBlock.Background = (SolidColorBrush)FindResource("PrimaryRedColor");
                DV_NotificationTextBlock.Foreground = (SolidColorBrush)FindResource("PrimaryTextColor");
                DV_NotificationTextBlock.Text = "FAILED";
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

        private void OpenSettingsView(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
