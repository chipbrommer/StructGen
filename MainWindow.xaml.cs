﻿using Microsoft.Win32;
using StructGen.Objects;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
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

            contentParsed = false;
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

                // reset parsed flag
                contentParsed = false;
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

        /// <summary> Parses the input file based on its type</summary>
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
                    if (InputFilePathTextBox.Text.Length == 0) { ShowErrorMessage("Please select an input file."); }
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

        /// <summary>Handles preview button click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify content
            if(!contentParsed) { if(ParseFileContent() < 0) { return; } }

            // Create the preview window
            PreviewWindow previewWindow = new PreviewWindow();

            if ((bool)CRadioButton.IsChecked)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                previewWindow.AddPreviewTab("C", cContent);
            }

            if ((bool)CppRadioButton.IsChecked)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                previewWindow.AddPreviewTab("C++", cppContent);
            }

            if ((bool)CSharpRadioButton.IsChecked)
            {
                string csharpContent = GetGeneratedContentForType(OutputType.CSharp);
                previewWindow.AddPreviewTab("C#", csharpContent);
            }

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

            if ((bool)CRadioButton.IsChecked)
            {
                string cContent = GetGeneratedContentForType(OutputType.C);
                SaveGeneratedContentToFile(outputFolderPath, OutputType.C, cContent);
            }

            if ((bool)CppRadioButton.IsChecked)
            {
                string cppContent = GetGeneratedContentForType(OutputType.Cpp);
                SaveGeneratedContentToFile(outputFolderPath, OutputType.Cpp, cppContent);
            }

            if ((bool)CSharpRadioButton.IsChecked)
            {
                string csharpContent = GetGeneratedContentForType(OutputType.CSharp);
                SaveGeneratedContentToFile(outputFolderPath, OutputType.CSharp, csharpContent);
            }
        }

        /// <summary>Saves received content to a file with the file ending for the output type</summary>
        /// <param name="outputFolderPath">Folder path to save generated file to.</param>
        /// <param name="outputType">Type of the file to be created.</param>
        /// <param name="content">Content to be written to file.</param>
        private void SaveGeneratedContentToFile(string outputFolderPath, OutputType outputType, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show($"No content to save for {outputType}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string outputExtension = ".txt";

            switch(outputType)
            {
                case OutputType.C: outputExtension = ".h"; break;
                case OutputType.Cpp: outputExtension = ".h"; break;
                case OutputType.CSharp: outputExtension = ".cs"; break;
            }

            string fileName = $"{parsedContent.FileName}.{outputExtension}";
            string filePath = System.IO.Path.Combine(outputFolderPath, fileName);

            // Write the generated content to the file
            System.IO.File.WriteAllText(filePath, content);
        }

        /// <summary>Shows an error message</summary>
        /// <param name="message"> -[in]- message to be displayed</param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
