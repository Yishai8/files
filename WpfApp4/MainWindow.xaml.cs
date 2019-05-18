using System.Collections.Generic;
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
        System.Windows.Point _startPoint;
        bool _IsDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            // ObservableCollection<tagsCategory> _Categories = new ObservableCollection<tagsCategory>();
            //_Categories = Tags.TagManagment.LoadCategoriesListFromXML();
            //Views.tagsCategory b = new Views.tagsCategory();
            //b.LoadCategoryListFromXML();
            Tags.XMLFile.init();
            
          
            Tags.TagManagment.getPathsByTag("test",true);

            //Tag a = new Tag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf");
            //List<string> d= a.windowsSearch("Test");
            //a.getFileTag();
            //a.setFileTag(@"C:\Users\Yishai\Downloads\תרגיל 3 - גבולות.pdf", "trying set a tag");

        }

        private void foldersItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) //context for right button  menu 1 menu 2
        {
            TreeView tv = sender as TreeView;
            //tv.ContextMenu.Visibility = tv.SelectedItem == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
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

        private void CustomviewTree_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_IsDragging)

            {

                System.Windows.Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||

                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)

                {

                    StartDrag(e);


                }

            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            _IsDragging = true;
            object temp = this.CustomviewTree.SelectedItem;
            DataObject data = null;

            data = new DataObject("inadt", temp);

            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Move;
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    dde = DragDropEffects.All;
                }
                DragDropEffects de = DragDrop.DoDragDrop(this.CustomviewTree, data, dde);
            }
            _IsDragging = false;
        }

        private void CustomviewTree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);

        }

        private void CustomviewTree_Drop(object sender, DragEventArgs e)
        {
            if(e.OriginalSource.GetType().Name != "Grid")
            { 
            TreeViewItem source = e.Source as TreeViewItem;
           
            //treeviewitem moving
            if (_IsDragging)
            {
                TreeViewItem dest = e.Data.GetData(e.Data.GetFormats()[0]) as TreeViewItem;
                if ((dest.Parent as TreeViewItem)!=null)
                {
                    (dest.Parent as TreeViewItem).Items.Remove(dest);
                }

                    else if ((dest.Parent as TreeView) != null)
                    {
                        (dest.Parent as TreeView).Items.Remove(dest);
                    }
                    source.Items.Remove(dest);
                source.Items.Insert(0, dest);
                e.Handled = true;
                _IsDragging = false;
                return;
            }
            //files drop
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string f in files)
            {
                TreeViewItem newEntry = new TreeViewItem();
                newEntry.Header = f;
               // source.Items.Add(newEntry);
                Populate(Path.GetFileName(f),f, null, source, false);
            }
            }

        }

        private void CustomTree_Drop(object sender, DragEventArgs e)
        {
            if (e.OriginalSource.GetType().Name == "Grid")
            {
                TreeView source = e.Source as TreeView;
                if (_IsDragging)
                {
                    TreeView from = e.Source as TreeView;
                    TreeViewItem dest = e.Data.GetData(e.Data.GetFormats()[0]) as TreeViewItem;
                    if ((dest.Parent as TreeViewItem) != null)
                    {
                        (dest.Parent as TreeViewItem).Items.Remove(dest);
                    }

                    else if ((dest.Parent as TreeView) != null)
                    {
                        (dest.Parent as TreeView).Items.Remove(dest);
                    }
                    from.Items.Remove(dest);
                    from.Items.Insert(0, dest);
                    e.Handled = true;
                    _IsDragging = false;
                    return;
                }

                //files drop
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string f in files)
                {
                    TreeViewItem newEntry = new TreeViewItem();
                    newEntry.Header = f;
                    // source.Items.Add(newEntry);
                    var isFile = new Uri(f).AbsolutePath.Split('/').Last().Contains('.');
                    if (!isFile)
                        Populate(Path.GetFileName(f), f, source, null, false);
                    else
                        Populate(Path.GetFileName(f), f, source, null, true);

                }
            }

        }
        private void addNode(object sender, RoutedEventArgs e)
        {
            //first root for empty tree
            if (CustomviewTree.Items.Count < 1)
            {
                TreeViewItem a = new TreeViewItem();
                a.Header = "aa";
                CustomviewTree.Items.Add(a);
                return;
            }
            TreeViewItem aa = new TreeViewItem();
            aa.Header = "ab";
            TreeViewItem d = (TreeViewItem)CustomviewTree.SelectedItem;
           if(d!=null) d.Items.Add(aa);



        }

        private void addRootNode(object sender, RoutedEventArgs e)
        {
            TreeViewItem aa = new TreeViewItem();
            aa.Header = "ac";
            CustomviewTree.Items.Add(aa);
            return;



        }

        private void removeNode(object sender, RoutedEventArgs e)
        {
    
            try { 
            TreeViewItem selected = (TreeViewItem)CustomviewTree.SelectedItem;

            if (selected.Parent != null && selected.Parent.GetType().Name!= "TreeView")
            {
                var par = (TreeViewItem)selected.Parent;
                    var itemIndex = par.Items.IndexOf(selected);
                par.Items.Remove(selected);
                if(par.Items.Count>0)
                    {
                        if (itemIndex == 0)
                            ((TreeViewItem)par.Items[0]).IsSelected = true;
                        else
                            ((TreeViewItem)par.Items[itemIndex]).IsSelected = true;

                    }


            }
            else
            {
                
                CustomviewTree.Items.Remove(CustomviewTree.SelectedItem);
            }
                
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.InnerException);
            }




        }

        private void createNewTree(object sender, RoutedEventArgs e)
        {

            CustomviewTree.Items.Clear();

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

        private void getView(object sender, RoutedEventArgs e) //populate treeview with view
        {
            Views.HandleViews b = new Views.HandleViews();
            string output = viewTxt.Text;
            b.createViewSubTag(output, viewTree);
            
            

        }

    }
}
