using System;
using System.Collections.Generic;
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

namespace WpfApp4.Controls.AddTag
{
    /// <summary>
    /// Interaction logic for AddNewTag.xaml
    /// </summary>
    public partial class AddNewTag : Window
    {
        public AddNewTag()
        {
            InitializeComponent();
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

           
        }
    }
}
