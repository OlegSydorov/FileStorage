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

namespace FileStorage_Client
{
    /// <summary>
    /// Interaction logic for Rule2.xaml
    /// </summary>
    public partial class Rule2 : Page
    {
        SolidColorBrush c1;
        SolidColorBrush c2;
        public Rule2(SolidColorBrush color1, SolidColorBrush color2)
        {
            InitializeComponent();
            c1 = color1;
            c2 = color2;
            txtTb.Text = "     *   To upload files to storage:" +
                "\r\n (1) drag them to the tray," +
                "\r\n (2) click UPLOAD SELECTED FILES TO STORAGE button" +
                         "\r\n     *   Use DELETE FILE context menu option to delete files from tray" +
                         "\r\n     *   Use CLEAR TRAY tray view context menu option to delete all files from tray";
            mainBorder.Background = color2;
            mainBorder.BorderBrush = color1;

            closeButton.Background = color1;
            closeButton.Foreground = color2;

            back.Fill = color1;
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
            NavigationService nav;
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Rule1(c1, c2));

        }
        private void Forward_MouseDown(object sender, RoutedEventArgs e)
        {

            NavigationService nav;
            nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Rule3(c1, c2));
        }

       
    }
}
