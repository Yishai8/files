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
//
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using WpfApp4.Tags;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;




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
		 public string getFileTag()
        {
            //Create a stream supporting ADS syntax
            string fileName = this._path;
            
                string fileTags = getFileStream(fileName);  //brings the tags by the filename

                //Writing in to an ADS
                //NtfsAlternateStream.WriteAllText("test.txt:hide", "Secret content");

                //Reading data from an ADS
                //string text = NtfsAlternateStream.ReadAllText(fileName + streamName);

                //Enumerating all the ADS in test.txt
              // IEnumerable<NtfsAlternateStream> adsStreams = NtfsAlternateStream.EnumerateStreams(fileName);
                //bool a=adsStreams.Contains("{:fileTags:$DATA}");
                

                //This will not delete the test.txt file
                //NtfsAlternateStream.Delete(fileName + streamName);
                return fileTags;
            
           
        }

		
        public void saveFileTags(string tags)   //write text to ads for adding tags
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

		
		
		// arrange the new remained tags  after  deleting tags from files
		 public void saveFileTags1(string tags)   //write text to ads  for deleting tags
        {    
            string streamName = ":fileTags";

            string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";
		    XDocument xmlDocument = XDocument.Load(docFilePath);
			
		    XDocument doc;
			
		    doc = XDocument.Load(docFilePath);   
			string file1=this._path;
			var ind_1=0 ;
			var ind1=0 ;			  
			string cat = tags;
			string subCat = "";
			
			 var ind = tags.IndexOf('.');
			
			if (ind != -1) 
			
			{
			  
			  ind_1 = ind-1;
			  ind1  = ind+1;
			  
			   cat = tags.Substring(0,(ind));
			   subCat = tags.Substring((ind1));
			}
			
			string cc = getFileTag();
			
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
                       
						   if(list.Contains(tags))
						   {  
					      
							var currentTagsLength = currentTags.Length;  
							var tagsLength = tags.Length;  
							for (var i=1;i< currentTagsLength ;i++)
							{   
								
								if (currentTags.Substring(i,tagsLength) == tags)
								{  
							        
							        currentTags=currentTags.Remove(i-1,tagsLength+1);
									
									if (currentTags == "")
										NtfsAlternateStream.WriteAllText(this._path,currentTags );  //haviva
									else

									NtfsAlternateStream.WriteAllText(this._path + streamName, currentTags);
									
									
									if (ind == -1)
									{
									  doc.Element("root").Elements("tag").Elements("path").Where(x => x.Parent.Attribute("name").Value == cat && x.Attribute("value").Value == file1).Remove();	
										
									}
									else
									{
                                     doc.Element("root").Elements("tag").Elements("path").Where(x => x.Parent.Attribute("name").Value == cat && x.Parent.Attribute("value").Value == subCat && x.Attribute("value").Value == file1).Remove();
                                    
				                  	}
									 doc.Save(docFilePath);
									 return;
				
			                    }	 

							}   
								
						}   
				  }  	
				         
						    
            }   
      }   
            

        
       

 public string getFileTag1()
        {
            //Create a stream supporting ADS syntax for delete
            
			string fileName = this._path;
            
                string fileTags = getFileStream(fileName);  //brings the tags by the filename
       

                return fileTags;
            
           
        }

        public void windowsSearchForTag()   //search on index tags on file made by windows , systemindex is all the indexed files the query can search only indexed files - is windows search
        {
            var connection = new OleDbConnection(@"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""; Data Source=(local);"); //windows files properties db

            // File name search (case insensitive), also searches sub directories
            var query1 = @"SELECT System.ItemName,System.FileName,SYSTEM.ITEMURL FROM SystemIndex " +   //filename itemanem itemurl r[2]    brings only files with system index
                        @"WHERE scope ='file:C:/' AND SYSTEM.ITEMURL LIKE '%AdwCleaner%'";  //brings adwcleaner by filename
            try
            {
                connection.Open();

                var command = new OleDbCommand(query1, connection);

                using (var r = command.ExecuteReader()) //bring all the items to r r[0],r[1],r[2]
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

        public List<string> windowsSearch(string tag)   //run on all the items tree on windows to search for tags - not windows search nothing to do with indexes
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
              /*  WalkDirectoryTree(rootDir,tagsList,tag);*/
            }
            return tagsList;
        }

      


      }

}
