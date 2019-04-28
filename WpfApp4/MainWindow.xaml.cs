﻿using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
            // ObservableCollection<tagsCategory> _Categories = new ObservableCollection<tagsCategory>();
            //_Categories = Tags.TagManagment.LoadCategoriesListFromXML();
            //Views.tagsCategory b = new Views.tagsCategory();
            //b.LoadCategoryListFromXML();
            Tags.XMLFile.init();
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

        private void ThumbnailsOpenFile(object sender, MouseButtonEventArgs e)
        {
            ListView _item = (ListView)sender;
            ListViewItem selected =(ListViewItem) _item.SelectedItem;
            var isFile = new Uri(selected.Tag.ToString()).AbsolutePath.Split('/').Last().Contains('.');
            if(isFile)
                System.Diagnostics.Process.Start(selected.Tag.ToString());
        }
        private void Populate(string header, string tag, TreeView _root, TreeViewItem _child, bool isfile)       //create the tree view
        {
            try
            {
                Icon ic = SysIcon.OfPath(tag);
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
            catch (System.NullReferenceException ex)
            { Console.WriteLine(ex.InnerException); }
            catch (System.UnauthorizedAccessException unauth)
            { Console.WriteLine(unauth.InnerException); }
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
                catch (System.NullReferenceException ex)
                { Console.WriteLine(ex.InnerException); }
                catch (System.UnauthorizedAccessException unauth)
                { Console.WriteLine(unauth.InnerException); }

            }
        }


        void _driitem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem _item = (TreeViewItem)sender;
            if (_item.Items.Count == 1 && ((TreeViewItem)_item.Items[0]).Header == null)
            {
                _item.Items.Clear();

                try
                {

                    foreach (string dir in Directory.GetDirectories(_item.Tag.ToString()))
                    {
                        DirectoryInfo _dirinfo = new DirectoryInfo(dir);
                        if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                            Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, false);
                    }

                    foreach (string dir in Directory.GetFiles(_item.Tag.ToString()))
                    {
                        FileInfo _dirinfo = new FileInfo(dir);
                        if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                            Populate(_dirinfo.Name, _dirinfo.FullName, null, _item, true);


                    }

                }
                catch (System.UnauthorizedAccessException unauth)
                { Console.WriteLine(unauth.InnerException); }

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


            //expend only a folder
            if (!File.Exists(temp.Tag.ToString()))
            {
                try
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
                catch (System.UnauthorizedAccessException unauth)
                { Console.WriteLine(unauth.InnerException); }

            }
            TagsOutput.Text = Tags.TagManagment.getFileTag(temp.Tag.ToString());   //brings the ads on the textblock





        }


        private void folders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {



            if (((TreeView)sender).SelectedItem is TreeViewItem)
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
            if (string.Compare(output, string.Empty) != 0)
                Tags.TagManagment.saveFileTags(_item.Tag.ToString(), output);
            else
                Tags.TagManagment.DeleteFileTags(_item.Tag.ToString());

        }
    }
}
