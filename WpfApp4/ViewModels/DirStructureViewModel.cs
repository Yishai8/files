using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using System.Windows;
using Client.Tags;
using System.Xml.Linq;

namespace Client.ViewModels //view model for the main directory view
{

    #region properties
    public class DirStructureViewModel : Screen
    {
        private BindableCollection<OutlookModel> _outlookList = new BindableCollection<OutlookModel>();  //list of all the views ***should be fetched from database
        private OutlookModel _selectedOutlook;  //selected view
        /*private BindableCollection<PairModel> _tagPicker = new BindableCollection<PairModel>(); //xml fetch strings*/
        private tagsCategory _selectedCategory;  //selected tag
        private string _selectedOption;
        private string _currentTag;
        public string CurrentTags
        {
            get
            {
                return _currentTag;
            }

            set
            {
                _currentTag = value;
                NotifyOfPropertyChange(() => CurrentTags); //updates the value in the code to match the view
            }
        }



   
  

        public BindableCollection<MyViewModel> Items { get; set; } //list of all directories each item of the treeviewmodel is of the myviewmodel type

        public string getTagsOutput(MyViewModel sender)
        {
            return sender.ToString();
        }



        public BindableCollection<tagsCategory> Categories { get; private set; } = new BindableCollection<tagsCategory>();   //tag options 
        public BindableCollection<string> Options { get; private set; } = new BindableCollection<string>();   //tag options 
        public BindableCollection<DirModel> TreeViewFiles { get;  set; } = new BindableCollection<DirModel>();   //tag options 

        #endregion

        #region constructor
        //create a new viewmodel from data
        public DirStructureViewModel()
        {
            Tags.XMLFile.init();
            var children = DirStructureModel.GetLogicalDrives(); //get the logical drives
            this.Items = new BindableCollection<MyViewModel>(children.Select(drive => new MyViewModel(drive.FullPath, DirEnumModel.Drive)));
            this.Categories=DirStructureModel.LoadCategoriesListFromXML(); //get xml tag options - fetched from xml "tagCategories"
            

           /* TagPicker.Add(new PairModel { PairName = "animal", PairValue = "dog" });   
            TagPicker.Add(new PairModel { PairName = "food", PairValue = "burger" });
           */

            OutlookList.Add(new OutlookModel { OutlookName = "tag:animal", OutlookId = 1 });  //***list test
            OutlookList.Add(new OutlookModel { OutlookName = "tag:hobbies", OutlookId = 3 });
            OutlookList.Add(new OutlookModel { OutlookName = "tag:place", OutlookId = 5 });
        }

        #endregion

        #region methods
        /*
                /// <summary>
                /// Tagging file using text block
                /// </summary>
                /// <param name="selectedItem"></param>
                public void saveTags(MyViewModel selectedItem)
                {
                    string selectedItemPath = selectedItem.Path.ToString();
                    MessageBox.Show(selectedItemPath + $" tagged with :{TagsOutput}" );

                    if (string.Compare(TagsOutput, string.Empty) != 0)
                        Tags.TagManagment.saveFileTags(selectedItemPath, TagsOutput);  //needs the selected item path
                    else
                        Tags.TagManagment.DeleteFileTags(selectedItemPath);
                }
        */

        /// <summary>
        /// Tagging file using combobox and xml data
        /// </summary>
        /// <param name="selectedItem"></param>
        public void saveTags(MyViewModel selectedItem)
        {
            string selectedItemPath = selectedItem.Path.ToString();
            string tagText = this._selectedCategory.CategoryName +":"+ this._selectedOption;
            MessageBox.Show(selectedItemPath + $" tagged with :{tagText}");

            if (string.Compare(tagText, string.Empty) != 0 )
                Tags.TagManagment.saveFileTags(selectedItemPath, tagText);  //needs the selected item path
            else
                Tags.TagManagment.DeleteFileTags(selectedItemPath);
        }

        public void removeTags(MyViewModel selectedItem)
        {
            TagManagment.DeleteFileTags(selectedItem.Path);
            NotifyOfPropertyChange(() => CurrentTags);
            MessageBox.Show($" tag removed from { selectedItem.Name}");
        }


        public void ThumbnailsOpenFile(MyViewModel selectedItem)    //open file from tree view
        {
            if (selectedItem!=null && selectedItem.Type == DirEnumModel.File)
            {
                var isFile = new Uri(selectedItem.Path).AbsolutePath.Split('/').Last().Contains('.');
                if (isFile)
                    System.Diagnostics.Process.Start(selectedItem.Path);
            }
            else return;
        }

        public void FetchTags(MyViewModel selectedItem)    //get the items tags
        {
            //MessageBox.Show($"{CurrentTags}");
            this.CurrentTags=TagManagment.getFileTag(selectedItem.Path);
            NotifyOfPropertyChange(() => CurrentTags);


        }



        #endregion



        public void showTreeViewItem(object sender)
        {
            MessageBox.Show($"this is the item: {sender}");
        }


        #region view categroy selection properties
        public BindableCollection<OutlookModel> OutlookList //mvvm list includes notification changes attributes
        {
            get { return _outlookList; }
            set { _outlookList = value; }
        }

        /*public BindableCollection<PairModel> TagPicker
        {
            get { return _tagPicker; }
            set { _tagPicker = value; }
        }*/

        public OutlookModel SelectedOutlook
        {
            get { return _selectedOutlook; }
            set
            {
                _selectedOutlook = value;
                NotifyOfPropertyChange(() => SelectedOutlook); //updates the value in the code to match the view
            }
        }

        public tagsCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                NotifyOfPropertyChange(() => _selectedCategory); //updates the value in the code to match the view
                this.Options = _selectedCategory.CategoryOptions;
                NotifyOfPropertyChange(() => Options);
            }
        }

        public string SelectedOptions
        {
            get { return _selectedOption; }
            set
            {
                _selectedOption = value;
                NotifyOfPropertyChange(() => _selectedOption); //updates the value in the code to match the view
            }
        }

 
        #endregion
    }
}
