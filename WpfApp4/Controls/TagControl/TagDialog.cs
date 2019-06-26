using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WpfApp4.Tags;

namespace WpfApp4.Controls.TagControl
{
    public partial class TagDialog : Window
    {
        

        public TagDialog(ObservableCollection<tagsCategory> cats)
        {
            InitializeComponent();
            lb.ItemsSource = cats;
            
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategoty = lb.SelectedValue as Tags.tagsCategory;
            if (selectedCategoty == null)
            {
                MessageBox.Show("Main category was not selected");
                return;
            }
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
           
        }

        
    }
}