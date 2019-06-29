using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;
using WpfApp4.Tags;

namespace WpfApp4.Controls.TagControl
{
    /// <summary>
    /// Interaction logic for inputMessage.xaml
    /// </summary>
    public partial class andOrSelection : Window
    {
        ObservableCollection<string> LVFilterTags = new ObservableCollection<string>() ;
        List<string> tags = new List<string>();
        string mainCatText = "CategoryName=";
        string subCatText = "Value=";

        public andOrSelection(ObservableCollection<tagsCategory> cats)
        {
            InitializeComponent();
            LVFilterTags.CollectionChanged += this.OnCollectionChanged;
            x.ItemsSource = cats;
            paramLV.ItemsSource = LVFilterTags;

        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            paramLV.ItemsSource = tags;
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void addParamtoLV(object sender, RoutedEventArgs e)
        {
            
            string paramsSelection = mainCatText + x.SelectedValue.ToString() +","+ subCatText + x1.SelectedValue.ToString();
            if (!LVFilterTags.Contains(paramsSelection))
            {
                LVFilterTags.Add(paramsSelection);
                tags.Add(x.SelectedValue.ToString() + "." + x1.SelectedValue.ToString());
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Tags.TagFilter> list1 = new List<Tags.TagFilter>();

            foreach(string param in (ObservableCollection<string>)sender)
            {
                var list = param.Split(',');
                list[0] = list[0].Replace(mainCatText, string.Empty);
                list[1] = list[1].Replace(subCatText, string.Empty);
                list1.Add(new Tags.TagFilter(list[0], list[1]));
            }

            var tagsOrder = (from listItem in list1
                             group listItem by listItem.path into g
                             select Merge(g)
      ).ToList();

            string andOrLabel=string.Empty;
            bool isFirst = true;
            foreach(TagFilter tf in tagsOrder)
            {
                var l = tf.FileTag.Split(';').ToList();
                if (l.Count() == 1)
                {
                    if (isFirst)
                        andOrLabel += tf.path + "=" + l[0];
                    else
                        andOrLabel += " AND " + tf.path + "=" + l[0];
                }
                else
                {
                    foreach(string item in l)
                    {
                        if (isFirst)
                        {
                            andOrLabel += tf.path + "=" + item;
                            isFirst = false;
                        }
                            
                        else
                            andOrLabel += " AND " + tf.path + "=" + item;
                    }
                }
                    

                if(isFirst)
                    isFirst=false;
            }
            paramsText.Text = andOrLabel;



        }

        private Tags.TagFilter Merge( IEnumerable<Tags.TagFilter> paths)
        {
            Tags.TagFilter u = new Tags.TagFilter("", "");
            if (!paths.Any())
            {
                return u;
            }
            else
            {
                u.path = paths.First().path;
                u.FileTag = string.Join(";", paths.Select(x => x.FileTag));
                return u;
            }
        }

    }
}
