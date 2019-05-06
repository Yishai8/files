using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class TagModel  //object with tag details
    {
        public string FileName { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public string FileDescription { get; set; }

      /*  public string FullDetails
        {
            get
            {
                return $"{FileName}, {FileDescription}";
            }
        } */
    }
}
