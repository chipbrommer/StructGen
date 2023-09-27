using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructGen.Objects
{
    /// <summary> A class to represent an output file </summary>
    public class File
    {
        public string FileName { get; set; }
        public string FileVersion { get; set; }
        public string LayoutFileName { get; set; }
        public List<Structure> Structures { get; set; }
    }

    /// <summary> A class to represent a structure </summary>
    public class Structure
    {
        public string Name { get; set; }
        public string Comment { get; set; }
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