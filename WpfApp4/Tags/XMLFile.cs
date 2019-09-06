using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

using System.Windows;

namespace WpfApp4.Tags
{
    static class XMLFile
    {

        static string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
        static readonly string viewFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Views.xml";//create xml on desktop
        static XDocument tagDoc = null;    //load the xml file to object


        static XDocument viewDoc = null;    //load the xml file to object


        public static string SaveView(ItemCollection treeItems, string viewName, bool replace)
        {
            XElement XMLElements = XMLFile.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLElements.Elements("CustomView")

            .Where(v => (string)v.Attribute("name") == viewName);
            if (!replace && isViewExist.Any())
                return "name already exists";

            XElement CustomViewNode = new XElement("CustomView");
            CustomViewNode.Add(new XAttribute("name", viewName));

            //add value for it

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
            XElement XMLBody = XMLFile.viewDoc.Element("root");
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
            XMLFile.viewDoc.Save(XMLFile.viewFilePath);
            return "success";

        }

        public static void deleteView(string viewName)
        {
            XElement XMLElements = XMLFile.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLElements.Elements("CustomView")

            .Where(v => (string)v.Attribute("name") == viewName);
            foreach (XElement el in isViewExist)
            {
                el.Remove();
            }


            XMLFile.viewDoc.Save(XMLFile.viewFilePath);
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

        public static string replaceCustomView(ItemCollection treeItems, string viewName)
        {
            return "x";
        }

        public static List<string> getViewList()
        {
            List<string> ListOfViews = new List<string>();
            var XMLElements = XMLFile.viewDoc.Element("root").Descendants("CustomView").Attributes("name").Select(x => x.Value);
            foreach (var el in XMLElements)
                ListOfViews.Add(el);
            return ListOfViews;

        }

        public static void BuildTree(TreeView treeView, string ViewName)
        {
            XElement Choosen = null;
            IEnumerable<XElement> view = XMLFile.viewDoc.Element("root").Elements("CustomView")

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
            XElement XMLBody = XMLFile.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLBody.Elements("view")

            .Where(v => (string)v.Attribute("name") == tag);
            foreach (XElement v in isViewExist)
            {
                if (XNode.DeepEquals(v, viewNode))
                    return;
            }

            XMLBody.Add(viewNode);
            XMLFile.viewDoc.Save(XMLFile.viewFilePath);


        }

        public static void AddTagNode(string tag, string path)
        {
            string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
            XDocument xmlDocument = XDocument.Load(docFilePath);

            XDocument doc;

            doc = XDocument.Load(docFilePath);
            XMLFile.tagDoc = LoadFile(docFilePath);

            List<string> parsedTags = parse_tags(tag);

            string mainCat = parsedTags[0];
            string subCat = string.Empty;
            if (parsedTags.Count > 1)
                subCat = parsedTags[1];
            IEnumerable<XElement> categoriesToAdd = null;
            if (subCat != string.Empty)
                categoriesToAdd = getNodeByTag(mainCat + "." + subCat, true);
            else
                categoriesToAdd = getNodeByTag(mainCat + ".", false);
            if (categoriesToAdd.Any())
            {
                foreach (XElement el in categoriesToAdd)  //paths
                {

                    addPathToExistingNode(el, path);

                }
            }

            else
                addPathToNewNode(mainCat, subCat, path);


        }

        public static IEnumerable<XElement> getNodeByTag(string tag, bool getBySubCat)
        {
            IEnumerable<XElement> NodeList;
            string mainCat = null;
            int i = -1;
            i = tag.IndexOf('.');
            if (i != -1)
                mainCat = tag.Substring(0, i); ;
            string subCat = tag.Substring(i + 1, (tag.Length) - i - 1);
            if (!getBySubCat)
            {

                NodeList = //bring all the tag block from the xml includes subtags
            from el in XMLFile.tagDoc.Descendants("root").Elements("tag")
            where ((string)el.Attribute("name") == mainCat && (string)el.Attribute("value") == subCat)
            select el;


            }
            else
            {
                NodeList = //bring all the tag block from the xml includes subtags
                from el in XMLFile.tagDoc.Descendants("root").Elements("tag")
                where (string)el.Attribute("value") == subCat
                select el;

            }
            return NodeList;
        }

        public static IEnumerable<XElement> getNodesForView(string tag, string iscategory)
        {
            string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
            XDocument xmlDocument = XDocument.Load(docFilePath);

            XDocument doc;

            doc = XDocument.Load(docFilePath);
            IEnumerable<XElement> NodeList = null;
            string mainCat = null;
            int i = -1;
            i = tag.IndexOf('.');
            if (i != -1)
                mainCat = tag.Substring(0, i); ;
            string subCat = tag.Substring(i + 1, (tag.Length) - i - 1);
            if (subCat == string.Empty)
                iscategory = "Main Category";
            switch (iscategory)
            {
                case "Main+SubCategory":
                    NodeList =
                        from el in doc.Element("root").Elements("tag")
                        where ((string)el.Attribute("name") == mainCat && (string)el.Attribute("value") == subCat)
                        select el;
                    break;

                case "Main Category":
                    NodeList = //bring all the tag block from the xml includes subtags
                    from el in XMLFile.tagDoc.Descendants("root").Elements("tag")
                    where ((string)el.Attribute("name") == mainCat)
                    select el;
                    break;

                case "SubCategoy":
                    NodeList = //bring all the tag block from the xml includes subtags
                    from el in XMLFile.tagDoc.Descendants("root").Elements("tag")
                    where (string)el.Attribute("value") == subCat
                    select el;
                    break;
            }

            return NodeList;
        }


        public static List<List<string>> getPathsByTag(string tag, string isCategory)
        {
            List<List<string>> pathList = new List<List<string>>();

            IEnumerable<XElement> listOftags = getNodesForView(tag, isCategory);
            foreach (XElement tagNode in listOftags)
            {
                List<string> innerPathList = new List<string>();
                innerPathList.Add(tagNode.Attribute("name").Value);
                innerPathList.Add(tagNode.Attribute("value").Value);
                foreach (XElement pathNode in tagNode.Descendants())
                {
                    innerPathList.Add(pathNode.Attribute("value").Value);
                }
                pathList.Add(innerPathList);

            }


            return pathList;
        }


        //tag+subtag exist
        private static void addPathToExistingNode(XElement el, string path)
        {

            string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
            XDocument xmlDocument = XDocument.Load(docFilePath);

            XDocument doc = xmlDocument;

            var isPathExistInNode = from p in el.Elements("path")
                                    where (string)p.Attribute("value") == path
                                    select p;

            //path doesn't exist in node
            if (!isPathExistInNode.Any())
            {
                XElement pathNode = new XElement("path");
                pathNode.Add(new XAttribute("value", path));
                //add value for it
                el.Add(pathNode);
                XMLFile.tagDoc.Save(XMLFile.docFilePath);
            }
        }

        //tag+subtag doesn't exist
        private static void addPathToNewNode(string mainCat, string subCat, string path)
        {
            XElement tagNode = new XElement("tag");
            tagNode.Add(new XAttribute("name", mainCat));
            tagNode.Add(new XAttribute("value", subCat));
            XElement pathNode = new XElement("path");
            pathNode.Add(new XAttribute("value", path));
            //add value for it
            tagNode.Add(pathNode);
            //add value for it
            XElement XMLBody = XMLFile.tagDoc.Element("root");
            XMLBody.Add(tagNode);
            XMLFile.tagDoc.Save(XMLFile.docFilePath);
        }
        //get all categories from tags parsed by delimiter (;)
        public static List<string> parse_tags(string tags)
        {
            List<string> parsedTags = new List<string>();
            parsedTags = tags.Split('.').Select(t => t.Trim()).ToList();
            return parsedTags;
        }

        private static void CheckRemovedTag()   //needs to remove or change tag
        {

        }

        private static bool CheckFileExsists(string path)  //if the xml exsist
        {
            return File.Exists(path);
        }

        private static void CreateTagFile() //create nodes to the xml which creates tag
        {

        }

        private static XDocument LoadFile(string filePath) //load the xml file
        {
            return XDocument.Load(filePath);
        }
        static public void init()
        {
            if (!CheckFileExsists(docFilePath))
                new XDocument(
                     new XElement("root")).Save(docFilePath);
            if (!CheckFileExsists(viewFilePath))
                new XDocument(
                     new XElement("root")).Save(viewFilePath);
            XMLFile.tagDoc = LoadFile(docFilePath);
            XMLFile.viewDoc = LoadFile(viewFilePath);



        }

    }
}
