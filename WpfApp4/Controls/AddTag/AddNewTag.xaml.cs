using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp4.Tags;

namespace WpfApp4.Controls.AddTag
{
    /// <summary>
    /// Interaction logic for AddNewTag.xaml
    /// </summary>
    public partial class AddNewTag : Window
    {
        private RadioButton quickReportCalendarCheckedRadioButton;
        ObservableCollection<tagsCategory> categories;
        public AddNewTag(ObservableCollection<tagsCategory> cats)
        {
            InitializeComponent();
            cbCats.MyItemsSource = cats;
            categories = cats;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            CheckRadioButton();
            this.DialogResult = true;

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void CheckRadioButton()
        {
            if(quickReportCalendarCheckedRadioButton.Content.ToString()=="New")
            {
                if (!categories.Any(p => p.categoryName == mainCat.Text))
                    categories.Add(new tagsCategory(mainCat.Text, new List<string> { subCat.Text }, false));
            }
            else
            {
                var selectedItem = (tagsCategory)cbCats.MySelectedItem;
                selectedItem.categoryOptions.Add(newOption.Text);
            }
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            if (ck.IsChecked.Value)
                quickReportCalendarCheckedRadioButton = ck;
        }




    }
}
