using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Views.SystemView b = new Views.SystemView();
            b.LoadCategoryListFromXML();
            XMLFile.init();
            //Tag a = new Tag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf");
            //List<string> d= a.windowsSearch("Test");
            //a.getFileTag();
            //a.setFileTag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf", "trying set a tag");

        }

        private void foldersItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) //context for right button  menu 1 menu 2
        {
            TreeView tv = sender as TreeView;
            tv.ContextMenu.Visibility = tv.SelectedItem == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
        private void Populate(string header, string tag, TreeView _root, TreeViewItem _child, bool isfile)       //create the tree view
        {
           
           Icon ic= SysIcon.OfPath(tag);
            TreeViewItem _driitem = new TreeViewItem();
            _driitem.Tag = tag;
            _driitem.Header = header;
          
            _driitem.Expanded += new RoutedEventHandler(_driitem_Expanded);
            if (!isfile)
                _driitem.Items.Add(new TreeViewItem());

            if (_root != null)
            { _root.Items.Add(_driitem); }
            else { _child.Items.Add(_driitem); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (DriveInfo driv in DriveInfo.GetDrives())   //fetch the drives info
            {
                try
                {
                    if (driv.IsReady)
                    { //check drive access permissions
                        var a = Directory.GetAccessControl(driv.Name);
                    
                        Populate(driv.VolumeLabel + "(" + driv.Name + ")", driv.Name, foldersItem, null, false);
                    }
                }
                catch (UnauthorizedAccessException ex)
                { }

            }
        }


        void _driitem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem _item = (TreeViewItem)sender;  
            if (_item.Items.Count == 1 && ((TreeViewItem)_item.Items[0]).Header == null)
            {
                _item.Items.Clear();
                
                foreach (string dir in Directory.GetDirectories(_item.Tag.ToString()))
                {
                    DirectoryInfo _dirinfo = new DirectoryInfo(dir);
                    if((_dirinfo.Attributes & FileAttributes.System) ==0 )
                    Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, false);
                }

                foreach (string dir in Directory.GetFiles(_item.Tag.ToString()))
                {
                    FileInfo _dirinfo = new FileInfo(dir);
                    if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                        Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, true);
                  

                    

                }

            }
          
        }

        void _driitem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            //you can access item properties eg item.Header etc. 
            //your logic here 
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)   //files on right side of the tree view
        {

            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
            Thumbnails.Items.Clear();
            Tag a = new Tag(temp.Tag.ToString());
           


            //expend only a folder
            if (!File.Exists(temp.Tag.ToString()))
            { 

                foreach (string dir in Directory.GetDirectories(temp.Tag.ToString()))
            {
                DirectoryInfo _dirinfo = new DirectoryInfo(dir);
                if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                { 
                    ListViewItem item = new ListViewItem { Content = dir };
                item.Tag = dir;

                Thumbnails.Items.Add(item);
                }
            }
            foreach (string file in Directory.GetFiles(temp.Tag.ToString()))
            {
                FileInfo _dirinfo = new FileInfo(file);
                if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                { 
                    ListViewItem item = new ListViewItem { Content = file };
                item.Tag = file;

                Thumbnails.Items.Add(item);
                }
            }

            }
            TagsOutput.Text = a.getFileTag();   //brings the ads on the textblock





        }


        private void folders_MouseDoubleClick(object sender, MouseButtonEventArgs e) 
        {
            


                if (((TreeView)sender).SelectedItem is TreeViewItem )
            {
                if (MessageBox.Show("Close Application?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
            }

        }

        private void saveTags(object sender, RoutedEventArgs e) //tag the file
        {
            string output = TagsOutput.Text;
            TreeViewItem _item = (TreeViewItem)foldersItem.SelectedItem;
            Tag a = new Tag(_item.Tag.ToString());
            if (string.Compare(output, string.Empty) != 0)
                a.saveFileTags(_item.Tag.ToString(), output);
            else
                a.DeleteFileTags(_item.Tag.ToString());

        }
    }
}
