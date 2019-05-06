using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client.Tags
{
    public class tagsCategory
    {        
        public BindableCollection<string> CategoryOptions; //single category
        public string CategoryName { get; set; }
        public bool isChecked;
        

        public tagsCategory(string name, BindableCollection<string> options)
        {
            this.CategoryName = name;
            this.CategoryOptions = options;
        }    
    }
}

