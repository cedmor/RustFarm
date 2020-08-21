using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
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
    /// Interaction logic for body.xaml
    /// </summary>
    public partial class body : Page
    {
        int plantNumber = 0;
        //ObservableCollection<Plant> plantListItem = new ObservableCollection<Plant>(ScanPlants.plants);

        public body()
        {
            InitializeComponent();
            plantList.ItemsSource = ScanPlants.plants;
        }

        public void AddPlantToList(string plants)
        {
            Grid geneList = new Grid();
            geneList.HorizontalAlignment = HorizontalAlignment.Center;
            geneList.VerticalAlignment = VerticalAlignment.Center;

            ColumnDefinition scanNumberColDef = new ColumnDefinition();
            ColumnDefinition firstGeneColDef = new ColumnDefinition();
            ColumnDefinition secondGeneColDef = new ColumnDefinition();
            ColumnDefinition thirdGeneColDef = new ColumnDefinition();
            ColumnDefinition fourthGeneColDef = new ColumnDefinition();
            ColumnDefinition fifthGeneColDef = new ColumnDefinition();
            ColumnDefinition sixthGeneColDef = new ColumnDefinition();
            geneList.ColumnDefinitions.Add(scanNumberColDef);
            geneList.ColumnDefinitions.Add(firstGeneColDef);
            geneList.ColumnDefinitions.Add(secondGeneColDef);
            geneList.ColumnDefinitions.Add(thirdGeneColDef);
            geneList.ColumnDefinitions.Add(fourthGeneColDef);
            geneList.ColumnDefinitions.Add(fifthGeneColDef);
            geneList.ColumnDefinitions.Add(sixthGeneColDef);

            var scanNumber = new Label();
            scanNumber.Content = ++plantNumber + ":";
            scanNumber.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(scanNumber, 0);
            geneList.Children.Add(scanNumber);

            var cpt = 0;
            foreach (var plant in plants)
            {
                var packIcon = new PackIcon();
                Grid.SetColumn(packIcon, ++cpt);
                packIcon.Height = 30;
                packIcon.Width = 30;

                switch (plant)
                {
                    case 'Y':
                        packIcon.Kind = PackIconKind.AlphaYCircle;
                        packIcon.Foreground = this.FindResource("PrimaryHueMidBrush") as SolidColorBrush;
                        break;
                    case 'G':
                        packIcon.Kind = PackIconKind.AlphaGCircle;
                        packIcon.Foreground = this.FindResource("PrimaryHueMidBrush") as SolidColorBrush;
                        break;
                    case 'H':
                        packIcon.Kind = PackIconKind.AlphaHCircle;
                        packIcon.Foreground = this.FindResource("PrimaryHueMidBrush") as SolidColorBrush;
                        break;
                    case 'W':
                        packIcon.Kind = PackIconKind.AlphaWCircle;
                        packIcon.Foreground = this.FindResource("SecondaryHueMidBrush") as SolidColorBrush;
                        break;
                    case 'X':
                        packIcon.Kind = PackIconKind.AlphaXCircle;
                        packIcon.Foreground = this.FindResource("SecondaryHueMidBrush") as SolidColorBrush;
                        break;
                    default:
                        break;
                }
                geneList.Children.Add(packIcon);
            }
            this.plantList.Items.Add(geneList);
        }

    }
}
