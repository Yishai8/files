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
        public string saveCustomView(TreeView tree, string viewName, bool replace)
        {
            ItemCollection items = null;
            if (!replace)
                items = tree.Items;
            else
            {
                TreeViewItem item = (TreeViewItem)tree.Items[0];
                items = item.Items; //without the root item=tree name
            }

            return Tags.XMLFile.SaveView(items, viewName, replace);
        }

        public void deleteCustomView(string viewName)
        {

            Tags.XMLFile.deleteView(viewName);
        }

        public List<string> getCustomViewsList()
        {
            return Tags.XMLFile.getViewList();
        }

        public void LoadCustomView(TreeView t, string ViewName)
        {
            t.Items.Clear();
            Tags.XMLFile.BuildTree(t, ViewName);
        }

        public void createViewByTag(string iscategory, string tag, TreeView t)
        {

            var Tagslist = Tags.TagManagment.getPathsByTag(tag, iscategory);
            List<List<string>> paths = new List<List<string>>();
            foreach(List<Tags.TagFilter> l in Tagslist)
            {
                List<String> list = new List<string>();
                int count = 0;
                foreach (Tags.TagFilter singleList in l)
                {
                    if (count < 2)
                        list.Add(singleList.FileTag);
                    else
                        list.Add(singleList.path);
                }
                paths.Add(list);
                
            }
            Tags.XMLFile.AddViewNode(paths, tag);
            t.Items.Clear();
            Populate(tag, null, t, null, true);
            TreeViewItem _item =
    (TreeViewItem)t.ItemContainerGenerator.Items[0];
            int nodeLocation = 0;
            foreach (List<string> list in paths)
            {
                Populate(list[0] + "." + list[1], null, null, _item, true);
                TreeViewItem _item1 = (TreeViewItem)_item.Items[nodeLocation];
                for (int i = 2; i < list.Count; i++)
                {
                    Populate(list[i], list[i], null, _item1, true);
                }
                nodeLocation++;
            }


        }

        private void Populate(string header, string tag, TreeView _root, TreeViewItem _child, bool isfile)       //create the tree view
        {
            try
            {
                //Icon ic = SysIcon.OfPath(tag);
                TreeViewItem _driitem = new TreeViewItem();
                _driitem.Tag = tag;
                _driitem.Header = header;

                _driitem.Expanded += new RoutedEventHandler(_driitem_Expanded);
                if (!isfile)
                {
                    _driitem.Items.Add(new TreeViewItem());

                    _driitem.IsExpanded = true;

                }

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

        public List<string> getTaggedPaths(TreeViewItem item)
        {
            ItemCollection items = item.Items;
            List<string> paths = new List<string>();
            if (item.Tag.ToString() != "Custom Folder")
                paths.Add(item.Tag.ToString()); //tagged top level
            for (int i = 0; i < items.Count; i++)

            {
                FileAttributes attr = FileAttributes.Directory; //default
                TreeViewItem node = (TreeViewItem)items[i];
                bool isFile = false;


                if (node.Tag.ToString() != "Custom Folder")
                {
                    isFile = new Uri(node.Tag.ToString()).AbsolutePath.Split('/').Last().Contains('.');
                }
                if (isFile)
                    attr = File.GetAttributes(node.Tag.ToString());

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (node.Items.Count > 0)
                        processTree(node, paths);


                }


                paths.Add(node.Tag.ToString());

            }
            return paths.Distinct().ToList();
        }

        public void processTree(TreeViewItem treeItems, List<string> paths)
        {

            List<TreeViewItem> children = GetChildren(treeItems);
            for (int i = 0; i < children.Count; i++)
            {

                TreeViewItem node = children[i];

                if (node.Header != null)
                {
                    FileAttributes attr = FileAttributes.Directory; //default
                    if (node.Tag.ToString() != "Custom Folder") //item dropped on is not custom folder
                        paths.Add(node.Tag.ToString());

                    if (node.Items.Count > 0)
                    {


                        processTree(node, paths);
                    }
                    else
                    {

                        //parent.Add(itemToAdd);
                    }
                }


            }
            return;

        }

        static List<TreeViewItem> GetChildren(TreeViewItem parent)
        {
            List<TreeViewItem> children = new List<TreeViewItem>();

            if (parent != null)
            {
                foreach (var item in parent.Items)
                {
                    TreeViewItem child = item as TreeViewItem;

                    if (child == null)
                    {
                        child = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                    }

                    children.Add(child);
                }
            }

            return children;
        }

        public void getComplexTags(TreeView tv,List<string> filterParams)
        {
            List<string> mutualPaths = new List<string>();
            foreach (string param in filterParams)
            {
                var Tagslist = Tags.TagManagment.getPathsByTag(param, "Main+SubCategory");
                Tagslist[0].RemoveRange(0, 2);
                mutualPaths.AddRange(Tagslist[0]);
            }
            mutualPaths = mutualPaths.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            MakeTreeFromPaths(tv,mutualPaths);

        }

        public void MakeTreeFromPaths(TreeView tv,List<string> paths, string rootNodeName = "", char separator = '/')
        {


            TreeViewItem root = new TreeViewItem();
            TreeViewItem node = new TreeViewItem();
            TreeViewItem par = new TreeViewItem();
            //treeView1.Nodes.Add(root);
           
           


            foreach (string filePath in paths) // myList is your list of paths
            {
               
                string tag = string.Empty;
                TreeViewItem lastNode = null;
                foreach (string pathBits in filePath.Split('\\'))
                {
                   
                    //var used for finding existing node
                    TreeViewItem existingNode = null;
                    //new node to add to tree
                    TreeViewItem newNode = new TreeViewItem();
                    newNode.Header= pathBits;
                    if (tag == string.Empty)
                    {
                        newNode.Tag = pathBits;
                        tag = pathBits;
                    }
                        
                    else
                    {
                        tag = tag + "\\" + pathBits;
                        newNode.Tag = tag;

                    }
                        
                    //collection of subnodes to search for node name (to check if node exists)
                    //in first pass, that collection is collection of treeView's nodes (first level)
                    ItemCollection nodesCollection = tv.Items;

                    //with first pass, this will be null, but in every other, this will hold last added node.
                    if (lastNode != null)
                    {
                        nodesCollection = lastNode.Items;
                    }

                    //look into collection if node is already there (method checks only first level of node collection)
                    existingNode = FindNode(nodesCollection, pathBits);
                    //node is found? In that case, skip it but mark it as last "added"
                    if (existingNode != null)
                    {
                        lastNode = existingNode;
                        continue;
                    }
                    else //not found so add it to collection and mark node as last added.
                    {
                        nodesCollection.Add(newNode);
                        lastNode = newNode;
                    }
                 
                }
            }
            
        }

        private ItemCollection CheckChildExists(ItemCollection items, string tag,string parent)
        {


            ItemCollection newCol= null;
            foreach (TreeViewItem i in items)
            {
                if (i.Tag.ToString() == tag)
                {
                    newCol = i.Items;
                    break;

                }
                    
            }
            return newCol;
        }


        private void AddNode(TreeViewItem node, string key,string parent,string tag)
        {
            if (node.Items.Cast<TreeViewItem>().Any(item => item.Header.ToString() == key))

            {
                IEnumerable<TreeViewItem> items = from node1 in node.Items.OfType<TreeViewItem>().Where((x) => x.Header.ToString()== key)
                                                  select node1;
                foreach(TreeViewItem item in items)
                {
                    node = item;
                }
                
                //node.Items.Cast<TreeViewItem>().Where(item => item.Header.ToString() = key);
            }
            else
            {
                if(node.Header==null)
                {
                    node.Header = key;
                    node.Tag = tag;
                }
                else
                {
                    TreeViewItem newtvi = new TreeViewItem();
                    newtvi.Header = key;
                    node.Items.Add(newtvi);
                    node = newtvi;

                }

            }
           
        }

   

        private TreeViewItem FindNode(ItemCollection nodeCollectionToSearch, string nodeText)
        {
            var nodesToSearch = nodeCollectionToSearch.Cast<TreeViewItem>();
            var foundNode = nodesToSearch.FirstOrDefault(n => n.Header.ToString() == nodeText);
            return foundNode;
        }


    }

}
