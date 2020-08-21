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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustaFarmer.ScanningPlants
{
    /// <summary>
    /// Interaction logic for header.xaml
    /// </summary>
    public partial class header : Page
    {
        public header()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new ResultsList.header());
            // overloading NavigateTo


            Page page = this;
            Page anotherPage = new ResultsList.header();


            Frame pageFrame = null;
            DependencyObject currParent = VisualTreeHelper.GetParent(page);

            while (currParent != null && pageFrame == null)
            {
                pageFrame = currParent as Frame;
                currParent = VisualTreeHelper.GetParent(currParent);
            }

            // Change the page of the frame.
            if (pageFrame != null)
            {
                pageFrame.Navigate(anotherPage);
            }

            //Frame pageFrame = null;
            //MainWindow currParent = (MainWindow)VisualTreeHelper.GetParent(this);
            //currParent.header.Navigate(new ResultsList.header());
            //currParent.body.Navigate(new ResultsList.body());
            //currParent.footer.Navigate(new ResultsList.footer());

            //Frame pageFrame = null;
            //DependencyObject currParent = VisualTreeHelper.GetParent(this);
            //while (currParent != null && pageFrame == null)
            //{
            //    pageFrame = currParent as Frame;
            //    currParent = VisualTreeHelper.GetParent(currParent);
            //}

            //// Change the page of the frame.
            //if (pageFrame != null)
            //{
            //    pageFrame.Source = new ResultsList.header();
            //}



            //ResultsList.header header = new ResultsList.header();
            //((MainWindow)this.Parent).header.Navigate(header);

            //ResultsList.body body = new ResultsList.body();
            //((MainWindow)this.Parent).body.Navigate(body);

            //ResultsList.footer footer = new ResultsList.footer();
            //((MainWindow)this.Parent).footer.Navigate(footer);


        }
    }
}
