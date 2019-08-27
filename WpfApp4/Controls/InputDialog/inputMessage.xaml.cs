using System;
using System.Windows;

namespace WpfApp4.Controls.InputDialog
{
    public partial class inputMessage : Window
    {
        //add view name
        public inputMessage(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtAnswer.Text != string.Empty)
                this.DialogResult = true;
            else
                lbErr.Content = "Name can't be Empty";
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }
    }
}