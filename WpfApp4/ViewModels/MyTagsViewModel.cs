using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Models;

namespace Client.ViewModels
{

    class MyTagsViewModel : Caliburn.Micro.Screen  // uses screen to notify property changes
    {
        public BindableCollection<TagModel> TagQue { get; set; }  //list of all the files waiting to tag should be fetched from drag and drop

        

        public MyTagsViewModel()
        {
            filltagque(10);
        }

       

        public BindableCollection<TagModel> filltagque(int total)       //test function to add fileque items should be fetched from drag and drop
        {
            TagQue = new BindableCollection<TagModel>();

            for (int i = 0; i < total; i++)
            {

                TagQue.Add(GetFileToTag());
            }

            return TagQue;
        }

         private TagModel GetFileToTag() //test function to add each object in the list of the fileque items should be fetched from drag and drop
        {
            TagModel fileTag = new TagModel();
            fileTag.FileName = "this is name";
            fileTag.FileDescription = "this is description";
            fileTag.Tags.Add("this is tag 1");
            fileTag.Tags.Add("this is tag 2");
            return fileTag;
        }

    

    }
}

 

