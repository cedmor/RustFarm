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
            ScanPlants.plants.Add(new Plant("YXWHXG"));
            ScanPlants.plants.Add(new Plant("GGGHXY"));
            ScanPlants.plants.Add(new Plant("YGXXWW"));
            ScanPlants.plants.Add(new Plant("YGHHHH"));
            ScanPlants.plants.Add(new Plant("WWYGGH"));
            ScanPlants.plants.Add(new Plant("WXXYHW"));
            ScanPlants.plants.Add(new Plant("GYWGHX"));
            ScanPlants.plants.Add(new Plant("WGGWHX"));
            ScanPlants.plants.Add(new Plant("GXHHGX"));
            ScanPlants.plants.Add(new Plant("WXWXXX"));
            ScanPlants.plants.Add(new Plant("HXXHXH"));
            ScanPlants.plants.Add(new Plant("HYHHYX"));
            ScanPlants.plants.Add(new Plant("GWYYHX"));
            ScanPlants.plants.Add(new Plant("YXYYHW"));
            ScanPlants.plants.Add(new Plant("GHYWYW"));
            ScanPlants.plants.Add(new Plant("GXXGWX"));
            ScanPlants.plants.Add(new Plant("HGHXYX"));
            ScanPlants.plants.Add(new Plant("GHXWXG"));
            ScanPlants.plants.Add(new Plant("XHGGGG"));
            ScanPlants.plants.Add(new Plant("WWXHHX"));
            ScanPlants.plants.Add(new Plant("YYHYHX"));
            ScanPlants.plants.Add(new Plant("WHYWYX"));
            ScanPlants.plants.Add(new Plant("YYHYGY"));
            ScanPlants.plants.Add(new Plant("GXHWGX"));
            ScanPlants.plants.Add(new Plant("XHWHGY"));
            ScanPlants.plants.Add(new Plant("XGWHWY"));
        }
    }
}
