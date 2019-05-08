using Caliburn.Micro;
using Client.Models;

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class MyViewModel : Caliburn.Micro.Screen  // uses screen to notify property changes
    {
 

        #region dirmodel region properties
        public string Path { get; set; } //path
        public DirEnumModel Type { get; set; } //type
        public bool Expandable { get { return this.Type != DirEnumModel.File; } } //is able to expand only if not file type
        public bool IsExpanded { get { return this.Children?.Count(f => f != null) > 0; }//enumarable f goes through the list and counts children if its expanded >0 if not set it to expand if it got a child it wont show cuz it check if its not null
                                 set { if (value == true)
                    Expand();
                    else this.ClearChildren();  }  //if we get from the view - value to expand = true, to close value=false
                      
                  
                    
        } 
        public BindableCollection<MyViewModel> Children { get; set; } // expanded directories inside this item
        public string Name //name
        {
            get
            {
                return Type == DirEnumModel.Drive ? this.Path : DirStructureModel.GetDirName(this.Path);
            }
        }
        #endregion

        #region dirmodel region methods
        private void Expand()   //expands the directory and create sub directories 
        {
            if (this.Type == DirEnumModel.File) //cant expand file
            {
                return;
            }
            else { 
            List<DirModel> children = DirStructureModel.GetDirContent(this.Path); //get the directory sub folders/files by its path
            this.Children = new BindableCollection<MyViewModel>(
                children.Select(content => new MyViewModel(content.FullPath, content.Type)));  //create sub folders 
                NotifyOfPropertyChange(() => Children);
            }

        }

        private void ClearChildren() //remove children from the list and add dummy item to show expand icon in case needed
        {
            this.Children = new BindableCollection<MyViewModel>();
            if (this.Type != DirEnumModel.File)
                this.Children.Add(null); //show expand option if its not a file
        }

        #endregion

        #region commands
        public ICommand ExpandCommand { get; set; } //command to expand this item

        #endregion

        #region constructor

        public MyViewModel(string path, DirEnumModel type) //get the path and type of this item
        {
            this.ExpandCommand = new RelayCommand(Expand); //create commands - that we combine to the ui that calls the expand() function
            this.Path = path;
            this.Type = type;
            this.ClearChildren(); //setup the children
        }
    
        #endregion


       



    }
}
