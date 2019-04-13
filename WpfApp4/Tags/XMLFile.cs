using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4
{
    static class XMLFile
    {
        static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
        static XDocument doc = null;
        private static void AddTagNode(string tag,string path)
        {
            IEnumerable<XElement> address =
            from el in XMLFile.doc.Descendants("root").Elements("tag")
            where (string)el.Attribute("value") == "fefdf"
             select el;
            foreach (XElement el in address.Descendants())
                Console.WriteLine(el.Value);

        }

        private static void CheckRemovedTag()
        {
            
        }

        private static bool CheckFileExsists()
        {
            return File.Exists(filePath);
        }

        private static void CreateTagFile()
        {

        }

        private static XDocument LoadFile()
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
