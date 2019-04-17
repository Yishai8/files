using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Views
{
    class SystemView
    {
        List<List<string>> listOfCat = new List<List<string>>();
        public void LoadCategoryListFromXML()
        {
            XDocument doc = XDocument.Load(@"Views\tagCategories.xml");
            IEnumerable <XElement> listOfcategories = //bring all the categories and sub categories from XML
            from el in doc.Descendants("root").Elements("HeaderTag")
            select el;
            foreach (XElement el in listOfcategories)  //paths
            {
                List<string> sublist = new List<string>();
                sublist.Add((string)el.Attribute("name").Value);
               
                foreach (XElement child in el.Descendants())
                {
                   
                    sublist.Add((string)child.Attribute("name").Value);
                }
                listOfCat.Add(sublist);

            }
         
        }
    }
    
      
}
