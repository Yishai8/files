using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Tags
{
    class TagFilter
    {
        public string FileTag { get; set; } //list of tags name and value sport.ski
        public string path { get; set; }  //file path
        public TagFilter(string _pathName,string _fileTag)
        {
            path = _pathName;
            FileTag = _fileTag;
        }
    }
}
