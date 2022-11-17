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
    /// Interaction logic for RulesWin.xaml
    /// </summary>
    public partial class RulesWin : NavigationWindow
    {
        
        public RulesWin(SolidColorBrush color1, SolidColorBrush color2)
        {
            InitializeComponent();
            ////Rule1 r1 = new Rule1();
            ////r1.C1 = color1;
            ////r1.C2 = color2;
            //this.Source = new Uri("Rule1.xaml", UriKind.Relative);
            ////color1.Color.
            //Rule1 r1 = new Rule1();
            ////SolidColorBrush[] s = new SolidColorBrush[]{ color1, color2 };
            //NavigationService.Navigate(r1, "HEllo world!" );

            this.Navigate(new Rule1(color1, color2));
        }

        private void NavigationWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
