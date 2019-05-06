using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client.Viewer
{
    class SystemView
    {
        public static ICollection<TreeNode<singleView>> listOfViews { get; set; }

        private SystemView()
        {

        }

    }

    internal class singleView
    {
        public string viewBaseCategory { get; set; }
        public bool isMainCategory { get; set; }
        public singleView(string tag)
        {
            this.viewBaseCategory = tag;
        }

    }


}