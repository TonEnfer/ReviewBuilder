using ReviewBuilder.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using TemplateEngine.Docx;
using ReviewBuilder.Models;
using System.Linq;
using System.Xml.Linq;
using System.IO.Compression;

namespace ReviewBuilder.Controllers
{
    class ReviewFieldController
    {
        public static void Build(string id)
        {
            ApplicationContext.UsersData[id].reviewFields =
                ParceInputFile(ApplicationContext.UsersData[id].inputFile);
            ZipArchive arh = new ZipArchive(
                ApplicationContext.UsersData[id].outputFile, ZipArchiveMode.Update);
            foreach (var rf in ApplicationContext.UsersData[id].reviewFields)
            {
                ZipArchiveEntry archEntry =
                arh.CreateEntry(rf.StudentName.Split(' ')[0] + " " +
                rf.StudentGroup + ".docx", CompressionLevel.Optimal);
                using (Stream st = archEntry.Open())
                {
                    FieldTemplate(rf).CopyTo(st);
                }
            }
        }
        private static List<ReviewFields> ParceInputFile(MemoryStream input)
        {
            List<ReviewFields> rfs = new List<ReviewFields>();
            SpreadsheetDocument ssd = SpreadsheetDocument.Open(input, true);
            WorkbookPart workbookPart = ssd.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
            Sheets sheets = workbookPart.Workbook.Sheets;
            Sheet sheet = sheets.GetFirstChild<Sheet>();

            SheetData sheetData = worksheetPart.Worksheet.Descendants<SheetData>().FirstOrDefault();
            SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;

            foreach (var row in sheetData.Elements<Row>())
            {
                if (row.RowIndex == 1)
                    continue;
                ReviewFields rf = new ReviewFields();
                rf.Discipline = ExcelUtils.GetCellText(sharedStringPart, row, "A");
                rf.Theme = ExcelUtils.GetCellText(sharedStringPart, row, "B");
                rf.StudentName = ExcelUtils.GetCellText(sharedStringPart, row, "C");
                rf.StudentGroup = ExcelUtils.GetCellText(sharedStringPart, row, "D");
                rf.ChiefName = ExcelUtils.GetCellText(sharedStringPart, row, "E");
                uint number;
                if (UInt32.TryParse(ExcelUtils.GetCellText(sharedStringPart, row, "F"), out number))
                    rf.Evaluation = number;
                else
                    rf.Evaluation = 0;
                rf.EvaluatonsSet = GetRandomEvaluationsSet(rf.Evaluation);
            }
            return rfs;
        }
        private static List<Evaluations> GetRandomEvaluationsSet(uint Evaluation)
        {
            List<Evaluations> evs = new List<Evaluations>();
            IEnumerable<int> seq;

            do
                seq = GetSequence(11);
            while (seq.Sum() / ReviewFields.EvaluationsCount < 0.75);
            switch (Evaluation)
            {
                case 3:
                    foreach (var e in seq)
                    {
                        Evaluations ev = new Evaluations();
                        ev.Low = Convert.ToBoolean(e);
                        ev.Medium = ev.Low == false ? Convert.ToBoolean(_random.Next(0, 1)) : false;
                        ev.High = !(ev.Low | ev.Medium);
                        evs.Add(ev);
                    }
                    break;
                case 4:
                    foreach (var e in seq)
                    {
                        Evaluations ev = new Evaluations();
                        ev.Medium = Convert.ToBoolean(e);
                        ev.High = ev.Medium == false ? Convert.ToBoolean(_random.Next(0, 1)) : false;
                        ev.Low = !(ev.High | ev.Medium);
                        evs.Add(ev);
                    }
                    break;
                case 5:
                default:
                    foreach (var e in seq)
                    {
                        Evaluations ev = new Evaluations();
                        ev.High = Convert.ToBoolean(e);
                        ev.Medium = ev.High == false ? Convert.ToBoolean(_random.Next(0, 1)) : false;
                        ev.Low = !(ev.High | ev.Medium);
                        evs.Add(ev);
                    }
                    break;
            }
            return evs;

        }
        private static Random _random = new Random();

        private static IEnumerable<int> GetSequence(int size)
        {
            return Enumerable.Range(_random.Next(0, 1), size);
        }
        private static MemoryStream FieldTemplate(ReviewFields rf)
        {
            MemoryStream outStream = new MemoryStream();
            ApplicationContext.templateFile.CopyTo(outStream);
            var valueToFill = new Content(
                new FieldContent("Discipline", rf.Discipline),
                new FieldContent("Theme", rf.Theme),
                new FieldContent("StudentName", rf.StudentName),
                new FieldContent("StudentGroup", rf.StudentGroup),
                new FieldContent("ChiefName", rf.ChiefName)
            );
            for (int i = 0; i < rf.EvaluatonsSet.Count; i++)
            {
                valueToFill.Fields.Add(
                    new FieldContent("High" + i, rf.EvaluatonsSet[i].High ? "+" : "-"));
                valueToFill.Fields.Add(
                    new FieldContent("Medium" + i, rf.EvaluatonsSet[i].Medium ? "+" : "-"));
                valueToFill.Fields.Add(
                    new FieldContent("Low" + i, rf.EvaluatonsSet[i].Low ? "+" : "-"));
            }

            using (TemplateProcessor tp = new TemplateProcessor(outStream)
                                                .SetRemoveContentControls(true))
            {
                tp.FillContent(valueToFill);
                tp.SaveChanges();
            }

            return outStream;
        }
    }
}