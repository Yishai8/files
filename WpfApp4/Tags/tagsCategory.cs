﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.Tags
{
    class tagsCategory
    {        
        List<string> categoryOptions; //single category
        public tagsCategory(string name, List<string> options)
        {
            this.categoryName = name;
            this.categoryOptions = options;
        }
        private string categoryName { get; set; }
        }

    }

