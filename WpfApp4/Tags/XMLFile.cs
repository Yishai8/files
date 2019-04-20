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
        static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";  //create xml on desktop
        static XDocument doc = null;    //load the xml file to object
        public static void AddTagNode(string tag, string path)
        {
            List<string> parsedTags = parse_tags(tag);
            foreach (string tagRes in parsedTags)
            {
                int nodesCount = 0;
                var i = tagRes.IndexOf('.');     
                string mainCat = tagRes.Substring(0, i); ;
                string subCat = tagRes.Substring(i+1, (tagRes.Length)-i-1);
                IEnumerable<XElement> categoriesToAdd = //bring all the tag block from the xml includes subtags
                from el in XMLFile.doc.Descendants("root").Elements("tag")
                where ((string)el.Attribute("Name") == mainCat)
                select el;

                foreach (XElement el in categoriesToAdd.Descendants())  //paths
                    nodesCount++;
                if(nodesCount>0)
                {

                }

            }
        }

        //get all categories from tags parsed by delimiter (;)
        public static List<string> parse_tags(string tags)
        {
            List<string> parsedTags = new List<string>();
            parsedTags = tags.Split(';').ToList();
            return parsedTags;
        }

        private static void CheckRemovedTag()   //needs to remove or change tag
        {

        }

        private static bool CheckFileExsists()  //if the xml exsist
        {
            return File.Exists(filePath);
        }

        private static void CreateTagFile() //create nodes to the xml which creates tag
        {

        }

        private static XDocument LoadFile() //load the xml file
        {
            return XDocument.Load(filePath);
        }
        static public void init()
        {
            if (!CheckFileExsists())
                new XDocument(
                     new XElement("root")).Save(filePath);
            XMLFile.doc = LoadFile();

            var newDoc = new XDocument(new XElement("root",
              from p in XMLFile.doc.Element("root").Elements("tag")
              select p));

        }

    }
}
