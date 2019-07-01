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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Point _startPoint;
        bool _IsDragging = false;
        ObservableCollection<tagsCategory> Categories = Tags.TagManagment.LoadCategoriesListFromXML();  //tag options 


        public MainWindow()
        {
            InitializeComponent();
            Tags.XMLFile.init();
            lb.ItemsSource = Categories;
            lb2.ItemsSource = Categories;
          
            //b.LoadCategoryListFromXML();


        }

        private void foldersItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) //context for right button  menu 1 menu 2
        {
            TreeView tv = sender as TreeView;
            //tv.ContextMenu.Visibility = tv.SelectedItem == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

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

        private void TagView(object sender, RoutedEventArgs e)
        {
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
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var myOtherFilesList = lb_tag.Items.Cast<String>().ToList();
            //list.Add(lb_tag.Items);
            if (files == null) //files are dragged from treeView
            {
                files = new[] { (string)((TreeViewItem)(e.Data.GetData(e.Data.GetFormats()[0]))).Tag };
            }
            foreach (string f in files)
            {
                if (!myOtherFilesList.Contains(f))
                    lb_tag.Items.Add(f);
            }
        }

        private void removeTagsFromList(object sender, EventArgs e)
        {
            if (lb_tag.SelectedIndex >= 0)
                lb_tag.Items.RemoveAt(lb_tag.SelectedIndex);
        }



        private void CustomviewTree_Drop(object sender, DragEventArgs e)
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
                    TreeViewItem dest = e.Data.GetData(e.Data.GetFormats()[0]) as TreeViewItem;
                    TreeViewItem searchItem = List.Find(x => x.Header.ToString().Equals(dest.Header));
                    if (searchItem == null)
                    {
                        if ((GetObjectParent(dest)).Name == "foldersItem")
                        {
                            ((TreeViewItem)e.Source).Items.Add(Clone<TreeViewItem>(dest));
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
                        MessageBox.Show("Item with the same name exists in destination - Copy failed");
                    e.Handled = true;
                    _IsDragging = false;
                    return;
                }
                //files drop
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
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

        }

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
                            from.Items.Add(Clone<TreeViewItem>(dest));
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
                        MessageBox.Show("Item with the same name exists in destination - Copy failed");
                    e.Handled = true;
                    _IsDragging = false;
                    return;
                }




                //files drop
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var myOtherList = source.Items.Cast<TreeViewItem>().ToList();
                foreach (string f in files)
                {
                    TreeViewItem searchItem = myOtherList.Find(x => x.Header.ToString().Equals(Path.GetFileName(f)));
                    if (searchItem == null)
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
        }

        public static T Clone<T>(T from)

        {

            string objStr = XamlWriter.Save(from);

            StringReader stringReader = new StringReader(objStr);

            XmlReader xmlReader = XmlReader.Create(stringReader);

            object clone = XamlReader.Load(xmlReader);

            return (T)clone;

        }

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
                    if (DestToAdd != null) DestToAdd.Items.Add(newRoot);
                    else
                        MessageBox.Show("No Item was selected - please selet a item to add the folder to");
                }


            }

        }

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

        private void removeNode(object sender, RoutedEventArgs e)
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
                    if (par.Items.Count > 0)
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
            //if (string.Compare(output, string.Empty) != 0)
            Tags.TagManagment.saveFileTags(fileNames, tag);
            //else
            //  Tags.TagManagment.DeleteFileTags(_item.Tag.ToString());

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
                MessageBox.Show("main category was not selected");



        }

        private void saveView(object sender, RoutedEventArgs e) //populate treeview with view
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

                    else
                        MessageBox.Show("View wasn't saved as view name already exists");

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

        private void addTags(object sender, RoutedEventArgs e)
        {
            var selectedCategoty = lb.SelectedValue as Tags.tagsCategory;
            var selectedSubCategory = lb1.SelectedValue;
            if (selectedCategoty == null)
            {
                MessageBox.Show("Main category was not selected");
                return;
            }

            List<string> fileslist = lb_tag.Items.Cast<String>().ToList();
            if (selectedSubCategory != null)
                saveTags(fileslist, selectedCategoty.categoryName + "." + selectedSubCategory);
            else
                saveTags(fileslist, selectedCategoty.categoryName + ".");
            MessageBox.Show("Tags added successfuly");

            lb_tag.Items.Clear();
        }

        private void open(object sender, RoutedEventArgs e)
        {

            Controls.TagControl.andOrSelection inputDialog = new Controls.TagControl.andOrSelection(Categories);
            inputDialog.Title = "Set Custom View filters";

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

            }
        }
    }
}