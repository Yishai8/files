using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class MainViewModel : Conductor<object> //one child at a time(item) - when we ask for a child creates the child and puts it in a view, for multiple active at same time use multiple
    {


        public MainViewModel()
        {
        }


        public void LoadMyViews()
        {
            ActivateItem(new DirStructureViewModel());  //*** switch page to testview 
        }

        public void LoadMyTags()
        {
            ActivateItem(new MyTagsViewModel());  //*** switch page to tagsview 
        }

        public void LoadMyTaggedFiles()
        {
            ActivateItem(new MyTaggedFilesViewModel());  //*** switch page to tagsview 
        }

        public void Save()
        {

            MessageBox.Show("dsgdsg");
        }

        public void Save(object sender, object sender2)
        {

            MessageBox.Show($"{ sender}", $"{sender2}");
        }

        public void Wee(object sender)
        {

            MessageBox.Show($"{sender}");

            NotifyOfPropertyChange(() => sender); //updates the value in the code to match the view   
        }

    }
}
