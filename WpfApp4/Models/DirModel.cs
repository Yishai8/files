using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class DirModel
    {
        public DirEnumModel Type {get;set; }   //directory type - folder/file/drive

        public string FullPath { get; set; }  //path

        public string Name { get { return this.Type == DirEnumModel.Drive ? this.FullPath : DirStructureModel.GetDirName(this.FullPath);  } } //name of this directory

    }
}
