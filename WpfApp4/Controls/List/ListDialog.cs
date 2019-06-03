using System;
using System.Collections.Generic;
using System.Windows;

namespace WpfApp4.Controls.List
{
    public partial class ListMessage : Window
    {
        

        public ListMessage(List<string> list)
        {
            InitializeComponent();
            lblQuestion.ItemsSource = list;
            //txtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
           
        }

        
    }
}