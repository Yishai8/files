﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client.Tags
{
    //tags management class - centrelized place for all tags related classes
    public static class TagManagment
    {
         public static string getFileTag(string filepath)
        {
            Tag newTag = new Tag(filepath);
            return newTag.getFileTag();
        }

        public static void saveFileTags(string filepath, string tags)
        {
            Tag newTag = new Tag(filepath);
            newTag.saveFileTags(tags); //write tag to ads
            Tags.XMLFile.AddTagNode(tags, filepath); //write the tag in the xml


        }

        public static void DeleteFileTags(string filepath)
        {
            Tag newTag = new Tag(filepath);
            newTag.DeleteFileTags();
        }

        /*public static ObservableCollection<tagsCategory> LoadCategoriesListFromXML()
        {
            ObservableCollection<tagsCategory> _Categories = new ObservableCollection<tagsCategory>(); //collection of categories
            XDocument doc = XDocument.Load(@"Tags/tagCategories.xml");
            
            string header;
            IEnumerable<XElement> listOfcategories = //bring all the categories and sub categories from XML
            from el in doc.Descendants("root").Elements("HeaderTag")
            select el;
            foreach (XElement el in listOfcategories)  //paths
            {
                BindableCollection<string> cat = new BindableCollection<string>();
                header = (string)el.Attribute("name").Value;

                foreach (XElement child in el.Descendants())
                {

                    cat.Add((string)child.Attribute("name").Value);
                }
                _Categories.Add(new tagsCategory ( header, cat ));

            }
            return _Categories;

        }*/
    }
}
