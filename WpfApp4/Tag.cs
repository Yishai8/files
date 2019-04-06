using CodeFluent.Runtime.BinaryServices;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp4
{
    class Tag
    {

        List<string> FileTag { get; set; }
        string _path;
        public Tag(string pathName)
        {
            _path = pathName;
        }
        public void setFileTag(string path, string tag)
        {
            //FileTag.Add(tag);

            // path is a file.
            if (File.Exists(path))

            {

                string filePath = System.IO.Path.GetFullPath(path);
                var file = ShellFile.FromFilePath(filePath);

                //file.Properties.System.Size.Value = 123;


            }

        }

        private bool checkTagFileExists(string path)
        {
            string streamName = ":fileTags";
            return NtfsAlternateStream.Exists(path + streamName);
        }

        //get tags file content
        private string getFileStream(string fileName)
        {
            if (checkTagFileExists(fileName))
            {

                string streamName = ":fileTags";
                FileStream stream = NtfsAlternateStream.Open(fileName + streamName, FileAccess.Write, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileName);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                            return Regex.Replace(NtfsAlternateStream.ReadAllText(fileName + streamName),"\n|\r", String.Empty);
                        }
                }
            }

            return "The file doesn't have tags";



        }


        private void createTagsFile(string fileName)
        {
            string streamName = ":fileTags";

        }
            public string getFileTag()
        {
            //Create a stream supporting ADS syntax
            string fileName = this._path;
            
                string fileTags = getFileStream(fileName);

                //Writing in to an ADS
                //NtfsAlternateStream.WriteAllText("test.txt:hide", "Secret content");

                //Reading data from an ADS
                //string text = NtfsAlternateStream.ReadAllText(fileName + streamName);

                //Enumerating all the ADS in test.txt
                IEnumerable<NtfsAlternateStream> adsStreams = NtfsAlternateStream.EnumerateStreams(fileName);
                //bool a=adsStreams.Contains("{:fileTags:$DATA}");
                foreach (NtfsAlternateStream ads in adsStreams)
                {
                    Console.WriteLine(ads.Name);
                }

                //This will not delete the test.txt file
                //NtfsAlternateStream.Delete(fileName + streamName);
                return fileTags;
            
           
        }

        public void windowsSearchForTag()
        {
            var connection = new OleDbConnection(@"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""");

            // File name search (case insensitive), also searches sub directories
            var query1 = @"SELECT System.ItemName,System.FileName,SYSTEM.ITEMURL FROM SystemIndex " +
                        @"WHERE scope ='file:' AND System.Keywords LIKE '%Test%'";
            try
            {
                connection.Open();

                var command = new OleDbCommand(query1, connection);

                using (var r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Console.WriteLine(r[0]);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection ! ");
                throw ex;
            }
        }


    }
}
