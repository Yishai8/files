using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Tags
{
    public class tagsCategory
    {        
        public IList<string> categoryOptions { get; set; } //single category
        public tagsCategory(string name, List<string> options)
        {
            this.categoryName = name;
            this.categoryOptions = options;
        }
        public string categoryName { get; set; }
        }

    }

