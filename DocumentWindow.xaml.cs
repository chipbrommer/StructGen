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
        public DocumentWindow()
        {
            InitializeComponent();
        }

        public void UpdateDocumentContent(string filepath)
        {
            try
            {
                // Load the DOCX document
                Document doc = new Document(filepath);

                // Create an XpsSaveOptions object
                XpsSaveOptions saveOptions = new XpsSaveOptions();

                // Set the output XPS file path
                string xpsFilePath = filepath + ".xps";

                // Save the document as XPS
                doc.Save(xpsFilePath, saveOptions);

                // Load the XPS document
                XpsDocument xpsDocument = new XpsDocument(xpsFilePath, FileAccess.Read);

                // Get the FixedDocumentSequence from the XPS document
                FixedDocumentSequence fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();

                // Set the FixedDocumentSequence as the DocumentViewer's Document
                documentViewer.Document = fixedDocumentSequence;

                // Delete the temporary XPS file
                File.Delete(xpsFilePath);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during loading
                MessageBox.Show($"Error loading XPS document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
