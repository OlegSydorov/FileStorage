using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace FileStorage_Client
{
    /// <summary>
    /// Interaction logic for Rule1.xaml
    /// </summary>
    public partial class Rule1 : Page
    {
        SolidColorBrush c1;
        SolidColorBrush c2;

        public Rule1(SolidColorBrush color1, SolidColorBrush color2)
        {
            InitializeComponent();
            c1 = color1;
            c2 = color2;
            txtTb.Text = "     *   To view storage - log in and push VIEW STORAGE button."+
                         "\r\n     *   Use context menu to delete files or folders, open folders or move files."+
                         "\r\n      *   To move file:" +
                         "\r\n(1) select CUT FILE option from context menu," +
                      "\r\n(2) open target folder," +
                      "\r\n(3) select PASTE FILE option from context menu";

            mainBorder.Background = color2;
            mainBorder.BorderBrush = color1;
            
            closeButton.Background = color1;
            closeButton.Foreground = color2;            
          
            forward.Fill = color1;
            
            headerTb.Foreground = color1;
            txtTb.Foreground = color1;
      
        }

    
       

        private void Close_ButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(null);
        }

        private void Back_MouseDown(object sender, RoutedEventArgs e)
        {


        }
        private void Forward_MouseDown(object sender, RoutedEventArgs e)
        {

            NavigationService nav;
            nav = NavigationService.GetNavigationService(this);
          
            nav.Navigate(new Rule2 (c1, c2));
        }
      
    }
}
