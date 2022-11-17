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
    /// Interaction logic for Rule3.xaml
    /// </summary>
    public partial class Rule3 : Page
    {

        SolidColorBrush c1;
        SolidColorBrush c2;
        public Rule3(SolidColorBrush color1, SolidColorBrush color2)
        {
            InitializeComponent();
            c1 = color1;
            c2 = color2;
            txtTb.Text = "     *   To download files from storage:" +
                "\r\n (1) drag them - one by one - from the storage view to the tray;" +
                "\r\n (2) select folder for downloading by clicking SELECT FOLDER button;" +
                "\r\n (3) download files to the selected folder by clicking DOWNLOAD SELECTED FILES button" +
                         "\r\n     *   Use storage view context menu to open folders in the storage" +
                         "\r\n     *   Use DELETE FILE context menu option to delete files from tray" +
                         "\r\n     *   Use CLEAR TRAY context menu option to delete all files from tray";
            mainBorder.Background = color2;
            mainBorder.BorderBrush = color1;

            closeButton.Background = color1;
            closeButton.Foreground = color2;

            back.Fill = color1;
            //forward.Fill = color1;

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
            nav.Navigate(new Rule2(c1, c2));

        }
        private void Forward_MouseDown(object sender, RoutedEventArgs e)
        {


        }



    }
}
