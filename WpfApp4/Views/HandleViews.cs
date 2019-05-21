using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace WpfApp4.Views
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

        public void saveCustomView(TreeView tree)
        {
            Tags.XMLFile.SaveView(tree);
        }

        public void createViewSubTag(string tag,TreeView t)
        {
           
            var Tagslist = Tags.TagManagment.getPathsByTag(tag,true);
            Tags.XMLFile.AddViewNode(Tagslist, tag);
            t.Items.Clear();
            Populate(tag, t, null, true);
            TreeViewItem _item =
    (TreeViewItem)t.ItemContainerGenerator.Items[0];
            int nodeLocation = 0;
            foreach (List<string> list  in Tagslist)
            {
                Populate(list[0]+"."+list[1], null, _item, true);
                TreeViewItem _item1 = (TreeViewItem)_item.Items[nodeLocation];
                for (int i = 2; i < list.Count; i++) { 
                Populate(list[i], null, _item1, true);
                }
                nodeLocation++;
            }
            
           
        }

        private void Populate(string header,TreeView _root, TreeViewItem _child, bool isfile)       //create the tree view
        {
            try
            {
                //Icon ic = SysIcon.OfPath(tag);
                TreeViewItem _driitem = new TreeViewItem();
                //_driitem.Tag = tag;
                _driitem.Header = header;

                _driitem.Expanded += new RoutedEventHandler(_driitem_Expanded);
                if (!isfile)
                    _driitem.Items.Add(new TreeViewItem());

                if (_root != null)
                { _root.Items.Add(_driitem); }
                else { _child.Items.Add(_driitem); }
            }
            catch (System.NullReferenceException ex)
            { Console.WriteLine(ex.InnerException); }
            catch (System.UnauthorizedAccessException unauth)
            { Console.WriteLine(unauth.InnerException); }
        }

        void _driitem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem _item = (TreeViewItem)sender;
            if (_item.Items.Count == 1 && ((TreeViewItem)_item.Items[0]).Header == null)
            {
                _item.Items.Clear();

                try
                {

                    foreach (string dir in Directory.GetDirectories(_item.Tag.ToString()))
                    {
                        DirectoryInfo _dirinfo = new DirectoryInfo(dir);
                        //if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                           // Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, false);
                    }

                    foreach (string dir in Directory.GetFiles(_item.Tag.ToString()))
                    {
                        FileInfo _dirinfo = new FileInfo(dir);
                       // if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                           // Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, true);


                    }

                }
                catch (System.UnauthorizedAccessException unauth)
                { Console.WriteLine(unauth.InnerException); }

            }

        }
    }
}
