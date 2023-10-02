using Aspose.Words;
using Aspose.Words.Saving;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace StructGen
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow : Window
    {
        // Holds path for the displayed XPS file. 
        string xpsFilePath = string.Empty;

        /// <summary>Default constructor</summary>
        public DocumentWindow()
        {
            InitializeComponent();
            Closing += DocumentWindow_Closing;
        }

        /// <summary>Handles updating the visible document for the window</summary>
        /// <param name="filepath"></param>
        public void UpdateDocumentContent(string filepath)
        {
            try
            {
                // Load the DOCX document
                Document doc = new Document(filepath);

                // Create an XpsSaveOptions object
                XpsSaveOptions saveOptions = new XpsSaveOptions();

                // Set the output XPS file path
                xpsFilePath = filepath + ".xps";

                // Save the document as XPS
                doc.Save(xpsFilePath, saveOptions);

                // Initialize XpsDocument and try to read it back in.
                XpsDocument xpsDocument = null;
              
                using (xpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read))
                {
                    // Get the FixedDocumentSequence from the XPS document
                    FixedDocumentSequence fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();

                    // Set the FixedDocumentSequence as the DocumentViewer's Document
                    documentViewer.Document = fixedDocumentSequence;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during loading
                MessageBox.Show($"Error loading XPS document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Additional closing procedures for the window</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if the XPS file exists before attempting to delete it
            if (xpsFilePath != string.Empty && File.Exists(xpsFilePath))
            {
                try
                {
                    File.Delete(xpsFilePath);
                }
                catch(Exception)
                {
                   
                }
            }
        }
    }
}
