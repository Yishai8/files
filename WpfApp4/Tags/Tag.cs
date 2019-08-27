using CodeFluent.Runtime.BinaryServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp4.Tags
{
    class Tag //each file is a tag
    {
        
        List<string> FileTag { get; set; } //list of tags name and value sport.ski
        string _path;  //file path
        public Tag(string pathName)
        {
            _path = pathName;
        }
       /* public void setFileTag(string path, string tag)  //get path and text tag
        {
            //FileTag.Add(tag);
            
            // path is a file.
            if (File.Exists(path))

            {

                string filePath = System.IO.Path.GetFullPath(path);
                var file = ShellFile.FromFilePath(filePath);

                //file.Properties.System.Size.Value = 123;


            }

        }*/

        private bool checkTagFileExists(string path)
        {
            string streamName = ":fileTags"; //ads needs file name : stream name
            return NtfsAlternateStream.Exists(path + streamName);
        }

        //get tags file content
        private string getFileStream(string fileName)   //brings the stream the ads read the ads of the file
        {
            if (checkTagFileExists(fileName))  //if there is a tag
            {

                string streamName = ":fileTags";
                FileStream stream = NtfsAlternateStream.Open(fileName + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None); //open the ads
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileName); //bring all the ads the file has whats in the stream
                foreach (NtfsAlternateStream ads in fileStream) //enter the filestream
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))             //type of ads

                        {
                           return Regex.Replace(NtfsAlternateStream.ReadAllText(fileName + streamName),"\n|\r", "");
                        }
                }
            }

            return "The file doesn't have tags";



        }


        public void saveFileTags(string tags)   //write text to ads
        {
            string streamName = ":fileTags";

                FileStream stream = NtfsAlternateStream.Open(this._path + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(this._path);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                            string currentTags = getFileTag();
                        List<string> list = currentTags.Split(';').ToList();
                        if(!list.Contains(tags)) //check if tag exists already

                        NtfsAlternateStream.WriteAllText(this._path + streamName, currentTags+";"+tags);
                            
                        }
                }
            

        }

        public void DeleteFileTags() //delete all ads from file
        {
            string streamName = ":fileTags";
            if (checkTagFileExists(this._path))
            {


                FileStream stream = NtfsAlternateStream.Open(this._path + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(this._path);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                            NtfsAlternateStream.Delete(this._path + streamName);

                        }
                }
            }

        }
        public string getFileTag()
        {
            //Create a stream supporting ADS syntax
            string fileName = this._path;
            
                string fileTags = getFileStream(fileName);  //brings the tags by the filename
            
                return fileTags;
            
           
        }

        }
}
