using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Tags
{
    //tags management class - centrelized place for all tags related classes
    static class TagManagment
    {
         public static string getFileTag(string filepath)
        {
            Tag newTag = new Tag(filepath);
            return newTag.getFileTag();
        }

        public static void saveFileTags(List<string> filepaths, string tags)
        {
            foreach(string filename in filepaths)
            {
                List<string> itemsToTag = new List<string>() ;
                var isFile = new Uri(filename).AbsolutePath.Split('/').Last().Contains('.');
                if (!isFile)
                {
                    itemsToTag.AddRange(Directory.GetFiles(filename, "*.*", SearchOption.AllDirectories));
                    itemsToTag.AddRange (Directory.GetDirectories(filename, "*.*", SearchOption.AllDirectories));
                    if(!itemsToTag.Contains(filename))
                    itemsToTag.Add(filename);
                }
                    
                else
                    itemsToTag.Add(filename);
                foreach(string item in itemsToTag)
                {
                    Tag newTag = new Tag(item);

                    newTag.saveFileTags(tags);
                    Tags.XMLFile.AddTagNode(tags, filename);
                }
                

            }


        }

        public static List<List<string>> getPathsByTag(string tag,string isCategory)
        {
            return Tags.XMLFile.getPathsByTag(tag, isCategory);
        }

        public static void DeleteFileTags(string filepath)
        {
            Tag newTag = new Tag(filepath);
            newTag.DeleteFileTags();
        }

        public static ObservableCollection<tagsCategory> LoadCategoriesListFromXML()
        {
            ObservableCollection<tagsCategory> _Categories = new ObservableCollection<tagsCategory>(); //collection of categories
            XDocument doc = XDocument.Load(@"Tags/tagCategories.xml");
            
            string header;
            IEnumerable<XElement> listOfcategories = //bring all the categories and sub categories from XML
            from el in doc.Descendants("root").Elements("HeaderTag")
            select el;
            foreach (XElement el in listOfcategories)  //paths
            {
                List<string> cat = new List<string>();
                bool showOld = bool.TryParse(el.Attribute("showOldValues").Value, out showOld);
                header = (string)el.Attribute("name").Value;

                foreach (XElement child in el.Descendants())
                {

                    cat.Add((string)child.Attribute("name").Value);
                }
                _Categories.Add(new tagsCategory ( header, cat, showOld));

            }
            return _Categories;

        }
    }
}
