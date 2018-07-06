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

namespace ReviewBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewBuilderController : ControllerBase
    {

        public ReviewBuilderController()
        {
            // _context = context;
            //  if(_context.UserModel.Count() != 0)
            //      _context.UserModel.Add(new User{id = "000000"});
        }
        [HttpPost("UploadFiles")]
        public IActionResult UploadFiles(IFormFile files)
        {

            if (files == null)
            {
                Console.WriteLine("No file found");
                return NotFound();
            }

            long size = files.Length;

            Console.WriteLine("\r\n!!!!New file {0}, files size: {1}\r\n", files.FileName, size);


            string path = "./wwwroot/Files/";
            string id = GetToken();
            Program.UsersData.TryAdd(id, new UserData());
            string newPath = path + id + "/";
            Directory.CreateDirectory(path + id);
            using (var ms = new MemoryStream())
            {
                files.CopyTo(ms);
                if (!CheckFileFormat(files.FileName))
                    return BadRequest(new { Message = "Bad File " + files.FileName });

                if (!CheckFileStruct(ms))
                    return BadRequest(new { Message = "Bad File " + files.FileName });
                ms.CopyTo(Program.UsersData[id].inputFile);
            }
            
            //_context.SaveChanges();

            return Ok(new { Id = id });
        }
        public string GetToken()
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            string id = r.Next(1000000, 9999999).ToString("X");
            while (Program.UsersData.ContainsKey(id))
                id = r.Next(1000000, 9999999).ToString("X");
            // _context.UserModel.Add(new User() { id = id });
            // _context.SaveChangesAsync();
            return id;
        }
        [HttpGet("IsReady/{id}")]
        public ActionResult<bool> IsReady(string id)
        {
            if (!Program.UsersData.ContainsKey(id))
                return NotFound();
            return Ok(new { isReady = Program.UsersData[id].isReady });
        }

        [HttpGet("GetFiles/{id}")]
        public async Task<IActionResult> GetFiles(int id)
        {

            // if (_context.UserModel.Find(id) == null ||
            //  _context.UserModel.Find(id).builded == false)
            //     return NotFound(new { Error = "File not found" });
            // string path = "./wwwroot/Files/" + id;
            // if (System.IO.File.Exists(path + "/zipped.zip"))
            //     System.IO.File.Delete(path + "/zipped.zip");
            // ZipFile.CreateFromDirectory(path + "/Generated/", path + "/zipped.zip",
            //     CompressionLevel.Optimal, false);
            // var memory = new MemoryStream();
            // path += "/zipped.zip";
            // using (var stream = new FileStream(path, FileMode.Open))
            // {
            //     await stream.CopyToAsync(memory);
            // }
            // memory.Position = 0;
            // return File(memory, GetContentType(path), Path.GetFileName(path));
            return null;
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
                {".xlsx", "application/vnd.openxmlformats.officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip","application/zip"}
            };
        }
        [HttpGet("BuildFiles/{id}")]
        public async Task<IActionResult> BuildFiles(int id)
        {
            return Ok();
        }
        private bool CheckFileFormat(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() == ".xlsx";
        }
        private bool CheckFileStruct(Stream stream)
        {
            var q = XElement.Load("TemplateStruct.xml");
            Dictionary<string, string> checkCellList = new Dictionary<string, string>();
            foreach (var e in q.Elements("STR"))
            {
                var addr = (string)e.Attribute("address").Value;
                var val = (string)e.Value;
                checkCellList.Add(addr, val);
            }
            SpreadsheetDocument ssd = SpreadsheetDocument.Open(stream, true);
            WorkbookPart workbookPart = ssd.WorkbookPart;
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
            Sheets sheets = workbookPart.Workbook.Sheets;
            Sheet sheet = sheets.GetFirstChild<Sheet>();

            SheetData sheetData = worksheetPart.Worksheet.Descendants<SheetData>().FirstOrDefault();
            SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;
            Row r = ExcelUtils.GetRow(sheetData, 1);
            foreach (var el in checkCellList)
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