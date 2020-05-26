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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using RustFarm.RustPlants;
using System.Xml;
using System.Xml.Serialization;
using RustFarm.CodeHelper;
using Rust.Utils;

namespace RustFarm
{
    static class Program
    {
        // Config for - UI Scale : 1.0
        public static int startPosX = 1562, startPosY = 488, cropHeight = 77, cropWidth = 344;
        public static List<int> lines = new List<int>() { 0, 40 };
        public static List<int> cols = new List<int>() { 0, 60, 122, 184, 246, 307 };
        public static int[] offsets = new int[4] { 6, 6, 24, 24 };
        public static List<Plant> plants = new List<Plant>();



        const int SWP_NOSIZE = 0x0001;


        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


        static void Main(string[] args)
        {
            int xpos = -668;
            int ypos = 0;
            SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
            Console.SetWindowSize(82, 73);

            var handle = InterceptKeys.GetConsoleWindow();
            InterceptKeys._hookID = InterceptKeys.SetHook(InterceptKeys._proc);

            Application.Run();
            InterceptKeys.UnhookWindowsHookEx(InterceptKeys._hookID);
        }

        public static void Analysing()
        {
            Console.WriteLine();
            Console.WriteLine("Launching analysis...");
            Console.WriteLine();

            Analyzer analyzer = new Analyzer();
            analyzer.Execute(plants, new Plant("GGGGYY"));
        }

        public static void ResetPlants()
        {
            Console.WriteLine();
            Console.WriteLine("Resetting plants...");
            Console.WriteLine();
            plants = new List<Plant>();
        }

        public static void SavePlants()
        {
            Console.WriteLine();
            Console.WriteLine("Saving plants...");
            Console.WriteLine();

            Serializing.WriteToBinaryFile("save/" + new Random().Next(0, 1000), plants);
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
            plants.AddRange(loadedPlants);

            Console.WriteLine(loadedPlants.Count + " plants loaded from : " + myFile.FullName);
        }

        public static void DisplayPlants()
        {
            Console.WriteLine();
            Console.WriteLine("Displaying plants...");
            Console.WriteLine();

            foreach(Plant plant in plants)
            {
                plant.Print();
            }
        }





        public static void WriteInExcel()
        {
            //using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("mytest.xlsx", SpreadsheetDocumentType.Workbook))
            //{
            //    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            //    workbookpart.Workbook = new Workbook();
            //    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            //    SheetData sheetData = new SheetData();
            //    worksheetPart.Worksheet = new Worksheet(sheetData);
            //    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            //    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "plants" };

            //    uint rowIndex = 1;
            //    sheetData.Append(CreateRow(new string[] { "Plant Number", "Gene 1", "Gene 2", "Gene 3", "Gene 4", "Gene 5", "Gene 6" }, rowIndex++));
            //    foreach (var plant in plants)
            //    {
            //        string[] values = new string[7];
            //        values[0] = ((int)rowIndex - 1).ToString();
            //        for (int i = 0; i < (plant.Length / 2); i++)
            //        {
            //            values[i + 1] = string.IsNullOrEmpty(plant[0, i]) ? plant[1, i] : plant[0, i];
            //        }
            //        sheetData.Append(CreateRow(values, rowIndex++));
            //    }

            //    sheets.Append(sheet);
            //    workbookpart.Workbook.Save();
            //    spreadsheetDocument.Close();
            //}
        }

