using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Process.NET;
using System.Collections.ObjectModel;

namespace RustaFarmer
{
    public class ScanPlants
    {
        public static int startPosX = 1562, startPosY = 488, cropWidth = 344, cropHeight = 77,
            screenSizeX = 2560, screenSizeY = 1440;

        public static int startPosX2 = 1061, startPosY2 = 400, cropWidth2 = 207, cropHeight2 = 26,
            screenSizeX2 = 2560, screenSizeY2 = 1440;

        public static ObservableCollection<Plant> plants = new ObservableCollection<Plant>();

        public static void Scan(Bitmap image)
        {
            List<Tuple<int[], string>> lettersCoords = GetLettersCoords(image);
            var scannedPlant = new Plant(GetPlantsStatsFromLettersCoords(lettersCoords));
            scannedPlant.scanId = Plant.cpt++;
            plants.Add(scannedPlant);
        }

        static string GetPlantsStatsFromLettersCoords(List<Tuple<int[], string>> lettersCoords)
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
            return str.ToString();
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
            while (lettersCoords != null && lettersCoords.Count != 0)
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
                return foundLettersFromPlant;
            }
            else if (foundLettersFromInventory.Count > 10 && foundLettersFromInventory.Count < 100)
            {
                return foundLettersFromInventory;
            }
            else
            {
                return null;
            }
        }
    }

    public static class CustomOCR
    {
        public static List<Tuple<int, int, bool>> GetPixelsCoordinatesRecognitionH()
        {
            List<Tuple<int, int, bool>> pixelsToCheck = new List<Tuple<int, int, bool>>();
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, 0, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, 5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, 0, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, -5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-4, 0, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-4, 5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-4, 0, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-4, -5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-2, 0, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(2, 0, true));

            pixelsToCheck.Add(new Tuple<int, int, bool>(0, 5, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, 5, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(8, 0, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(8, 0, false));

            return pixelsToCheck;
        }
        public static List<Tuple<int, int, bool>> GetPixelsCoordinatesRecognitionG()
        {
            List<Tuple<int, int, bool>> pixelsToCheck = new List<Tuple<int, int, bool>>();
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, 3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, -3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(1, -5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, -7, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(7, -5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(1, 5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, 7, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(8, 2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(3, 0, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-3, 0, false));

            return pixelsToCheck;
        }
        public static List<Tuple<int, int, bool>> GetPixelsCoordinatesRecognitionY()
        {
            List<Tuple<int, int, bool>> pixelsToCheck = new List<Tuple<int, int, bool>>();
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, 5, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-1, -3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(1, -3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-3, -6, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(3, -6, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, -2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(7, 0, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(9, 0, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-7, 0, false));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-9, 0, false));
            return pixelsToCheck;
        }
        public static List<Tuple<int, int, bool>> GetPixelsCoordinatesRecognitionW()
        {
            List<Tuple<int, int, bool>> pixelsToCheck = new List<Tuple<int, int, bool>>();
            pixelsToCheck.Add(new Tuple<int, int, bool>(5, -1, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-5, -1, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, 3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(0, -3, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(3, 8, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-3, 8, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(4, 4, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-4, 4, true));
            return pixelsToCheck;
        }
        public static List<Tuple<int, int, bool>> GetPixelsCoordinatesRecognitionX()
        {
            List<Tuple<int, int, bool>> pixelsToCheck = new List<Tuple<int, int, bool>>();
            pixelsToCheck.Add(new Tuple<int, int, bool>(1, 2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(2, 4, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(3, 6, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-1, 2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-2, 4, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-3, 6, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(1, -2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(2, -4, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(3, -6, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-1, -2, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-2, -4, true));
            pixelsToCheck.Add(new Tuple<int, int, bool>(-3, -6, true));
            return pixelsToCheck;
        }

        public static List<Tuple<int[], string>> GetLettersCoordinates(Bitmap image)
        {
            List<Tuple<int[], string>> letters = new List<Tuple<int[], string>>();
            for (int j = image.Width - 1; j >= 0; j--)
            {
                for (int i = image.Height - 1; i >= 0; i--)
                {
                    if (image.GetPixel(j, i).Name == "ffffffff")
                    {
                        foreach (var letter in new string[] { "G", "Y", "H", "W", "X" })
                        {
                            if (DoesSpecificLetterExist(image, j, i, letter))
                                letters.Add(new Tuple<int[], string>(new int[] { j, i }, letter));
                        }
                    }
                }
            }
            return letters;
        }

        public static bool DoesSpecificLetterExist(Bitmap image, int j, int i, string letter)
        {
            List<Tuple<int, int, bool>> pixelsToCheck;
            switch (letter)
            {
                case "G": pixelsToCheck = GetPixelsCoordinatesRecognitionG(); break;
                case "Y": pixelsToCheck = GetPixelsCoordinatesRecognitionY(); break;
                case "H": pixelsToCheck = GetPixelsCoordinatesRecognitionH(); break;
                case "W": pixelsToCheck = GetPixelsCoordinatesRecognitionW(); break;
                case "X": pixelsToCheck = GetPixelsCoordinatesRecognitionX(); break;
                default: return false;
            }
            var allWhite = false;
            foreach (var offset in pixelsToCheck)
            {
                if (j + offset.Item1 < image.Width && i + offset.Item2 < image.Height && j + offset.Item1 > 0 && i + offset.Item2 > 0)
                {
                    if (!(image.GetPixel(j + offset.Item1, i + offset.Item2).Name == "ffffffff" && offset.Item3 ||
                        image.GetPixel(j + offset.Item1, i + offset.Item2).Name != "ffffffff" && !offset.Item3))
                    {
                        allWhite = false;
                        break;
                    }
                    allWhite = true;
                }
            }
            return allWhite;
        }
    }

}
