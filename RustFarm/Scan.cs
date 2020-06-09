using Rust.Utils;
using Rust.Farm.RustPlants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronOcr;

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
                        //singleStat.Save("ress/planted/" + " lin" + (lines.IndexOf(line) + 1) + "col" + (cols.IndexOf(col) + 1) + ".png");
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
}
