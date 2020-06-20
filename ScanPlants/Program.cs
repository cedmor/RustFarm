using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ScanPlants
{
    class Program
    {
        static string imgtestfolderpath = @"C:\Users\Windows\source\repos\RustFarm\ScanPlants\testimg";
        public static int startPosX = 1562, startPosY = 488, cropWidth = 344, cropHeight = 77,
            screenSizeX = 2560, screenSizeY = 1440;

        //// DEBUG ALL PICTURES
        //public static int startPosX = 0, startPosY = 0, cropWidth = 2560, cropHeight = 1440,
        //            screenSizeX = 2560, screenSizeY = 1440;


        public static int startPosX2 = 1061, startPosY2 = 400, cropWidth2 = 207, cropHeight2 = 26,
            screenSizeX2 = 2560, screenSizeY2 = 1440;
        

        static void Main(string[] args)
        {
            List<Bitmap> images = LoadImagesFromFolder(imgtestfolderpath);
            foreach (var image in images)
            {
                List<Tuple<int[], string>> lettersCoords = GetLettersCoords(image);
                GetPlantsStatsFromLettersCoords(lettersCoords);

            }
        }

        static void GetPlantsStatsFromLettersCoords(List<Tuple<int[], string>> lettersCoords)
        {
            var clusters = GetClustersFromLettersCoords(lettersCoords);
            var letterClusters = GetLetterClustersFromClusters(
                clusters.OrderBy(c => { return c.Average(lc => lc.Item1[0]); }).ToList());

            StringBuilder str = new StringBuilder();
            foreach (var clust in letterClusters)
            {
                str.Append(clust.OrderByDescending(c => { return c.Average(lc => lc.Item1[1]); })
                    .First().GroupBy(lc => lc.Item2).OrderByDescending(x => x.Count()).First().Key);
            }
        }

        static List<List<List<Tuple<int[], string>>>> GetLetterClustersFromClusters(List<List<Tuple<int[], string>>> clusters)
        {
            List<List<List<Tuple<int[], string>>>> letterCluster = new List<List<List<Tuple<int[], string>>>>();
            while (clusters.Count != 0)
            {
                var tempLetterCluster = new List<List<Tuple<int[], string>>>();
                tempLetterCluster.Add(clusters.ElementAt(0));
                clusters.ForEach(c =>
                {
                    if (clusters.ElementAt(0) != c)
                    {
                        if (Math.Abs(clusters.ElementAt(0).Average(lc => lc.Item1[0]) - c.Average(lc => lc.Item1[0])) < 10)
                        {
                            tempLetterCluster.Add(c);
                        }
                    }
                });
                letterCluster.Add(tempLetterCluster);
                tempLetterCluster.ForEach(e => clusters.Remove(e));
            }
            return letterCluster;
        }

        static List<List<Tuple<int[], string>>> GetClustersFromLettersCoords(List<Tuple<int[], string>> lettersCoords)
        {
            List<List<Tuple<int[], string>>> cluster = new List<List<Tuple<int[], string>>>();
            while (lettersCoords!= null && lettersCoords.Count != 0)
            {
                var tempCluster = new List<Tuple<int[], string>>();
                tempCluster.Add(lettersCoords.ElementAt(0));
                lettersCoords.ForEach(lc =>
                {
                    if (lettersCoords.ElementAt(0) != lc)
                    {
                        if (Math.Abs(lettersCoords.ElementAt(0).Item1[0] - lc.Item1[0]) < 10
                        && Math.Abs(lettersCoords.ElementAt(0).Item1[1] - lc.Item1[1]) < 10)
                        {
                            tempCluster.Add(lc);
                        }
                    }
                });
                cluster.Add(tempCluster);
                tempCluster.ForEach(e => lettersCoords.Remove(e));
            }
            return cluster;
        }

        static List<Tuple<int[], string>> GetLettersCoords(Bitmap image)
        {
            int posX = image.Width * startPosX / screenSizeX,
                posY = image.Height * startPosY / screenSizeY,
                sizeX = image.Width * cropWidth / screenSizeX,
                sizeY = image.Height * cropHeight / screenSizeY;

            int posX2 = image.Width * startPosX2 / screenSizeX2,
                posY2 = image.Height * startPosY2 / screenSizeY2,
                sizeX2 = image.Width * cropWidth2 / screenSizeX2,
                sizeY2 = image.Height * cropHeight2 / screenSizeY2;

            var a = image.Clone(new Rectangle(posX, posY, sizeX, sizeY), PixelFormat.Format1bppIndexed);
            var b = image.Clone(new Rectangle(posX2, posY2, sizeX2, sizeY2), PixelFormat.Format1bppIndexed);

            a = a.Clone(new Rectangle(0, 0, a.Width, a.Height), PixelFormat.Format24bppRgb);
            b = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);

            List<Tuple<int[], string>> foundLettersFromPlant = CustomOCR.GetLettersCoordinates(a);
            List<Tuple<int[], string>> foundLettersFromInventory = CustomOCR.GetLettersCoordinates(b);

            if (foundLettersFromPlant.Count > 10 && foundLettersFromPlant.Count < 100)
            {
                DebugFoundLetters(foundLettersFromPlant, a);
                return foundLettersFromPlant;
            }
            else if (foundLettersFromInventory.Count > 10 && foundLettersFromInventory.Count < 100)
            {
                DebugFoundLetters(foundLettersFromInventory, b);
                return foundLettersFromInventory;
            }
            else
            {
                return null;
            }
        }

        static void DebugFoundLetters(List<Tuple<int[], string>> foundLetters, Bitmap image)
        {
            foundLetters.ForEach(l => { if (l.Item2 == "G") image.SetPixel(l.Item1[0], l.Item1[1], Color.Red); });
            foundLetters.ForEach(l => { if (l.Item2 == "H") image.SetPixel(l.Item1[0], l.Item1[1], Color.Green); });
            foundLetters.ForEach(l => { if (l.Item2 == "Y") image.SetPixel(l.Item1[0], l.Item1[1], Color.Blue); });
            foundLetters.ForEach(l => { if (l.Item2 == "W") image.SetPixel(l.Item1[0], l.Item1[1], Color.Cyan); });
            foundLetters.ForEach(l => { if (l.Item2 == "X") image.SetPixel(l.Item1[0], l.Item1[1], Color.Magenta); });
            BitMapExtensions.DisplayBitMap(image);
        }

        static List<Bitmap> LoadImagesFromFolder(string imgtestfolderpath)
        {
            string[] fileEntries = Directory.GetFiles(imgtestfolderpath);
            List<Bitmap> images = new List<Bitmap>();
            foreach (string fileName in fileEntries)
                images.Add(new Bitmap(fileName));
            return images;
        }

    }

}
