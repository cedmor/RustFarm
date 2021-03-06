﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour Result.xaml
    /// </summary>
    public partial class Result : Page
    {
        public static ObservableCollection<Plant> resultPlantList = new ObservableCollection<Plant>();
        public static Plant resultPlant;

        public Result()
        {
            InitializeComponent();
            result.ItemsSource = resultPlantList;

            if (resultPlantList.Count == 0 && resultPlant != null)
            {
                AddPlantsToResultPlantList(resultPlant);
            }
        }

        public void AddPlantsToResultPlantList(Plant plant, string offset = "")
        {
            resultPlantList.Add(plant);
            offset = offset += "\t";
            foreach (var p in plant.breededFrom)
            {
                AddPlantsToResultPlantList(p, offset);
            }
        }
    }
}
