using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructGen.Objects
{
    /// <summary> A class to represent an output header file </summary>
    public class HeaderFile
    {
        public FileInfo File { get; set; }
        public List<Structure> Structures { get; set; }
        public FDD DescDoc { get; set; }

        public HeaderFile()
        {
            // Initialize properties with default values
            File = new FileInfo();
            Structures = new List<Structure>();
            DescDoc = new FDD();
        }
    }

    /// <summary> A class to hold a files information </summary>
    public class FileInfo
    {
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public string FileHeader { get; set; }

        public FileInfo()
        {
            ProjectName = string.Empty;
            FileName = string.Empty;
            FileVersion = string.Empty;
            FileHeader = string.Empty;
        }
    }

    /// <summary> A class to represent a structure </summary>
    public class Structure
    {
        public string StructureName { get; set; }
        public string StructureComment { get; set; }
        public string StructurePacking { get; set; }
        public string AdditionalInformation { get; set; }
        public List<Variable> Variables { get; set; }

        public Structure()
        {
            // Initialize properties with default values
            StructureName = string.Empty;
            StructureComment = string.Empty;
            StructurePacking = string.Empty;
            AdditionalInformation = string.Empty;
            Variables = new List<Variable>();
        }
    }

    /// <summary> A class to represent a variable </summary>
    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }

        public Variable()
        {
            Name = string.Empty;
            Type = string.Empty;
            Comment = string.Empty;
        }
    }

    /// <summary> A class to hold accompanying File Description Document information</summary>
    public class FDD
    {
        public string Revision { get; set; }

        public FDD() 
        {
            Revision = "1";
        }
    }
}