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

namespace RustaFarmer.Pages
{
    /// <summary>
    /// Logique d'interaction pour Scanning.xaml
    /// </summary>
    public partial class Scanning : Page
    {
        public Scanning()
        {
            InitializeComponent();
            plantList.ItemsSource = ScanPlants.plants;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ResultsList());
        }

        private void BtnIconAdd_Click(object sender, RoutedEventArgs e)
        {
            ScanPlants.plants.Add(new Plant("GHWWWY"));
            ScanPlants.plants.Add(new Plant("YYGGHH"));
            ScanPlants.plants.Add(new Plant("HGWYXX"));
            ScanPlants.plants.Add(new Plant("GGYHWX"));
            ScanPlants.plants.Add(new Plant("WWYGXH"));
            ScanPlants.plants.Add(new Plant("YHWHYY"));
        }
    }
}
