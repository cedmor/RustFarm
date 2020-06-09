using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace Rust.Utils
{
    public class ExcelHelper
    {
        public static void WriteInExcel()
        {
            // // TODO REWORK : using plant paremeters & object
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

    }
}
