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
using System.Windows.Shapes;
using WpfApp4.Tags;

namespace WpfApp4.Controls.TagControl
{
    /// <summary>
    /// Interaction logic for inputMessage.xaml
    /// </summary>
    public partial class andOrSelection : Window
    {

        public andOrSelection(ObservableCollection<tagsCategory> cats)
        {
            InitializeComponent();
            x.ItemsSource = cats;

        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void addParamtoLV(object sender, RoutedEventArgs e)
        {

            string paramsSelection = x.SelectedValue.ToString() + "." + x1.SelectedValue.ToString();
            if(!paramLV.Items.Contains(paramsSelection))
            paramLV.Items.Add(paramsSelection);
        }
    }
}
