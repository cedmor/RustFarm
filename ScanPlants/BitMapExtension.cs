using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace ScanPlants
{
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
