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
                FileStream stream = NtfsAlternateStream.Open(fileName + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileName);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                           return Regex.Replace(NtfsAlternateStream.ReadAllText(fileName + streamName),"\n|\r", "");
                        }
                }
            }

            return "The file doesn't have tags";



        }


        public void saveFileTags(string fileName,string tags)
        {
            string streamName = ":fileTags";

                FileStream stream = NtfsAlternateStream.Open(fileName + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileName);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                             NtfsAlternateStream.WriteAllText(fileName+streamName, tags);
                            
                        }
                }
            

        }

        public void DeleteFileTags(string fileName)
        {
            string streamName = ":fileTags";
            if (checkTagFileExists(fileName))
            {


                FileStream stream = NtfsAlternateStream.Open(fileName + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileName);
                foreach (NtfsAlternateStream ads in fileStream)
                {
                    if (ads.StreamType.ToString().Equals("AlternateData"))
                        if (ads.Name.Equals(streamName + ":$DATA"))

                        {
                            NtfsAlternateStream.Delete(fileName+streamName);

                        }
                }
            }

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
                

                //This will not delete the test.txt file
                //NtfsAlternateStream.Delete(fileName + streamName);
                return fileTags;
            
           
        }

        public void windowsSearchForTag()
        {
            var connection = new OleDbConnection(@"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""; Data Source=(local);");

            // File name search (case insensitive), also searches sub directories
            var query1 = @"SELECT System.ItemName,System.FileName,SYSTEM.ITEMURL FROM SystemIndex " +
                        @"WHERE scope ='file:C:/' AND SYSTEM.ITEMURL LIKE '%AdwCleaner%'";
            try
            {
                connection.Open();

                var command = new OleDbCommand(query1, connection);

                using (var r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Console.WriteLine(r[2]);
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

        public List<string> windowsSearch(string tag)
        {
            string[] drives = System.Environment.GetLogicalDrives();
            List<string> tagsList = new List<string>();
            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                // Here we skip the drive if it is not ready to be read. This
                // is not necessarily the appropriate action in all scenarios.
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir,tagsList,tag);
            }
            return tagsList;
        }

        static void WalkDirectoryTree(System.IO.DirectoryInfo root,List<string> tagslist,string tag)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            IEnumerable<FileInfo> afiles = null;
            IEnumerable<DirectoryInfo> afolders = null;
            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");
                afiles = files.Where(f => !f.Attributes.HasFlag(FileAttributes.System));
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
               
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (afiles != null)
            {
                foreach (System.IO.FileInfo fi in afiles)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                   
                    var a = ShellFile.FromFilePath(fi.FullName);

                    try
                    {
                        if(a.Properties.System.Keywords.Value[0] ==tag)
                        { tagslist.Add(fi.FullName); }
                    }
                    catch(Exception ex)
                    { Console.WriteLine(fi.FullName + ex); }
                    
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();
                afolders = subDirs.Where(f => !f.Attributes.HasFlag(FileAttributes.System));
                foreach (System.IO.DirectoryInfo dirInfo in afolders)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo,tagslist,tag);
                }
            }
        }


        }
}
