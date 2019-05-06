using Caliburn.Micro;
using Client.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Client.Models
{
    public static class DirStructureModel  ///<summary>get all the directory data list of drives, list of folders and files</summary>
    {

        #region windows default tree functions
        public static List<DirModel> GetLogicalDrives() ///<summary>get every logical drive as a list</summary>
        {
           return Directory.GetLogicalDrives().Select(drive => new DirModel { FullPath = drive, Type = DirEnumModel.Drive }).ToList();
           
        }  

        public static List<DirModel> GetDirContent(string path)     ///<summary>get directories-folders top level content and files as a list</summary>
        {
            var items = new List<DirModel>();

            try
            {
                var dirs = Directory.GetDirectories(path); //get folders

                if (dirs.Length > 0)
                {
                    items.AddRange(dirs.Select(dir => new DirModel { FullPath = dir, Type = DirEnumModel.Folder }));
                }
            }
            catch { }

            try
            {
                var fs = Directory.GetFiles(path);  //get files from the folders
                if (fs.Length > 0)
                {
                    items.AddRange(fs.Select(file => new DirModel { FullPath = file, Type = DirEnumModel.File })); 
                }
            }
            catch { }
            return items;
        }

        public static string GetDirName(string path)  ///<summary>find the dir name from the path</summary>
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
           
                var normalizePath = path.Replace('/', '\\');
                var lastIndex = normalizePath.LastIndexOf('\\'); // find the last backlslash in the path

                if (lastIndex <= 0) return path; //if we cant find backslash return the path
                return path.Substring(lastIndex + 1);
            
        }

        #endregion

        #region virtual tree functions

        public static List<DirModel> GetVirtualDrives() ///<summary>get every logical drive as a list</summary>
        {
            return Directory.GetLogicalDrives().Select(drive => new DirModel { FullPath = drive, Type = DirEnumModel.Drive }).ToList();

        } 

        #endregion


        /// <summary>
        /// load xml tags list
        /// </summary>
        /// <param name="sender"></param>
        /// 
        public static BindableCollection<tagsCategory> LoadCategoriesListFromXML()
        {
            BindableCollection<tagsCategory> categories = new BindableCollection<tagsCategory>(); //collection of categories
            XDocument doc = XDocument.Load(@"..\..\Tags\tagCategories.xml"); //should be changed to relative path

            string header;
            IEnumerable<XElement> listOfcategories = //bring all the categories and sub categories from XML
            from el in doc.Descendants("root").Elements("HeaderTag")
            select el;
            foreach (XElement el in listOfcategories)  //paths
            {
                BindableCollection<string> cat = new BindableCollection<string>();  //list of the sub categories
                header = (string)el.Attribute("name").Value;

                foreach (XElement child in el.Descendants())
                {

                    cat.Add((string)child.Attribute("name").Value);
                }
                categories.Add(new tagsCategory(header, cat));

            }
            return categories;

        }
    }  
}