        public static Row CreateRow(string[] values, uint rowIndex)
        {
            Row row = new Row() { RowIndex = rowIndex };
            char headerId = 'A';
            foreach (var value in values)
            {
                Cell cell = new Cell() { CellReference = (headerId++).ToString() + rowIndex.ToString(), CellValue = new CellValue(value), DataType = CellValues.String };
                row.Append(cell);
            }
            return row;
        }


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
                //var Ocr = new AdvancedOcr()
                //{
                //    CleanBackgroundNoise = false,
                //    EnhanceContrast = false,
                //    EnhanceResolution = false,
                //    Language = IronOcr.Languages.English.OcrLanguagePack,
                //    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Fast,
                //    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                //    DetectWhiteTextOnDarkBackgrounds = false,
                //    InputImageType = AdvancedOcr.InputTypes.Snippet,
                //    RotateAndStraighten = false,
                //    ReadBarCodes = false,
                //    ColorDepth = 32
                //};

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
                    //Console.WriteLine("Transformed '" + value + "' into W");
                    return "Y";
                case "":
                    //Console.WriteLine("Transformed '" + value + "' into W");
                    return "W";
                default:
                    return value;
            }
        }

        public static void PrintGenes(string[,] genes)
        {
            for (int i = 0; i < genes.GetLength(0); i++)
            {
                for (int j = 0; j < genes.GetLength(1); j++)
                {
                    var value = genes[i, j];
                    switch (value)
                    {
                        case "X":
                        case "W":
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                            break;
                        case "G":
                        case "Y":
                            Console.BackgroundColor = ConsoleColor.Green;
                            break;
                        case "H":
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            break;
                        case "NotWhite":
                            //Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            genes[i, j] = string.Empty;
                            value = string.Empty;
                            break;
                        default:
                            Console.BackgroundColor = ConsoleColor.DarkMagenta;
                            break;
                    }
                    Console.Write(value + "\t");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
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



        private static void BACKUPFUNCTION()
        {


            ScreenCapture sc = new ScreenCapture();
            // capture entire screen, and save it to a file
            Image img = sc.CaptureScreen();


            // 0.6 UI Scale
            //int startPosX = 1550; int startPosY = 480;
            //int cropHeight = 77; int cropWidth = 294;
            // // NOT UPDATED !!!!
            //List<int> lines = new int[] { 0, 40 }.ToList();
            //List<int> cols = new int[] { 0, 60, 122, 184, 246, 307 }.ToList();

            // 1 UI Scale
            int startPosX = 1562, startPosY = 488, cropHeight = 77, cropWidth = 344;
            List<int> lines = new int[] { 0, 40 }.ToList();
            List<int> cols = new int[] { 0, 60, 122, 184, 246, 307 }.ToList();

            Bitmap plantedPlant = cropImage(img, new Rectangle(startPosX, startPosY, cropWidth, cropHeight));
            Point?[] plantedPoints = new Point?[6]; int plantedPointsCpt = 0;
            plantedPlant.Save("test.png");



            foreach (var line in lines)
            {
                foreach (var col in cols)
                {
                    Bitmap temp = cropImage(plantedPlant, new Rectangle(col, line, 36, 36));
                    //temp.Save("ress/planted/temp/" + "lin" + (lines.IndexOf(line) + 1) + "-col" + (cols.IndexOf(col) + 1) + ".png");
                    Bitmap tempLetter2 = cropImage(plantedPlant, new Rectangle(col + 6, line + 6, 24, 24));


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
                        ColorDepth = 32
                    };
                    var Results = Ocr.Read(temp);
                    var Results2 = Ocr.Read(tempLetter2);
                    Console.WriteLine("OCR Result1 : " + Results.Text + "OCR Result2 : " + Results2.Text);

                    var found = false;
                    var nbFileAnalysed = 0;
                    foreach (var file in Directory.GetFiles("ress/planted"))
                    {
                        nbFileAnalysed++;
                        var planted = Find(temp, new Bitmap(file));
                        if (planted != null)
                        {
                            Console.WriteLine("File: " + file + " has been found in - " + " lin:" + (lines.IndexOf(line) + 1) + "col:" + (cols.IndexOf(col) + 1));
                            plantedPointsCpt++;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine("NotFound - " + nbFileAnalysed + ": Please enter the Image at - col:" + (cols.IndexOf(col) + 1) + " / lin:" + (lines.IndexOf(line) + 1) + " :");
                        ConsoleKeyInfo letter = Console.ReadKey();
                        Console.WriteLine();
                        if (letter.Key == ConsoleKey.X || letter.Key == ConsoleKey.W ||
                            letter.Key == ConsoleKey.G || letter.Key == ConsoleKey.Y ||
                            letter.Key == ConsoleKey.H)
                        {
                            Bitmap tempLetter = cropImage(plantedPlant, new Rectangle(col + 6, line + 6, 24, 24));
                            //tempLetter.Save("ress/planted/" + letter.KeyChar + " -" + new Random().Next(1000) + ".png");
                        }

                        //else
                        //{
                        //    if (letter.Key != ConsoleKey.Enter)
                        //    {
                        //        Console.WriteLine("NotFound: Please enter the Image at - col:" + cols.IndexOf(col) + 1 + " / lin:" + lines.IndexOf(line) + 1 + " :");
                        //        ConsoleKeyInfo letter2 = Console.ReadKey();
                        //        Console.WriteLine();
                        //        if (letter2.Key == ConsoleKey.X || letter2.Key == ConsoleKey.W ||
                        //            letter2.Key == ConsoleKey.G || letter2.Key == ConsoleKey.Y ||
                        //            letter2.Key == ConsoleKey.H)
                        //        {
                        //            Bitmap tempLetter = cropImage(plantedPlant, new Rectangle(col + 8, line + 8, 16, 16));
                        //            tempLetter.Save("ress/planted/" + letter2.KeyChar + " -" + new Random().Next(1000) + ".png");
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }



        public static void initImage(Bitmap img)
        {
            Bitmap plantedW = cropImage(img, new Rectangle(13, 17, 13, 12));
            plantedW.Save("plantedW.png");

            Bitmap plantedX = cropImage(img, new Rectangle(262, 17, 13, 12));
            plantedX.Save("plantedX.png");

            Bitmap plantedG = cropImage(img, new Rectangle(212, 17, 13, 12));
            plantedG.Save("plantedG.png");

            Bitmap plantedY = cropImage(img, new Rectangle(114, 17, 13, 12));
            plantedY.Save("plantedY.png");

            Bitmap plantedH = cropImage(img, new Rectangle(64, 17, 13, 12));
            plantedH.Save("plantedH.png");



            //Bitmap inventoryW = cropImage(img, new Rectangle(11, 7, 11, 9));
            //inventoryW.Save("inventoryW.png");

            //Bitmap inventoryX = cropImage(img, new Rectangle(41, 7, 11, 9));
            //inventoryX.Save("inventoryX.png");

            //Bitmap inventoryG = cropImage(img, new Rectangle(70, 7, 11, 9));
            //inventoryG.Save("inventoryG.png");

            //Bitmap inventoryY = cropImage(img, new Rectangle(127, 7, 11, 9));
            //inventoryY.Save("inventoryY.png");

            //Bitmap inventoryH = cropImage(img, new Rectangle(156, 7, 11, 9));
            //inventoryH.Save("inventoryH.png");

        }


        private static Bitmap cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            //return (Image)(bmpCrop);
        }

        public static Point? Find(Bitmap haystack, Bitmap needle)
        {
            if (null == haystack || null == needle)
            {
                return null;
            }
            if (haystack.Width < needle.Width || haystack.Height < needle.Height)
            {
                return null;
            }

            var haystackArray = GetPixelArray(haystack);
            var needleArray = GetPixelArray(needle);

            foreach (var firstLineMatchPoint in FindMatch(haystackArray.Take(haystack.Height - needle.Height), needleArray[0]))
            {
                if (IsNeedlePresentAtLocation(haystackArray, needleArray, firstLineMatchPoint, 1))
                {
                    return firstLineMatchPoint;
                }
            }

            return null;
        }

        private static int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        private static IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine)
        {
            var y = 0;
            foreach (var haystackLine in haystackLines)
            {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x)
                {
                    if (ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length))
                    {
                        yield return new Point(x, y);
                    }
                }
                y += 1;
            }
        }

        private static bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (first[i + firstStart] != second[i + secondStart])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified)
        {
            //we already know that "alreadyVerified" lines already match, so skip them
            for (int y = alreadyVerified; y < needle.Length; ++y)
            {
                if (!ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle.Length))
                {
                    return false;
                }
            }
            return true;
        }





    }


}


