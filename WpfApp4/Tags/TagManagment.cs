using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4.Tags
{
//tags management class - centrelized place for all tags related classes
    static class TagManagment
    {
        public static string getFileTag(string filepath)
        {
            Tag newTag = new Tag(filepath);
            return newTag.getFileTag();
        }

        public static void saveFileTags(string filepath,string tags)
        {
            Tag newTag = new Tag(filepath);
            newTag.saveFileTags(tags);
        }

        public static void DeleteFileTags(string filepath)
        {
            Tag newTag = new Tag(filepath);
            newTag.DeleteFileTags();
        }
    }
}
