using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReviewBuilder.Excel
{
    public static class ExcelUtils
    {
        public static int FindStringId(SharedStringTablePart sharedStrings, string value)
        {
            int markerId = -100;
            //sharedStrings.SharedStringTable.ChildElements.ToList().IndexOf(
            //    sharedStrings.SharedStringTable.ChildElements.Where(k => k.InnerText == value).FirstOrDefault());
            if (sharedStrings != null)
                for (int i = 0; i < sharedStrings.SharedStringTable.ChildElements.Count; i++)
                {
                    if (sharedStrings.SharedStringTable.ChildElements[i].InnerText == value)
                    {
                        markerId = i;
                        break;
                    }
                }

            return markerId;
        }
        public static string FindStringValue(SharedStringTablePart sharedStrings, int id)
        {
            return sharedStrings.SharedStringTable.ChildElements[id].InnerText;
        }

        public static Row GetRow(SheetData wsData, UInt32 rowIndex)
        {
            var row = wsData.Elements<Row>().
            Where(r => r.RowIndex.Value == rowIndex).FirstOrDefault();
            if (row == null)
            {
                row = new Row() { RowIndex = rowIndex };
                wsData.Append(row);
            }
            return row;
        }
        public static Cell GetCell(Row r, string collumnName)
        {

            return r.Elements<Cell>()
                .Where(c => (Regex.IsMatch(c.CellReference.Value, collumnName + "[0-9]*")))
                .FirstOrDefault();
        }
        public static string GetCellText(Row r, string col)
        {
            Cell c = GetCell(r, col);
            if (c == null)
                return "0";
            if (c.CellValue == null)
                return "0";

            return c.CellValue.Text;
        }


        public static Worksheet GetWorksheet(SpreadsheetDocument document, string worksheetName)
        {
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook
                .Descendants<Sheet>().Where(s => s.Name == worksheetName);
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart
                .GetPartById(sheets.First().Id);
            return worksheetPart.Worksheet;
        }

        public static Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = value == "0" ? new CellValue() : new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        public static Cell ConstructCell(string value, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = value == "0" ? new CellValue() : new CellValue(value),
                StyleIndex = styleIndex
            };
        }
    }
}