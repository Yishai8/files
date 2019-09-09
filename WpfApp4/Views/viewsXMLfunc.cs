using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

//handle views xml file
namespace WpfApp4.Views
{
    static class viewsXMLfunc
    {
        static readonly string viewFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Views.xml";//create xml on desktop
        static XDocument viewDoc = null;    //load the xml file to object


        public static string SaveView(ItemCollection treeItems, string viewName, bool replace)
        {
            XElement XMLElements = viewsXMLfunc.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLElements.Elements("CustomView")

.Where(v => (string)v.Attribute("name") == viewName);
            if (!replace && isViewExist.Any())
                return "name already exists";

            XElement CustomViewNode = new XElement("CustomView");
            CustomViewNode.Add(new XAttribute("name", viewName));

            //build view hierarchy for XML

            for (int i = 0; i < treeItems.Count; i++)
            {
                XElement itemToAdd = null;
                TreeViewItem node = (TreeViewItem)treeItems[i];
                FileAttributes attr = FileAttributes.Directory; //default
                if (node.Tag.ToString() != "Custom Folder") //item dropped on is not custom folder
                    attr = File.GetAttributes(node.Tag.ToString());

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    itemToAdd = new XElement("folder");
                    itemToAdd.Add(new XAttribute("name", node.Header));
                    itemToAdd.Add(new XAttribute("osLocation", node.Tag));
                    processTree(node, itemToAdd);

                }
                else
                {
                    itemToAdd = new XElement("file");
                    itemToAdd.Add(new XAttribute("name", node.Header));
                    itemToAdd.Add(new XAttribute("osLocation", node.Tag));
                }

                CustomViewNode.Add(itemToAdd);


            }
            XElement XMLBody = viewsXMLfunc.viewDoc.Element("root");
            if (replace)
            {
                if (!checkDifferenceInNodes(CustomViewNode, isViewExist))
                    return "success";
            }
            foreach (XElement el in isViewExist)
            {
                el.Remove();
            }

            XMLBody.Add(CustomViewNode);
            viewsXMLfunc.viewDoc.Save(viewsXMLfunc.viewFilePath);
            return "success";

        }

        public static bool checkDifferenceInNodes(XElement newElement, IEnumerable<XElement> isViewExist)
        {
            foreach (XElement el in isViewExist)
            {
                if (XNode.DeepEquals(el, newElement))
                    return false;
            }
            return true;
        }

        public static void deleteView(string viewName)
        {
            XElement XMLElements = viewsXMLfunc.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLElements.Elements("CustomView")

.Where(v => (string)v.Attribute("name") == viewName);
            foreach (XElement el in isViewExist)
            {
                el.Remove();
            }


            viewsXMLfunc.viewDoc.Save(viewsXMLfunc.viewFilePath);
        }


        public static string replaceCustomView(ItemCollection treeItems, string viewName)
        {
            return "x";
        }

        public static List<string> getViewList()
        {
            List<string> ListOfViews = new List<string>();
            var XMLElements = viewsXMLfunc.viewDoc.Element("root").Descendants("CustomView").Attributes("name").Select(x => x.Value);
            foreach (var el in XMLElements)
                ListOfViews.Add(el);
            return ListOfViews;

        }

        //XML View to folder stracture
        public static void BuildTree(TreeView treeView, string ViewName)
        {
            XElement Choosen = null;
            IEnumerable<XElement> view = viewsXMLfunc.viewDoc.Element("root").Elements("CustomView")

.Where(v => (string)v.Attribute("name") == ViewName);
            foreach (XElement el in view)
                Choosen = el;
            TreeViewItem treeNode = new TreeViewItem
            {
                //Should be Root
                Header = ViewName,
                IsExpanded = true
            };
            treeView.Items.Add(treeNode);
            BuildNodes(treeNode, Choosen);
        }

        public static void BuildNodes(TreeViewItem treeNode, XElement element)
        {
            foreach (XNode child in element.Nodes())
            {
                switch (child.NodeType)
                {
                    case XmlNodeType.Element:
                        XElement childElement = child as XElement;
                        TreeViewItem childTreeNode = new TreeViewItem
                        {
                            //Get First attribute where it is equal to value
                            Header = childElement.Attributes().First(s => s.Name == "name").Value,
                            Tag = childElement.Attributes().First(s => s.Name == "osLocation").Value,
                            //Automatically expand elements
                            IsExpanded = true
                        };
                        treeNode.Items.Add(childTreeNode);
                        BuildNodes(childTreeNode, childElement);
                        break;
                    case XmlNodeType.Text:
                        XText childText = child as XText;
                        treeNode.Items.Add(new TreeViewItem { Header = childText.Value, });
                        break;
                }
            }
        }

        public static XElement processTree(TreeViewItem treeItems, XElement parent)
        {
            XElement itemToAdd = null;

            List<TreeViewItem> children = GetChildren(treeItems);
            for (int i = 0; i < children.Count; i++)
            {

                TreeViewItem node = children[i];

                if (node.Header != null)
                {
                    FileAttributes attr = FileAttributes.Directory; //default
                    if (node.Tag.ToString() != "Custom Folder") //item dropped on is not custom folder
                        attr = File.GetAttributes(node.Tag.ToString());

                    if (attr.HasFlag(FileAttributes.Directory))
                        itemToAdd = new XElement("folder");
                    else
                        itemToAdd = new XElement("file");


                    itemToAdd.Add(new XAttribute("name", node.Header));
                    itemToAdd.Add(new XAttribute("osLocation", node.Tag));
                    // add other node properties to serialize here  
                    if (node.Items.Count > 0)
                    {


                        parent.Add(processTree(node, itemToAdd));
                    }
                    else
                    {

                        parent.Add(itemToAdd);
                    }
                }


            }
            return parent;

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



        public static void AddViewNode(List<List<string>> list, string tag)
        {
            // List<string> parsedTags = parse_tags(tag);
            XElement viewNode = new XElement("view");
            viewNode.Add(new XAttribute("name", tag));

            //add value for it

            foreach (List<string> l in list)
            {
                XElement CategoryNode = new XElement("category");
                CategoryNode.Add(new XAttribute("value", l[0] + "." + l[1]));
                for (int i = 2; i < l.Count; i++)
                {

                    XElement PathNode = new XElement("path");
                    PathNode.Add(new XAttribute("name", Path.GetFileName(l[i])));
                    PathNode.Add(new XAttribute("osLocation", l[i]));
                    CategoryNode.Add(PathNode);
                }
                viewNode.Add(CategoryNode);


            }


            //add value for it
            XElement XMLBody = viewsXMLfunc.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLBody.Elements("view")

.Where(v => (string)v.Attribute("name") == tag);
            foreach (XElement v in isViewExist)
            {
                if (XNode.DeepEquals(v, viewNode))
                    return;
            }

            XMLBody.Add(viewNode);
            viewsXMLfunc.viewDoc.Save(viewsXMLfunc.viewFilePath);


        }


        private static bool CheckFileExsists(string path)  //if the xml exsist
        {
            return File.Exists(path);
        }



        private static XDocument LoadFile(string filePath) //load the xml file
        {
            return XDocument.Load(filePath);
        }
        static public void init()
        {

            if (!CheckFileExsists(viewFilePath))
                new XDocument(
                     new XElement("root")).Save(viewFilePath);
            viewsXMLfunc.viewDoc = LoadFile(viewFilePath);



        }

    }
}