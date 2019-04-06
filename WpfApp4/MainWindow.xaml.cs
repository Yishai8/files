using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using CodeFluent.Runtime.BinaryServices;
using System.Collections;

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
           
            Tag a = new Tag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf");
            a.getFileTag();
            a.setFileTag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf", "trying set a tag");

        }

        private void Populate(string header, string tag, TreeView _root, TreeViewItem _child, bool isfile)
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

            foreach (DriveInfo driv in DriveInfo.GetDrives())
            {
                if (driv.IsReady)
                    Populate(driv.VolumeLabel + "(" + driv.Name + ")", driv.Name, foldersItem, null, false);
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

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
            Thumbnails.Items.Clear();
            Tag a = new Tag(temp.Tag.ToString());
            TagsOutput.Text = a.getFileTag();


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
    }
}
