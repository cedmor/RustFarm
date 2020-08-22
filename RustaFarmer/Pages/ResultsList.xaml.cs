using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustaFarmer.Pages
{
    /// <summary>
    /// Logique d'interaction pour ResultsList.xaml
    /// </summary>
    public partial class ResultsList : Page
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        public static ObservableCollection<Plant> resultsPlants = new ObservableCollection<Plant>();

        public ResultsList()
        {
            InitializeComponent();
            resultsList.ItemsSource = resultsPlants;

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Scanning());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnIconAdd_Click(object sender, RoutedEventArgs e)
        {
            resultsPlants.Clear();
            backgroundWorker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Analyzer analyzer = new Analyzer();
            e.Result = analyzer.Execute(ScanPlants.plants.ToList(), new Plant("GGYYYY"), backgroundWorker).resultPlants;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            textBlock.Text = e.ProgressPercentage.ToString();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This is called on the UI thread when the DoWork method completes
            // so it's a good place to hide busy indicators, or put clean up code
            textBlock.Text = "I'm done working!";
            resultsPlants.Clear();
            ((ObservableCollection<Plant>)e.Result).ToList().ForEach(e => resultsPlants.Add(e));
        }

        private void resultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultsList.SelectedItem != null)
                this.NavigationService.Navigate(new Result((Plant)resultsList.SelectedItem));

        }
    }
}
