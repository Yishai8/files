using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Tags
{
    static class XMLFile
    {
        static readonly string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
        static readonly string viewFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Views.xml";//create xml on desktop
        static XDocument tagDoc = null;    //load the xml file to object
        static XDocument viewDoc = null;    //load the xml file to object

        public static void AddViewNode(List<List<string>> list,string tag)
        {
            // List<string> parsedTags = parse_tags(tag);
            XElement viewNode = new XElement("view");
            viewNode.Add(new XAttribute("name", tag));
            
            //add value for it
          
            foreach (List<string> l in list)
            {
                XElement CategoryNode = new XElement("category");
                CategoryNode.Add(new XAttribute("value", l[0] + "." + l[1]));
                for(int i=2;i<l.Count;i++)
                {
                    XElement PathNode = new XElement("path");
                    PathNode.Add(new XAttribute("value", l[i]));
                    CategoryNode.Add(PathNode);
                }
                viewNode.Add(CategoryNode);


            }

           
            //add value for it
            XElement XMLBody = XMLFile.viewDoc.Element("root");
            IEnumerable<XElement> isViewExist = XMLBody.Elements("view")

.Where(v => (string)v.Attribute("name") == tag);
            foreach(XElement v in isViewExist)
            {
                if (XNode.DeepEquals(v, viewNode))
                    return;
            }
            
                XMLBody.Add(viewNode);
                XMLFile.viewDoc.Save(XMLFile.viewFilePath); ;
            
           
        }


        public static void AddTagNode(string tag, string path)
        {
            List<string> parsedTags = parse_tags(tag);
            foreach (string tagRes in parsedTags)
            {
                var i = tagRes.IndexOf('.');
                string mainCat = tagRes.Substring(0, i); ;
                string subCat = tagRes.Substring(i + 1, (tagRes.Length) - i - 1);

                IEnumerable<XElement> categoriesToAdd = getNodeByTag(tagRes, false);
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
        }

        public static IEnumerable<XElement> getNodeByTag(string tag, bool getBySubCat)
        {
            IEnumerable<XElement> NodeList;
            string mainCat = null ;
            int i = -1;
            i=tag.IndexOf('.');
            if(i!=-1)
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



        public static List<List<string>> getPathsByTag(string tag, bool getBySubCat)
        {
            List<List<string>> pathList = new List<List<string>>();

            IEnumerable<XElement> listOftags = getNodeByTag(tag, getBySubCat);
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
            parsedTags = tags.Split(';').Select(t => t.Trim()).ToList();
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
