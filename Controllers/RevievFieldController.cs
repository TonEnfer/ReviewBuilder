using ReviewBuilder.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using ReviewBuilder.Models;
using System.Linq;
using System.Xml.Linq;

namespace ReviewBuilder.Controllers
{
    class ReviewFieldController
    {
        private string path = null;
        private ApplicationContext context;

        public ReviewFieldController(string path, ApplicationContext context)
        {
            this.path = path;
            this.context = context;
        }

        public List<ReviewFields> ReadExcelDoc()
        {
            List<ReviewFields> ReviewFields = null;
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, false))
                {
                    ReviewFields = ExcelParser.GetParser(document).Parse(document);
                    return ReviewFields;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
            finally
            {

                GC.Collect();
            }
        }
    }
    public abstract class ExcelParser
    {
        public abstract List<ReviewFields> Parse(SpreadsheetDocument document);
        public static ExcelParser GetParser(SpreadsheetDocument document)
        {


            return new FieldParcer();
        }
    }
    class FieldParcer : ExcelParser
    {
        public override List<ReviewFields> Parse(SpreadsheetDocument document)
        {
            try
            {
                XElement tmp = XElement.Load("TemplateStruction.xml");
                Dictionary<string, string> tmpStruct = new Dictionary<string, string>();
                foreach (var el in tmp.Elements("STR"))
                {
                    var addr = (string)el.Attribute("address").Value;
                    var val = (string)el.Value;
                    tmpStruct.Add(addr, val);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            WorkbookPart workbookPart = document.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
            Sheets sheets = workbookPart.Workbook.Sheets;
            Sheet sheet = sheets.GetFirstChild<Sheet>();

            SheetData sheetData = worksheetPart.Worksheet.Descendants<SheetData>().FirstOrDefault();
            SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;
            return new List<ReviewFields>();
        }
    }
}