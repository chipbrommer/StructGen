﻿using StructGen.Objects;
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
        public string programDataPath = string.Empty;
        private View previousView;
        private View currentView;
        private readonly string companyFolder = "InnovativeConcepts";
        private readonly string applicationFolder = "StructGen";
        private readonly string settingsFileName = @"\settings.json";
        internal static SettingsFile<Objects.Settings> settingsFile;
        internal static Objects.Settings settings;

        /// Views
        static private Pages.Document documentView;
        static private Pages.Main mainView;
        static private Pages.Parse parseView;
        static private Pages.Settings settingsView;
        static private Pages.Startup startupView;

        public enum View
        {
            Document,
            Main,
            Parse,
            Settings,
            Startup
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
            settings = settingsFile.data;

            programDataPath = string.Empty;

            documentView = new Pages.Document();
            mainView = new Pages.Main(this);
            parseView = new Pages.Parse();
            settingsView = new Pages.Settings();
            startupView = new Pages.Startup();

            // Set the starting view.
            ChangeView(View.Main);

            // Update things based on settings.
            settingsView.SetThemeSelection(settings.theme);
            ThemeController.SetTheme(settings.theme);

        }

        /// <summary>Changes the view content</summary>
        /// <param name="view"></param>
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

        /// <summary>Handles request to go back to home view</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeView_Click(object sender, RoutedEventArgs e)
        {
            // Clear old data
            parseView.Reset();
            documentView.Reset();

            // Toggle Views
            DownloadLayoutsButton.Visibility = Visibility.Hidden;
            ChangeView(View.Main);
        }

        /// <summary>Handles opening and closing of Settings view</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsView_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
    }
}
