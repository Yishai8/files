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



using System.IO;



using System.Xml.Linq;


namespace WpfApp4.Controls.AddTag
{
    /// <summary>
    /// add new tag category to categories list
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



//  check the selection of the user about which update he wants to do in the categories 
//  update , delete or add and handles according the the selection 
// and announce the user if there is any problem and ask him to fix it and to do the right action.
//check if the added category should be added to main or sub category

   

        private void CheckRadioButton()
        {	
		   
		    XElement ele;
			XDocument doc;
						
			doc = XDocument.Load(@"..\..\Tags\tagCategories.xml");
		    if (quickReportCalendarCheckedRadioButton.Name.ToString()=="NewCat" ) 
			 {
				 if  (mainCat.Text.Length == 0)
				 {
					  mainCat.Focus();
					  MessageBox.Show("Enter the new Category ");
					return;
				 }
				 
				if (categories.Any(p => p.categoryName == mainCat.Text))
				{
					MessageBox.Show("category already exsist , to add subCategory press third Radiobutton");
					return;
				}
				if (subCat.Text.Length == 0)
				{
					MessageBox.Show("Enter the sub Category ");
					return;
					
				}
				
				if  (subCat.Text.Length !=0 )
				 {
					
					 ele = new XElement("HeaderTag",new XAttribute("name",mainCat.Text),new XAttribute("showOldValues","true"),
					      new XElement("ChildTag",new XAttribute("name",subCat.Text)));
				   
					 doc.Root.Add(ele);
					
					 
					 doc.Save(@"..\..\Tags\tagCategories.xml");							

				   
				   categories.Add(new tagsCategory(mainCat.Text,new List<string> {subCat.Text},false));
					MessageBox.Show("category added sucessfully");
				 
                     return;				   
				  }
			 }
				
			
		
				 
				 if (quickReportCalendarCheckedRadioButton.Name.ToString()=="Newsub" ) 
			     {
					
					
					 
			 	     if (cbCats.MySelectedItem==null)
					 {
				 		 
						MessageBox.Show("you have to select category, try again");
                         return;
                     }						 
						 
								
					var  selectedItem1 = (tagsCategory)cbCats.MySelectedItem;
					
					 if ( selectedItem1.categoryName.Length == 0 || newOption.Text.Length ==0 )
				    {
					   MessageBox.Show ("you have to type  subCategory , try again"); 
				       return;
				    }
				 }
				 
			
	  			 
			 
          
			if(quickReportCalendarCheckedRadioButton.Name.ToString()=="NewCat")
            {
				
                if (!categories.Any(p => p.categoryName == mainCat.Text))
				{
					MessageBox.Show("maincat");
					
					if (	subCat.Text.Length == 0)
				     {
						
						MessageBox.Show("you didn't enter subCategory, try again ");
			 			return;
				     }
					
					 ele = new XElement("HeaderTag",new XAttribute("name",mainCat.Text),new XAttribute("showOldValues","true"),
					       new XElement("ChildTag",new XAttribute("name",subCat.Text)));
				   
					 doc.Root.Add(ele);
					
					 
					 doc.Save(@"..\..\Tags\tagCategories.xml");							

				    categories.Add(new tagsCategory(mainCat.Text, new List<string> { subCat.Text }, false));
					MessageBox.Show("category added sucessfully");
				   doc.Save(@"..\..\Tags\tagCategories.xml");		
				}
            }
        
		
			 if(quickReportCalendarCheckedRadioButton.Name.ToString()=="delete")
              {
				
					try
					{
					
					 
			 	     if (cbCats.MySelectedItem==null)
					 {
				 		 
						MessageBox.Show("you have to select category, try again");
                         return;
                     }
					}
					 catch (NullReferenceException ex)
                   {
                Console.WriteLine(ex.InnerException);
				MessageBox.Show("you have not select category you want to delete , try again");
				return;
                   }
			  				  
                  
				  
				var selectedItem2 = (tagsCategory)cbCats.MySelectedItem; 
								
					
					
				// in case of delete the system make it sure with the user before performing the delete
			 
            	string messageBoxText = "Are you sure you want to delete this category and all subcategories " + "?";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                if (MessageBox.Show(messageBoxText, null, button, icon) == MessageBoxResult.No)
                    return;
					
                doc.Element("root").Elements("HeaderTag")  
				
 				.Where(x => x.Attribute("name").Value == selectedItem2.categoryName.ToString()).Remove();
				
				
				doc.Save(@"..\..\Tags\tagCategories.xml");	


				 
				categories.Remove(selectedItem2);
				MessageBox.Show("category deleted sucessfully");
				doc.Save(@"..\..\Tags\tagCategories.xml");		
			}
			
			
			 if (quickReportCalendarCheckedRadioButton.Name.ToString()=="Newsub" ) 
			  {
				if (newOption.Text.Length==0)
				 {
					MessageBox.Show("you didn't enter subCategory, try again");
					return;
				 }
				 
			
				var selectedItem = (tagsCategory)cbCats.MySelectedItem;
			    selectedItem.categoryOptions.Add(newOption.Text);
				bool found1 = false;
				bool found2 = false;
				try
				 {
                  				 
                  found1 =  doc.Element("root").Elements("HeaderTag").Elements("ChildTag").Any(p => p.Parent.Attribute("name").Value == selectedItem.categoryName); //)&& )==newOption.Text);
				
				 }
				
				catch(NullReferenceException e)
				
			   	 {
					  MessageBox.Show("notfound");
			     }
				
				if (found1 && found2)
				  {
					 MessageBox.Show("subCategory already exsist");
                     return;					 
									  
				  }
       
				try
				{				
				 doc.Element("root").Elements("HeaderTag").Elements("ChildTag") 
				 .Where(x => x.Parent.Attribute("name").Value == selectedItem.categoryName).FirstOrDefault()
			 	 .AddAfterSelf(new XElement("ChildTag",new XAttribute("name",newOption.Text)));
				}
				
				catch (NullReferenceException ex)   
				{
				 doc.Element("root").Elements("HeaderTag")   
				 .Where(x => x.Attribute("name").Value == selectedItem.categoryName).FirstOrDefault()
			     .Add(new XElement("ChildTag",new XAttribute("name",newOption.Text)));	
					
				}
				doc.Save(@"..\..\Tags\tagCategories.xml");	
				MessageBox.Show("category updated succesfully");
				 
			  }
			
            }
			
      

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            //disable irrelevant selection options
            if (ck.IsChecked.Value)
                quickReportCalendarCheckedRadioButton = ck;
        }




    }
}
