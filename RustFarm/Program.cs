using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using IronOcr;
using Rust.Farm.RustPlants;
using System.Xml;
using System.Xml.Serialization;
using Rust.Utils;

namespace Rust.Farm
{
    static class Program
    {
        static void Main(string[] args)
        {
            InitConfig();
            Run();
        }

        public static void InitConfig()
        {
            RustFarm.Config.Console cons = new RustFarm.Config.Console();
        }

        public static void Run()
        {
            InterceptKeys.callback = new InterceptKeys.CallBack(MySelector);
            InterceptKeys._hookID = InterceptKeys.SetHook(InterceptKeys._proc);
            Application.Run();
            InterceptKeys.UnhookWindowsHookEx(InterceptKeys._hookID);
        }

        public static bool MySelector(Keys vkCode)
        {
            switch (vkCode)
            {
                case Keys.B: Scan.TakeCapture(); break;
                case Keys.NumPad1: Analysing(); ; break;
                case Keys.NumPad3: SavePlants(); break;
                case Keys.NumPad5: ResetPlants(); break;
                case Keys.NumPad6: LoadPlants(); break;
                case Keys.NumPad7: DisplayPlants(); break;
                case Keys.NumPad9: System.Environment.Exit(1); break;
                default:break;
            }
            return true;
        }

        public static void Analysing()
        {
            Console.WriteLine();
            Console.WriteLine("Launching analysis...");
            Console.WriteLine();

            Analyzer analyzer = new Analyzer();
            analyzer.Execute(Scan.plants, new Plant("GGYYYY"));
        }

        public static void ResetPlants()
        {
            Console.WriteLine();
            Console.WriteLine("Resetting plants...");
            Console.WriteLine();
            Scan.plants = new List<Plant>();
        }

        public static void SavePlants()
        {
            Console.WriteLine();
            Console.WriteLine("Saving plants...");
            Console.WriteLine();

            Serializing.WriteToBinaryFile("save/" + new Random().Next(0, 1000), Scan.plants);
        }

        public static void LoadPlants()
        {
            Console.WriteLine();
            Console.WriteLine("Loading plants...");
            Console.WriteLine();

            var directory = new DirectoryInfo("save");
            var myFile = (from f in directory.GetFiles()
                          orderby f.LastWriteTime descending
                          select f).First();

            var loadedPlants = Serializing.ReadFromBinaryFile<List<Plant>>(myFile.FullName);
            Scan.plants.AddRange(loadedPlants);

            Console.WriteLine(loadedPlants.Count + " plants loaded from : " + myFile.FullName);
        }

        public static void DisplayPlants()
        {
            Console.WriteLine();
            Console.WriteLine("Displaying plants...");
            Console.WriteLine();

            foreach (Plant plant in Scan.plants)
            {
                plant.Print();
            }
        }

    }
}


