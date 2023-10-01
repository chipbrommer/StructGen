using StructGen.Objects;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
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

        /// Views
        static Pages.Document   documentView;
        static Pages.Main       mainView;
        static Pages.Parse      parseView;
        static Pages.Settings   settingsView;
        static Pages.Startup startupView;

        public enum View
        {
            Document,
            Main,
            Parse,
            Settings,
            Startup
        }

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

            documentView = new Pages.Document();
            mainView = new Pages.Main();
            parseView = new Pages.Parse();
            settingsView = new Pages.Settings();
            startupView = new Pages.Startup();

        }

        public void ChangeView(View view)
        {
            switch(view)
            {
                case View.Document: ContentArea.Content = documentView; break;
                case View.Main:     ContentArea.Content = mainView;     break;
                case View.Parse:    ContentArea.Content = parseView;    break;
                case View.Settings: ContentArea.Content = settingsView; break;
                case View.Startup:  ContentArea.Content = startupView;  break;
                default:                                                break;
            }
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
            string filePath = documentView.InputFilePathTextBox.Text;

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
                    if (documentView.InputFilePathTextBox.Text == string.Empty)
                    {
                        ShowErrorMessage("Please select an input file.");
                    }
                    else if (documentView.OutputFilePathTextBox.Text == string.Empty)
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
            ContentArea.Content = parseView;
        }

        private void CreateHeaderDocumentation_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = documentView;
        }

        private void HomeView_Click(object sender, RoutedEventArgs e)
        {
            // Toggle Views
            ContentArea.Content = mainView;

            // Clear old data in the parseView
            parsedContent = new HeaderFile();
            contentParsed = false;

            // Clear old data
            parseView.Reset();
            documentView.Reset();
        }

        private void OpenSettingsView(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ContentArea.Content = settingsView;
        }
    }
}
