using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

//handle tags xml file
namespace WpfApp4.Tags
{
    static class tagsXMLfunc
    {
        static readonly string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
        static XDocument tagDoc = null;    //load the xml file to object

        //compare tags block to decide if changes are needed
        public static bool checkDifferenceInNodes(XElement newElement, IEnumerable<XElement> isViewExist)
        {
            foreach (XElement el in isViewExist)
            {
                if (XNode.DeepEquals(el, newElement))
                    return false;
            }
            return true;
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



        //new tag node by main/sub category
        public static void AddTagNode(string tag, string path)
        {
            List<string> parsedTags = parse_tags(tag);
           
                string mainCat =parsedTags[0] ;
                string subCat = string.Empty;
            if(parsedTags.Count>1)
                 subCat = parsedTags[1];
            IEnumerable<XElement> categoriesToAdd = null;
            if (subCat!=string.Empty)
                 categoriesToAdd = getNodeByTag(mainCat+"."+subCat, true);
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

        //tags filtering
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
                from el in tagsXMLfunc.tagDoc.Descendants("root").Elements("tag")
                where ((string)el.Attribute("name") == mainCat && (string)el.Attribute("value") == subCat)
                select el;
                
                
            }
            else
            {
                NodeList = //bring all the tag block from the xml includes subtags
                from el in tagsXMLfunc.tagDoc.Descendants("root").Elements("tag")
                where (string)el.Attribute("value") == subCat
                select el;
            }
            return NodeList;
        }

        //view by tags
        public static IEnumerable<XElement> getNodesForView(string tag, string iscategory)
        {
            IEnumerable<XElement> NodeList=null;
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
                    NodeList = //bring all the tag block from the xml includes subtags
               from el in tagsXMLfunc.tagDoc.Descendants("root").Elements("tag")
               where ((string)el.Attribute("name") == mainCat && (string)el.Attribute("value") == subCat)
               select el;
                    break;

                case "Main Category":
                    NodeList = //bring all the tag block from the xml includes subtags
               from el in tagsXMLfunc.tagDoc.Descendants("root").Elements("tag")
               where ((string)el.Attribute("name") == mainCat)
               select el;
                    break;

                case "SubCategoy":
                    NodeList = //bring all the tag block from the xml includes subtags
                from el in tagsXMLfunc.tagDoc.Descendants("root").Elements("tag")
                where (string)el.Attribute("value") == subCat
                select el;
                    break;
            }
          
            
            return NodeList;
        }

        //paths list by tag
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
                tagsXMLfunc.tagDoc.Save(tagsXMLfunc.docFilePath);
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
            XElement XMLBody = tagsXMLfunc.tagDoc.Element("root");
            XMLBody.Add(tagNode);
            tagsXMLfunc.tagDoc.Save(tagsXMLfunc.docFilePath);
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
            tagsXMLfunc.tagDoc = LoadFile(docFilePath);
           



        }

    }
}
