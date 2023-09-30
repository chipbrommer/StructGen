using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructGen.Objects
{
    public class ViewController
    {
        public static ViewTypes CurrentView { get; set; }

        public enum ViewTypes
        {
            Main,
            Settings,
            Document,
            Parse,
            Startup
        }

        public void ChangeView(ViewTypes view)
        {
            
        }
    }
}
