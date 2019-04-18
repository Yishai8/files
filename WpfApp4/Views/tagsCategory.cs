using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Views
{
    class tagsCategory
    {
        
        List<string> categoryOptions; //single category



        private string categoryName { get; set; }

        public void LoadCategoryListFromXML()
        {
            ObservableCollection<tagsCategory> _Categories = new ObservableCollection<tagsCategory>(); //collection of categories
            XDocument doc = XDocument.Load(@"Views\tagCategories.xml");
            List<string> cat = new List<string>();
            string header;
            IEnumerable<XElement> listOfcategories = //bring all the categories and sub categories from XML
            from el in doc.Descendants("root").Elements("HeaderTag")
            select el;
            foreach (XElement el in listOfcategories)  //paths
            {
               
               header=(string)el.Attribute("name").Value;

                foreach (XElement child in el.Descendants())
                {

                    cat.Add((string)child.Attribute("name").Value);
                }
                _Categories.Add(new tagsCategory { categoryName = header, categoryOptions = cat });

            }

        }


    }
}
