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
    }

    /// <summary> A class to hold a files information </summary>
    public class FileInfo
    {
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public string FileHeader { get; set; }
    }

    /// <summary> A class to represent a structure </summary>
    public class Structure
    {
        public string StructureName { get; set; }
        public string StructureComment { get; set; }
        public string StructurePacking { get; set; }
        public List<Variable> Variables { get; set; }
    }

    /// <summary> A class to represent a variable </summary>
    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }
    }
}