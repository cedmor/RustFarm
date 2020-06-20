using Rust.Utils;
using Rust.Farm.RustPlants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace Rust.Farm
{

    public class Scan
    {
        // Config for - UI Scale : 1.0
        public static int startPosX = 1562, startPosY = 488, cropHeight = 77, cropWidth = 344;
        public static List<int> lines = new List<int>() { 0, 40 };
        public static List<int> cols = new List<int>() { 0, 60, 122, 184, 246, 307 };
        public static int[] offsets = new int[4] { 6, 6, 24, 24 };

        public static List<Plant> plants = new List<Plant>();
        public static string notSureString = "%notsure%";

        public static void TakeCapture()
        {
            //// WORKING EXAMPLE
            //Rectangle rect = new Rectangle(startPosX, startPosY, cropWidth, cropHeight);
            //Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            //Graphics g = Graphics.FromImage(bmp);
            //g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            //BitMapExtensions.DisplayBitMap(new Bitmap(bmp));



            ScanPlants.Scan(new Bitmap(new ScreenCapture().CaptureScreen()));
            return;

            Bitmap plantedPlantStats = cropImage(new ScreenCapture().CaptureScreen(), new Rectangle(startPosX, startPosY, cropWidth, cropHeight));
            string[,] statsMatrix = new string[2, 6];
            foreach (var line in lines)
            {
                foreach (var col in cols)
                {
                    Bitmap singleStat = cropImage(plantedPlantStats, new Rectangle(col + offsets[0], line + offsets[1], offsets[2], offsets[3]));
                    if (DoesImageHasWhite(singleStat))
                    {
                        statsMatrix[lines.IndexOf(line), cols.IndexOf(col)] = TransformCommonWrongValue(ExecuteOCR(singleStat));
                        if (isError(singleStat, statsMatrix[lines.IndexOf(line), cols.IndexOf(col)], 1))
                        {
                            singleStat = resizeImage(singleStat, new Size(50, 30));
                            statsMatrix[lines.IndexOf(line), cols.IndexOf(col)] = TransformCommonWrongValue(ExecuteOCR(singleStat));
                            isError(singleStat, statsMatrix[lines.IndexOf(line), cols.IndexOf(col)], 2);
                        }
                        //singleStat.Save("ress /planted/" + " lin" + (lines.IndexOf(line) + 1) + "col" + (cols.IndexOf(col) + 1) + ".png");
                    }
                    else
                    {
                        statsMatrix[lines.IndexOf(line), cols.IndexOf(col)] = "NotWhite";
                    }
                }
            }
            //PrintGenes(statsMatrix);
            plants.Add(new Plant(statsMatrix));
        }

        public static bool isError(Bitmap image, string value, int depth)
        {
            switch (value)
            {
                case "X":
                case "W":
                case "G":
                case "Y":
                case "H":
                    return false;
                default:
                    if (depth > 1)
                    {
                        image.Save("ress/planted/" + "E" + depth + "-" + new Random().Next(999) + " - '" + MakeValidFileName(value) + "'.png");
                    }
                    return true;
            }
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public static string ExecuteOCR(Bitmap image)
        {
            try
            {
                var Ocr = new AdvancedOcr()
                {
                    CleanBackgroundNoise = false,
                    EnhanceContrast = false,
                    EnhanceResolution = false,
                    Language = IronOcr.Languages.English.OcrLanguagePack,
                    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Fast,
                    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                    DetectWhiteTextOnDarkBackgrounds = false,
                    InputImageType = AdvancedOcr.InputTypes.Snippet,
                    RotateAndStraighten = false,
                    ReadBarCodes = false,
                    ColorDepth = 2
                };
                return Ocr.Read(image).Text;
            }
            catch (Exception)
            {
                Console.WriteLine("Trial popUp, please rescan your plants.");
                return string.Empty;
            }
        }

        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        public static string TransformCommonWrongValue(string value)
        {
            switch (value)
            {
                case "X":
                case "W":
                case "G":
                case "Y":
                case "H":
                    return value;
                case "V":
                    return notSureString + "Y";
                case "":
                    return notSureString + "W";
                default:
                    return value;
            }
        }

        public static bool DoesImageHasWhite(Bitmap image)
        {

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    var pixel = image.GetPixel(j, i);
                    if (pixel.R >= 150 && pixel.R < 170 &&
                        pixel.G >= 150 && pixel.G < 170 &&
                        pixel.B >= 150 && pixel.B < 170)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private static Bitmap cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

    }

    public class ScanPlants
    {
        static string imgtestfolderpath = @"C:\Users\Windows\source\repos\RustFarm\ScanPlants\testimg";
        public static int startPosX = 1562, startPosY = 488, cropWidth = 344, cropHeight = 77,
            screenSizeX = 2560, screenSizeY = 1440;

        public static int startPosX2 = 1061, startPosY2 = 400, cropWidth2 = 207, cropHeight2 = 26,
            screenSizeX2 = 2560, screenSizeY2 = 1440;


        public static void Scan(Bitmap image)
        {
            //List<Bitmap> images = LoadImagesFromFolder(imgtestfolderpath);
            //foreach (var image in images)
            //{
            List<Tuple<int[], string>> lettersCoords = GetLettersCoords(image);
            Console.WriteLine(GetPlantsStatsFromLettersCoords(lettersCoords));
            //}
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

            DebugFoundLetters(foundLettersFromPlant, a);

            if (foundLettersFromPlant.Count > 10 && foundLettersFromPlant.Count < 100)
            {
                //DebugFoundLetters(foundLettersFromPlant, a);
                return foundLettersFromPlant;
            }
            else if (foundLettersFromInventory.Count > 10 && foundLettersFromInventory.Count < 100)
            {
                //DebugFoundLetters(foundLettersFromInventory, b);
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
    public static class BitMapExtensions
    {
        public static void DisplayBitMap(Bitmap bitmap)
        {
            var tempFileName = Path.GetTempFileName();
            File.WriteAllBytes(tempFileName, bitmap.ToByteArray(ImageFormat.Bmp));

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var psi = new ProcessStartInfo(
                "rundll32.exe",
                String.Format(
                    "\"{0}{1}\", ImageView_Fullscreen {2}",
                    Environment.Is64BitOperatingSystem ?
                        path.Replace(" (x86)", "") : path,
                    @"\Windows Photo Viewer\PhotoViewer.dll",
                    tempFileName)
                );
            psi.UseShellExecute = false;

            var viewer = Process.Start(psi);
            viewer.EnableRaisingEvents = true;
            viewer.Exited += (o, args) => { File.Delete(tempFileName); };
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
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
