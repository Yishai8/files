using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfApp4.Controls.CustomComboBox
{
    /// <summary>
    /// custom combobox with default "select an option" text and dependant properties
    /// </summary>
    public partial class CustomComboBox : UserControl
    {
        public CustomComboBox()
        {
            InitializeComponent();
        }
        public static DependencyProperty DefaultTextProperty =
             DependencyProperty.Register("DefaultText", typeof(string), typeof(CustomComboBox));

        public static DependencyProperty MySelectedValuePathProperty =
             DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(CustomComboBox));

        public static DependencyProperty MyTextProperty =
            DependencyProperty.Register("MyText", typeof(string), typeof(CustomComboBox));


        public static DependencyProperty MyItemsSourceProperty =
            DependencyProperty.Register("MyItemsSource", typeof(IEnumerable), typeof(CustomComboBox));

        public static DependencyProperty MySelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomComboBox));

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        public string SelectedValuePath
        {
            get { return (string)GetValue(MySelectedValuePathProperty); }
            set { SetValue(MySelectedValuePathProperty, value); }
        }

        public string MyText
        {
            get { return (string)GetValue(MyTextProperty); }
            set { SetValue(MyTextProperty, value); }
        }

        public IEnumerable MyItemsSource
        {
            get { return (IEnumerable)GetValue(MyItemsSourceProperty); }
            set { SetValue(MyItemsSourceProperty, value); }
        }

        public object MySelectedItem
        {
            get { return GetValue(MySelectedItemProperty); }
            set { SetValue(MySelectedItemProperty, value); }
        }
    }

    
}