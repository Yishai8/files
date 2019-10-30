using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp4.Tags;
using System.Windows.Markup;
using System.Xml;
using CodeFluent.Runtime.BinaryServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    //===========================================================================================
    // This is the main program which includes the activities of the Altview.
    // According to the option/action the user does , this program navigates 
    //to the appropriate process and class functions that handles in the specific user's action .
    //============================================================================================

    public partial class MainWindow : Window
    {
        System.Windows.Point _startPoint;
        bool _IsDragging = false;
        ObservableCollection<tagsCategory> Categories = Tags.TagManagment.LoadCategoriesListFromXML();  //tag options 


        public MainWindow()
        {

            InitializeComponent();
            Tags.tagsXMLfunc.init();
            Views.viewsXMLfunc.init();
            lb.ItemsSource = Categories;
            lb2.ItemsSource = Categories;

            //b.LoadCategoryListFromXML();


        }

        private void foldersItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) //context for right button  menu 1 menu 2
        {

            TreeView tv = sender as TreeView;
            //tv.ContextMenu.Visibility = tv.SelectedItem == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        // Enable the user to open the files he drag for view for reading the data or update the data  
        private void ThumbnailsOpenFile(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)    // Left button was double clicked
            {


                if (sender is ListView)
                {
                    ListView _item = (ListView)sender;
                    ListViewItem selected = (ListViewItem)_item.SelectedItem;
                    var isFile = new Uri(selected.Tag.ToString()).AbsolutePath.Split('/').Last().Contains('.');
                    if (isFile)
                        System.Diagnostics.Process.Start(selected.Tag.ToString());
                    return;
                }
                else
                {
                    try
                    {
                        if (((TreeViewItem)e.Source).Header.ToString() == ((TextBlock)e.OriginalSource).DataContext.ToString())
                        {
                            TreeViewItem _item = (TreeViewItem)sender;
                            TreeView par = GetObjectParent(_item);

                            _item = (TreeViewItem)par.SelectedItem;
                            if (_item.Tag != null && _item.Tag.ToString() != "Custom Folder")
                            {
                                var isFile = new Uri(_item.Tag.ToString()).AbsolutePath.Split('/').Last().Contains('.');
                                if (isFile)
                                    System.Diagnostics.Process.Start(_item.Tag.ToString());
                            }
                            e.Handled = true;
                            sender = null;
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                }
            }
        }


        private TreeView GetObjectParent(TreeViewItem obj)
        {

            while (!(obj.Parent.GetType().Name == "TreeView")) //get to treeview parent
                obj = (TreeViewItem)obj.Parent;
            return (TreeView)obj.Parent;
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

        private void Tree_PreviewMouseMove(object sender, MouseEventArgs e)
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

            TreeView par = GetObjectParent((TreeViewItem)e.Source);
            object temp = par.SelectedItem;
            DataObject data = null;
            try
            {
                data = new DataObject("inadt", temp);

                if (data != null)
                {
                    DragDropEffects dde = DragDropEffects.Move;
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        dde = DragDropEffects.All;
                    }
                    DragDropEffects de = DragDrop.DoDragDrop(par, data, dde);
                }
                _IsDragging = false;
            }
            catch (ArgumentNullException ex)
            {
                {
                    Console.WriteLine(ex.InnerException);
                    MessageBox.Show("you have not dragged well ,drag again");
                    _IsDragging = false;

                }
            }

        }

        private void CustomviewTree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            _startPoint = e.GetPosition(null);
        }


        private void Tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {


            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {

                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {

            while (source != null && !(source is TreeViewItem))
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        // Display the list of tags i.e categories and subCategories before perfoming any change in those lists
        // and enables the user to tag view he has created
        private void TagView(object sender, RoutedEventArgs e)
        {
           
            Tags.tagsXMLfunc.init1();  
                                      

            Views.HandleViews b = new Views.HandleViews();
            MenuItem mnu = sender as MenuItem;

            TreeViewItem selected = null;
            TreeView parent = null;
            if (mnu != null)
            {

                selected = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;
                parent = GetObjectParent(selected);
                selected = parent.SelectedItem as TreeViewItem;
            }
            Controls.TagControl.TagDialog inputDialog = new Controls.TagControl.TagDialog(Categories);


            if (inputDialog.ShowDialog() == true)
            {
                var selectedCategoty = inputDialog.lb.SelectedValue as Tags.tagsCategory;
                var selectedSubCategory = inputDialog.lb1.SelectedValue;
                List<string> fileslist = b.getTaggedPaths(selected);
                if (fileslist == null) return; 
                if (selectedSubCategory != null)
                    saveTags(fileslist, selectedCategoty.categoryName + "." + selectedSubCategory);
                else
                    saveTags(fileslist, selectedCategoty.categoryName + ".");
                MessageBox.Show("Tags added successfuly");
            }
            e.Handled = true;
        }
        //Handle dropped files on listBox for tagging 
        private void files_Drop(object sender, DragEventArgs e)
        {

            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var myOtherFilesList = lb_tag.Items.Cast<String>().ToList();

                if (files == null) //files are dragged from treeView
                {

                    files = new[] { (string)((TreeViewItem)(e.Data.GetData(e.Data.GetFormats()[0]))).Tag };
                }
                if ((files[0].ToString()).IndexOf('.') > -1)
                {

                    string streamName = ":fileTags";
                    try
                    {
                        FileStream stream = NtfsAlternateStream.Open(files[0] + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                        stream.Close();
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine(ex.InnerException);
                        MessageBox.Show("you have not authority to the file " + files[0]);
                        return;
                    }

                    if (!myOtherFilesList.Contains(files[0]))
                    {
                        lb_tag.Items.Add(files[0]);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("The file you dragged already exsist in the dropped file list"); return;
                    }
                }
                String[] allfiles = Directory.GetFiles(files[0], "*", SearchOption.AllDirectories);
                if (allfiles.Length == 0)
                {
                    MessageBox.Show("The folder you want to drag has no files, drag ignored");
                    return;

                }



                for (var i = 0; i < allfiles.Length; i++)
                {
                    if (!myOtherFilesList.Contains(allfiles[i]))
                        lb_tag.Items.Add(allfiles[i]);
                    else
                    {
                        MessageBox.Show("The files of the folder you dragged ,already exsist in the dropped file list"); return;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.InnerException);
                MessageBox.Show("you have not authority to those files");
            }

        }

        // Remove the selected file from the drop files list 
        private void removeTagsFromList(object sender, EventArgs e)
        {

            if (lb_tag.SelectedIndex >= 0)
                lb_tag.Items.RemoveAt(lb_tag.SelectedIndex);
        }

        // The user can drag folder   or file to create free view 
        private void CustomviewTree_Drop(object sender, DragEventArgs e)
        {
            try
            {

                if (e.OriginalSource.GetType().Name != "Grid")
                {
                    FileAttributes attr = FileAttributes.Directory; //default
                    TreeViewItem source = e.Source as TreeViewItem;
                    if (source.Tag.ToString() != "Custom Folder") //item dropped on is not custom folder
                        attr = File.GetAttributes(source.Tag.ToString());

                    //treeviewitem moving
                    if (_IsDragging)
                    {

                        TreeViewItem from = (TreeViewItem)e.Source;

                        var List = from.Items.Cast<TreeViewItem>().ToList();


                        TreeViewItem _item = from;// (TreeViewItem)sender;


                        if (_item.Items.Count == 1 && ((TreeViewItem)_item.Items[0]).Header == null)
                        {
                            _item.Items.Clear();



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

                        _item = from;
                        List = from.Items.Cast<TreeViewItem>().ToList();

                        TreeViewItem dest = e.Data.GetData(e.Data.GetFormats()[0]) as TreeViewItem;

                        if ((from.Header.ToString()).IndexOf('.') > -1)
                        {
                            MessageBox.Show("you can't drag file into file only into folder");
                            e.Handled = true;
                            _IsDragging = false;
                            return;
                        }


                        if ((from.Header).Equals(dest.Header))
                        {
                            MessageBox.Show("folder already exsist in the folder you try to drag into");
                            e.Handled = true;
                            _IsDragging = false;
                            return;
                        }

                        foreach (TreeViewItem t in List)
                        {

                            if (t.Header != null)
                            {
                                if ((t.Header).Equals(dest.Header))

                                {
                                    MessageBox.Show("file/folder you try to drag, already exsist in the destination folder , drag denied");
                                    e.Handled = true;
                                    _IsDragging = false;
                                    return;
                                }

                            }
                        }
                        var files1 = (string[])e.Data.GetData(DataFormats.FileDrop);
                        if (files1 == null) //files are dragged from treeView
                        {

                            files1 = new[] { (string)((TreeViewItem)(e.Data.GetData(e.Data.GetFormats()[0]))).Tag };
                        }
                        foreach (string f in files1)
                        {

                            attr = File.GetAttributes(f);

                            if (!(attr.HasFlag(FileAttributes.Directory) || source.Tag.ToString() == "Custom Folder"))
                            {

                                Populate(Path.GetFileName(f), f, null, source, true); //dropped file is a file

                            }

                        }



                        if ((GetObjectParent(dest)).Name == "foldersItem")
                        {
                            //files drop
                            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            if (files == null) //files are dragged from treeView
                            {

                                files = new[] { (string)((TreeViewItem)(e.Data.GetData(e.Data.GetFormats()[0]))).Tag };
                            }
                            foreach (string f in files)
                            {

                                attr = File.GetAttributes(f);

                                if (attr.HasFlag(FileAttributes.Directory) || source.Tag.ToString() == "Custom Folder")
                                {
                                    if (attr.HasFlag(FileAttributes.Directory))
                                        Populate(Path.GetFileName(f), f, null, source, false); //dropped file is a folder
                                    else
                                        Populate(Path.GetFileName(f), f, null, source, true); //dropped file is a file

                                }

                            }


                        }
                        else
                        {
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
                        }





                        e.Handled = true;
                        _IsDragging = false;
                        return;
                    }


                }
            }
            catch (Exception ex) { MessageBox.Show("you did invalid drag"); return; }
        }


        // Drag the root level  for creating free view 
        //(the user can also create new root level according to his needs and not to drag )
        private void CustomTree_Drop(object sender, DragEventArgs e)
        {

            if (e.OriginalSource.GetType().Name == "Grid")
            {
                TreeView source = e.Source as TreeView;
                if (_IsDragging)
                {
                    TreeView from = e.Source as TreeView;
                    var List = from.Items.Cast<TreeViewItem>().ToList();


                    TreeViewItem dest = e.Data.GetData(e.Data.GetFormats()[0]) as TreeViewItem;

                    TreeViewItem searchItem = List.Find(x => x.Header.ToString().Equals(dest.Header));

                    if (searchItem == null)
                    {

                        if ((GetObjectParent(dest)).Name == "foldersItem")
                        {

                            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                            var myOtherList = source.Items.Cast<TreeViewItem>().ToList();

                            if (files == null) //files are dragged from treeView
                            {

                                files = new[] { (string)((TreeViewItem)(e.Data.GetData(e.Data.GetFormats()[0]))).Tag };
                            }
                            foreach (string f in files)
                            {

                                TreeViewItem searchItem1 = myOtherList.Find(x => x.Header.ToString().Equals(Path.GetFileName(f)));
                                if (searchItem1 == null)
                                {

                                    // source.Items.Add(newEntry);
                                    var isFile = new Uri(f).AbsolutePath.Split('/').Last().Contains('.');
                                    if (!isFile)
                                    {
                                        if (!(new DirectoryInfo(f).FullName == new DirectoryInfo(f).Root.FullName))
                                            Populate(Path.GetFileName(f), f, source, null, false);
                                        else
                                            Populate(f, f, source, null, false);

                                        // path is a directory.


                                    }

                                    else
                                        Populate(Path.GetFileName(f), f, source, null, true);
                                }
                            }


                        }
                        else
                        {

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
                        }

                    }



                    else
                        MessageBox.Show("Item with the same name exists in destination - Drag failed");



                    TreeViewItem _item = dest;


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

                        catch (Exception ex)
                        { Console.WriteLine(ex.InnerException); }




                    }


                    e.Handled = true;
                    _IsDragging = false;
                    return;
                }


            }
        }



        public static T Clone<T>(T from)

        {

            string objStr = XamlWriter.Save(from);

            StringReader stringReader = new StringReader(objStr);

            XmlReader xmlReader = XmlReader.Create(stringReader);

            object clone = XamlReader.Load(xmlReader);

            return (T)clone;

        }

        // The user can add child folders as much as he wants and in each level he wants
        private void addNode(object sender, RoutedEventArgs e)
        {

            TreeViewItem newRoot = new TreeViewItem();
            Controls.InputDialog.inputMessage inputDialog = new Controls.InputDialog.inputMessage("Please enter folder name", "");
            inputDialog.Title = "Add New Folder";
            if (inputDialog.ShowDialog() == true)
            {
                newRoot.Header = inputDialog.txtAnswer.Text;
                newRoot.Tag = "Custom Folder";
                //first root for empty tree

                if (CustomviewTree.Items.Count < 1)
                {


                    //TreeViewItem topParent = (TreeViewItem)CustomviewTree.Items[0];

                    CustomviewTree.Items.Add(newRoot);
                    return;


                }
                else
                {
                    TreeViewItem DestToAdd = (TreeViewItem)CustomviewTree.SelectedItem;

                    if (DestToAdd != null)
                    {

                        if ((DestToAdd.Header.ToString()).IndexOf('.') > -1)
                        {
                            MessageBox.Show("you add folder  into  file, you can drag folder only into folder  ");

                            return;
                        }

                        DestToAdd.Items.Add(newRoot);
                    }
                    else
                    {
                        MessageBox.Show("No Item was selected - please select an item to add the folder to");
                        return;
                    }

                }


            }

        }
        //  Enables the user to add his own  root folder to the view he wants to create 
        private void addRootNode(object sender, RoutedEventArgs e)
        {

            TreeViewItem newRoot = new TreeViewItem();
            Controls.InputDialog.inputMessage inputDialog = new Controls.InputDialog.inputMessage("Please enter folder name", "");
            inputDialog.Title = "Add New Folder";
            if (inputDialog.ShowDialog() == true && inputDialog.Answer != string.Empty)
            {


                if (viewName.Text != string.Empty)
                {
                    TreeViewItem topParent = (TreeViewItem)CustomviewTree.Items[0];
                    newRoot.Header = inputDialog.txtAnswer.Text;
                    newRoot.Tag = "Custom Folder";
                    topParent.Items.Add(newRoot);

                }
                else
                {
                    newRoot.Header = inputDialog.txtAnswer.Text;
                    newRoot.Tag = "Custom Folder";
                    CustomviewTree.Items.Add(newRoot);
                }
                //inputDialog.txtAnswer.Text = viewName.Text;


            }

            return;



        }

        private void checkDistinct(object sender, RoutedEventArgs e)
        {

        }

        private void removeNode(object sender, RoutedEventArgs e)  // remove selected folders/files  from view 
        {


            try
            {
                TreeViewItem selected = (TreeViewItem)CustomviewTree.SelectedItem;

                if (viewName.Text != string.Empty && selected.Parent.GetType().Name == "TreeView")
                {
                    string messageBoxText = "Are you sure you want to delete view " + viewName.Text + "?";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    if (MessageBox.Show(messageBoxText, null, button, icon) == MessageBoxResult.No)
                        return;
                    else
                    {
                        Views.HandleViews b = new Views.HandleViews();
                        b.deleteCustomView(viewName.Text);
                        MessageBox.Show("View deleted");
                        CustomviewTree.Items.Clear();
                        return;

                    }

                }

                if (selected.Parent != null && selected.Parent.GetType().Name != "TreeView")
                {
                    var par = (TreeViewItem)selected.Parent;
                    var itemIndex = par.Items.IndexOf(selected);

                    par.Items.Remove(selected);


                }
                else
                {

                    CustomviewTree.Items.Remove(CustomviewTree.SelectedItem);
                }
                MessageBox.Show("press save control to keep changes");

            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.InnerException);
                MessageBox.Show("Choose the item you want to remove");
            }

        }


        private void createNewTree(object sender, RoutedEventArgs e)
        {


            CustomviewTree.Items.Clear();
            viewName.Text = string.Empty;

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

                catch (Exception ex)
                { Console.WriteLine(ex.InnerException); }


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



            //expand only a folder
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


                        }
                    }
                    foreach (string file in Directory.GetFiles(temp.Tag.ToString()))
                    {
                        FileInfo _dirinfo = new FileInfo(file);
                        if ((_dirinfo.Attributes & FileAttributes.System) == 0)
                        {
                            ListViewItem item = new ListViewItem { Content = file };
                            item.Tag = file;


                        }
                    }
                }
                catch (System.UnauthorizedAccessException unauth)
                { Console.WriteLine(unauth.InnerException); }

            }

            TagsOutput.Content = Tags.TagManagment.getFileTag(temp.Tag.ToString());   //brings the ads on the textblock






        }


        private void saveTags(List<string> fileNames, string tag) //tag the file

        {

            TreeViewItem _item = (TreeViewItem)foldersItem.SelectedItem;

            Tags.TagManagment.saveFileTags(fileNames, tag);


        }






        private void getView(object sender, RoutedEventArgs e) //populate treeview with view
        {

            Views.HandleViews b = new Views.HandleViews();
            var selectedCategoryLB = lb2.SelectedValue as Tags.tagsCategory;
            var selectedSubCategory = lb3.SelectedValue;
            if (selectedCategoryLB != null)
            {
                string selectedCategory = selectedCategoryLB.categoryName;
                string selectedSubCategoryName = null; ;
                List<RadioButton> radioButtons = Radio.Children.OfType<RadioButton>().ToList();
                RadioButton rbTarget = radioButtons
                 .Where(r => r.GroupName == "tagsWay" && r.IsChecked == true)
                 .Single();

                if (rbTarget.Content.ToString() == "Main Category")
                {
                    selectedSubCategoryName = "";
                }
                else
                {
                    if (selectedSubCategory != null)
                    {
                        selectedSubCategoryName = lb3.SelectedValue.ToString();
                    }
                    else
                    {
                        MessageBox.Show("sub category was not selected");
                        return;
                    }


                }




                b.createViewByTag(rbTarget.Content.ToString(), selectedCategory + "." + selectedSubCategory, viewTree);
            }
            else
            {
                MessageBox.Show("main category was not selected");
                return;
            }




        }

        private void saveView(object sender, RoutedEventArgs e) //save the view created by the user 
        {

            Views.HandleViews b = new Views.HandleViews();
            //save existing view
            if (viewName.Text != string.Empty)
            {
                b.saveCustomView(CustomviewTree, viewName.Text, true);
                return;
            }

            if (CustomviewTree.Items.Count > 0)
            {

                Controls.InputDialog.inputMessage inputDialog = new Controls.InputDialog.inputMessage("Please enter view name", "");
                inputDialog.Title = "Save Custom View";
                if (viewName.Text != string.Empty)
                    inputDialog.txtAnswer.Text = viewName.Text;
                if (inputDialog.ShowDialog() == true && inputDialog.Answer != string.Empty)
                {

                    string msg = b.saveCustomView(CustomviewTree, inputDialog.Answer, false);
                    if (msg == "success")
                    {
                        MessageBox.Show("View saved");
                        viewName.Text = inputDialog.Answer;

                    }

                    else { MessageBox.Show("View wasn't saved as view name already exists"); return; }


                }
            }

        }

        private void LoadView(object sender, RoutedEventArgs e) //populate treeview with view
        {

            Views.HandleViews b = new Views.HandleViews();
            List<string> NamesList = b.getCustomViewsList();
            Controls.List.ListMessage inputDialog = new Controls.List.ListMessage(NamesList);

            //b.LoadCustomView

            //string msg = b.LoadCustomView(CustomviewTree, inputDialog.Answer);
            if (inputDialog.ShowDialog() == true)
            {
                b.LoadCustomView(CustomviewTree, inputDialog.lblQuestion.SelectedItem.ToString());
                viewName.Text = inputDialog.lblQuestion.SelectedItem.ToString();
                //MessageBox.Show("To Tag View : Right Click View");
            }


        }

        private void clearTagsFromList(object sender, RoutedEventArgs e)
        {

            lb_tag.Items.Clear();

        }

        private void changeBinding(object sender, SelectionChangedEventArgs e)
        {

            ListBox av = (ListBox)sender;
            var Categories = Tags.TagManagment.LoadCategoriesListFromXML();
            var a = (Tags.tagsCategory)(((ListBox)sender).SelectedItem);
            if (av.Name == "lb")

                lb1.ItemsSource = a.categoryOptions;
            else
                lb3.ItemsSource = a.categoryOptions;
        }

        private void addTags(object sender, RoutedEventArgs e)  // handles in adding tags to files
        {


            var selectedCategoty = lb.SelectedValue as Tags.tagsCategory;
            var selectedSubCategory = lb1.SelectedValue;
            if (selectedCategoty == null)
            {
                MessageBox.Show("Main category was not selected");
                return;
            }

            if (selectedSubCategory == null)
            {
                MessageBox.Show("you have to select subCategory");
                return;

            }

            if (lb_tag.Items.Count > 0)
            {
                List<string> fileslist = lb_tag.Items.Cast<String>().ToList();

                saveTags(fileslist, selectedCategoty.categoryName + "." + selectedSubCategory);

            }
            else
            {
                MessageBox.Show(" you have to drag the files you want to tag ");
                return;
            }

            //lb_tag.Items.Clear();

            return;
        }


        //delete all tags from selected files 

        private void deleteTags(object sender, RoutedEventArgs e)
        {

            string docFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Tags.xml";

            XDocument xmlDocument = XDocument.Load(docFilePath);

            XDocument doc;

            doc = XDocument.Load(docFilePath);



            if (lb_tag.Items.Count > 0)
            {

                string messageBoxText = "Are you sure you want to delete tags from those files" + "?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                if (MessageBox.Show(messageBoxText, null, button, icon) == MessageBoxResult.No)
                    return;

                List<string> fileslist = lb_tag.Items.Cast<String>().ToList();

                string streamName = ":fileTags";



                FileStream stream = NtfsAlternateStream.Open(fileslist[0] + streamName, FileAccess.ReadWrite, FileMode.OpenOrCreate, FileShare.None);
                stream.Close();
                IEnumerable<NtfsAlternateStream> fileStream = NtfsAlternateStream.EnumerateStreams(fileslist[0]);

                for (var i = 0; i < fileslist.Count; i++)
                {

                    NtfsAlternateStream.Delete(fileslist[i] + streamName);


                    doc.Element("root").Elements("tag").Elements("path")

                      .Where(x => x.Attribute("value").Value == fileslist[i]).Remove();


                    doc.Save(docFilePath);

                }


                MessageBox.Show("Tags deleted successfuly");
            }
            else
            {
                MessageBox.Show(" you have to drag the files you want to tag ");


            }

            lb_tag.Items.Clear();
            doc = XDocument.Load(docFilePath);
            return;
        }
        //delete  selected tags from file

        private void deleteTags1(object sender, RoutedEventArgs e)
        {

            var selectedCategoty = lb.SelectedValue as Tags.tagsCategory;
            var selectedSubCategory = lb1.SelectedValue;
            if (selectedCategoty == null)
            {
                MessageBox.Show("Main category was not selected");
                return;
            }
            if (selectedSubCategory == null)
            {
                MessageBox.Show("you have to select subCategory");
                return;

            }

            if (lb_tag.Items.Count > 0)
            {
                string messageBoxText = "Are you sure you want to delete this category from the above files " + "?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                if (MessageBox.Show(messageBoxText, null, button, icon) == MessageBoxResult.No)
                    return;
                List<string> fileslist = lb_tag.Items.Cast<String>().ToList();

                saveTags_for_delete(fileslist, selectedCategoty.categoryName + "." + selectedSubCategory);

                MessageBox.Show("Tags  deleted successfuly");

            }
            else
            {
                MessageBox.Show(" you have to drag the files you want to tag ");
                return;
            }

            lb_tag.Items.Clear();

        }

        //The selected tags to be deleted  from selected files

        private void saveTags_for_delete(List<string> fileNames, string tag) //tag the file

        {


            TreeViewItem _item = (TreeViewItem)foldersItem.SelectedItem;

            Tags.TagManagment.saveFileTags1(fileNames, tag);


        }


        private void open(object sender, RoutedEventArgs e)   // handles with the creating of filtered view 
        {

            Controls.TagControl.andOrSelection inputDialog = new Controls.TagControl.andOrSelection(Categories);
            inputDialog.Title = "Set Custom View filters";
            Tags.tagsXMLfunc.init1();  
            if (inputDialog.ShowDialog() == true)
            {
                Views.HandleViews b = new Views.HandleViews();
                viewTree.Items.Clear();
                List<string> filterParams = inputDialog.paramLV.Items.Cast<string>()
                                 .Select(x => x.ToString()).ToList();
                if (filterParams.Count == 0)
                    return;
                b.getComplexTags(viewTree, filterParams);
                if (viewTree.Items.Count == 0)
                    MessageBox.Show("No results were found for your selection");
                else
                {
                    MessageBox.Show("To clear the view ,press the 'Create Filter View' button  and than  press ok");

                    string filters = "";
                    foreach (string param in filterParams)
                    {

                        filters = filters + " ( " + param + " )   ";
                    }

                    MessageBox.Show("View filter is:  " + filters);
                    b.saveCustomView(viewTree, "view" + filters, true);
                    MessageBox.Show("view" + filters + "created");
                }
                return;
            }
        }
    }
}