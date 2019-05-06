using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Viewer
{
    class HandleViews
    {

        // TreeNode<Views.singleView> root = new TreeNode<Views.singleView>();

        public void getPathsForView(string tag)
        {
            singleView rt = new singleView(tag);
            TreeNode<singleView> root = new TreeNode<singleView>(rt);
            //SystemView.listOfViews.Add(root);
        }
    }
}