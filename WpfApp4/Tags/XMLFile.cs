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
        private static void AddTagNode(string tag,string path)
        {
            IEnumerable<XElement> address = //bring all the tag block from the xml includes subtags
            from el in XMLFile.doc.Descendants("root").Elements("tag")
            where (string)el.Attribute("value") == "fefdf"
             select el;
            foreach (XElement el in address.Descendants())  //paths
                Console.WriteLine(el.Value);

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
