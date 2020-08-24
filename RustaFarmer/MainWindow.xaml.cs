using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
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
using Process.NET;
using Process.NET.Memory;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using RustaFarmer.Pages;

namespace RustaFarmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MainPage.Navigate(new Scanning());
        }


        private LowLevelKeyboardListener _listener;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            _listener.HookKeyboard();
        }
        private void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            switch (e.KeyPressed.ToString())
            {
                //case Keys.B: Scan.TakeCapture(); break;
                case "B": ScanPlants.Scan(new Bitmap(new ScreenCapture().CaptureScreen())); break;
                //case Keys.NumPad1: Analysing(); ; break;
                //case Keys.NumPad3: SavePlants(); break;
                //case Keys.NumPad5: ResetPlants(); break;
                //case Keys.NumPad6: LoadPlants(); break;
                //case Keys.NumPad7: DisplayPlants(); break;
                //case Keys.NumPad8: LaunchOverlay(); break;
                //case Keys.NumPad9: System.Environment.Exit(1); break;
                default: break;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.UnHookKeyboard();
        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }


        #region Navigation
        private void Menu_ScanPlants_Click(object sender, RoutedEventArgs e)
        {
            MainPage.NavigationService.Navigate(new Scanning());

        }

        private void Menu_ResultsList_Click(object sender, RoutedEventArgs e)
        {
            MainPage.NavigationService.Navigate(new ResultsList());

        }

        private void Menu_Result_Click(object sender, RoutedEventArgs e)
        {
            MainPage.NavigationService.Navigate(new Result());

        }
        #endregion

        #region TopMenuButtons
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion
    }
}
