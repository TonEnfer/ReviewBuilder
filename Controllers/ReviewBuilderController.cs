using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using ReviewBuilder.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO.Compression;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;
using ReviewBuilder.Excel;
using ReviewBuilder;
using System.ComponentModel;
using System.Threading;

namespace ReviewBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewBuilderController : ControllerBase
    {

        public ReviewBuilderController()
        {

        }
        [HttpPost("UploadFiles")]
        public IActionResult UploadFiles(IFormFile files)
        {

            if (files == null)
            {
                Console.WriteLine("{0}\tПопытка загрузить пустой файл", DateTime.Now);
                return BadRequest();
            }

            long size = files.Length;

            if (size > 102400)
            {
                Console.WriteLine("{0}\tПопытка загрузить слишком большой файл", DateTime.Now);
                return BadRequest(new { Error = "Файл слишком большой" });
            }
            string id = GetToken();
            ApplicationContext.UsersData.TryAdd(id, new UserData());
            using (var ms = new MemoryStream())
            {
                ms.Position = 0;
                files.CopyTo(ms);
                if (!CheckFileFormat(files.FileName))
                    return BadRequest(new { Message = "Bad File " + files.FileName });
                if (!CheckFileStruct(ms))
                    return BadRequest(new { Message = "Bad File " + files.FileName });
                Console.WriteLine("{0}\tNew file {1}, files size: {2}",
                    DateTime.Now, files.FileName, size);
                ms.Position = 0;
                ApplicationContext.UsersData[id].inputFile.Position = 0;
                ms.CopyTo(ApplicationContext.UsersData[id].inputFile);
            }
            ApplicationContext.UsersData[id].uploadFile = DateTime.Now;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (object sender, DoWorkEventArgs arg) =>
            {
                Console.WriteLine("{0}\tStart building file {1}", DateTime.Now, id);
                ReviewFieldController.Build(id);
            };
            worker.RunWorkerCompleted +=
                (object sender, RunWorkerCompletedEventArgs arg) =>
                    {
                        ApplicationContext.UsersData[id].isReady = true;
                        Console.WriteLine("{0}\tFile {1} builded", DateTime.Now, id);
                    };
            worker.RunWorkerAsync();

            return Ok(new { Id = id });
        }

        public string GetToken()
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            string id = r.Next(1000000, 9999999).ToString("X");
            while (ApplicationContext.UsersData.ContainsKey(id))
                id = r.Next(1000000, 9999999).ToString("X");
            return id;
        }

        [HttpGet("IsReady/{id}")]
        public ActionResult<bool> IsReady(string id)
        {
            if (!ApplicationContext.UsersData.ContainsKey(id))
                return NotFound();
            return Ok(new { isReady = ApplicationContext.UsersData[id].isReady });
        }

        [HttpGet("GetFiles/{id}")]
        public IActionResult GetFiles(string id)
        {
            if (ApplicationContext.UsersData.ContainsKey(id) &&
                ApplicationContext.UsersData[id].isReady)
            {
                ApplicationContext.UsersData[id].outputFile.Position = 0;
                MemoryStream ms = new MemoryStream();
                ApplicationContext.UsersData[id].outputFile.CopyTo(ms);
                ms.Position = 0;
                if (ApplicationContext.UsersData[id].downloadedTime == new DateTime())
                {
                    //ApplicationContext.UsersData[id].downloadedTime = new DateTime();
                    ApplicationContext.UsersData[id].downloadedTime = DateTime.Now;
                }
                return File(ms, GetContentType("file.zip"), id + ".zip");
            }
            return NotFound(new { Error = "File not found" });
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip","application/zip"}
            };
        }
        private bool CheckFileFormat(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() == ".xlsx";
        }
        private bool CheckFileStruct(Stream stream)
        {

            SpreadsheetDocument ssd = SpreadsheetDocument.Open(stream, true);
            WorkbookPart workbookPart = ssd.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
            Sheets sheets = workbookPart.Workbook.Sheets;
            Sheet sheet = sheets.GetFirstChild<Sheet>();

            SheetData sheetData = worksheetPart.Worksheet.Descendants<SheetData>().FirstOrDefault();
            SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;
            Row r = ExcelUtils.GetRow(sheetData, 1);
            foreach (var el in ApplicationContext.checkCellList)
            {
                var strId = Convert.ToString(ExcelUtils.FindStringId(sharedStringPart, el.Value));
                var cl = ExcelUtils.GetCell(r, el.Key);
                if (!(cl.DataType == "s" && cl.CellValue.Text == strId))
                    return false;
            }
            return true;
        }
    }
}