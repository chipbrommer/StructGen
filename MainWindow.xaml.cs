using StructGen.Objects;
using StructGen.Pages;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Window = System.Windows.Window;


namespace StructGen
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance = new();
        public static string programDataPath = string.Empty;
        private View previousView;
        private View currentView;
        private readonly string companyFolder = "InnovativeConcepts";
        private readonly string applicationFolder = "StructGen";
        private readonly string settingsFileName = @"\settings.json";
        internal static SettingsFile<Objects.Settings> settingsFile;
        internal static Objects.Settings settings;

        /// Views
        static private Pages.Document documentView = new();
        static private Pages.Main mainView = new();
        static private Pages.Parse parseView = new();
        static private Pages.Settings settingsView = new();
        static private Pages.Startup startupView = new();

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

            programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), companyFolder, applicationFolder);

            // Ensure the ProgramData folder exists, create it if it doesn't
            if (!Directory.Exists(programDataPath))
            {
                Directory.CreateDirectory(programDataPath);
            }

            // Handle settings file
            string settingsFilePath = programDataPath + settingsFileName;
            settingsFile = new SettingsFile<Objects.Settings>(settingsFilePath);

            // Attempt to load settings
            if (!settingsFile.Load() || settingsFile.data == null)
            {
                settingsFile.data = new();
            }

            // set theme based on settings
            settings = settingsFile.data;
            ThemeController.SetTheme(settings.theme);

            ChangeView(View.Main);

            programDataPath = string.Empty;

            documentView = new Pages.Document();
            mainView = new Pages.Main();
            parseView = new Pages.Parse();
            settingsView = new Pages.Settings();
            startupView = new Pages.Startup();

            Instance = this;
        }

        public void ChangeView(View view)
        {
            switch(view)
            {
                case View.Document: ContentArea.Content = documentView; previousView = view; break;
                case View.Main:     ContentArea.Content = mainView;     previousView = view; break;
                case View.Parse:    ContentArea.Content = parseView;    previousView = view; break;
                case View.Settings: ContentArea.Content = settingsView; break;
                case View.Startup:  ContentArea.Content = startupView;  break;
                default:                                                break;
            }

            currentView = view;
        }

        /// <summary>Shows an error message</summary>
        /// <param name="message"> -[in]- message to be displayed</param>
        private static void ShowErrorMessage(string message)
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
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json",
                FilterIndex = 1,
                FileName = "layout" // Default file name
            };

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
                string? exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (exeDirectory == null || exeDirectory == string.Empty)
                {
                    // Display a failure message
                    ShowErrorMessage("Layout failed to download!");
                }
                else
                {
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
        }

        private void CreateHeaderFile_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(View.Parse);
        }

        private void CreateHeaderDocumentation_Click(object sender, RoutedEventArgs e)
        {
            ChangeView(View.Document);
        }

        private void HomeView_Click(object sender, RoutedEventArgs e)
        {
            // Clear old data
            parseView.Reset();
            documentView.Reset();

            // Toggle Views
            ChangeView(View.Main);
        }

        private void OpenSettingsView(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentView == View.Settings)
            {
                ChangeView(previousView);
            }
            else
            {
                ChangeView(View.Settings);
            }
        }

        public string GetProgramFolder()
        {
            return programDataPath;
        }
    }
}
