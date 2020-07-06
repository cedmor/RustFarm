using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            var myHeader = new ScanningPlants.header();
            this.header.Navigate(myHeader);

            var myBody = new ScanningPlants.body();
            this.body.Navigate(myBody);

            var myFooter = new ScanningPlants.footer();
            this.footer.Navigate(myFooter);


        }
    }
}
