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

namespace ReviewBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewBuilderController : ControllerBase
    {
        private ApplicationContext _context;

        public ReviewBuilderController(ApplicationContext context)
        {
            _context = context;
            // if(_context.UserModel.Count() != 0)
            //     _context.UserModel.Add(new UserModel{Id = 0});
        }
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            string path = "./wwwroot/Files/";
            long size = files.Sum(f => f.Length);
            string id = GetToken();
            User user = _context.UserModel.Find(id);
            string newPath = path + id + "/";
            Directory.CreateDirectory(path + id);
            foreach (var UploadedFile in files)
            {
                using (var fileStream =
                new FileStream(newPath + UploadedFile.FileName, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }
                FieldFileData f = new FieldFileData();
                f.Id = (user.fieldFiles == null || user.fieldFiles.Count == 0) ?
                 0 : user.fieldFiles.Last().Id + 1;
                f.Name = UploadedFile.Name;
                f.Path = newPath;
                user.fieldFiles.Add(f);
                _context.UserModel.Update(user);
            }
            _context.SaveChanges();

            return Ok(new { Id = id });
        }
        public string GetToken()
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            string id = r.Next(1000000, 9999999).ToString("X");
            while (_context.UserModel.Find(id) != null)
                id = r.Next(1000000, 9999999).ToString("X");
            _context.UserModel.Add(new User() { Id = id, dt = DateTime.Now });
            _context.SaveChangesAsync();
            return id;
        }
        [HttpGet("GetBuilded/{id}")]
        public ActionResult<bool> GetBuilded(int id)
        {
            if (_context.UserModel.Find(id) == null)
                return NotFound();
            return Ok(new { builded = _context.UserModel.Find(id).builded });
        }

        [HttpGet("GetFiles/{id}")]
        public async Task<IActionResult> GetFiles(int id)
        {

            if (_context.UserModel.Find(id) == null ||
             _context.UserModel.Find(id).builded == false)
                return NotFound(new { Error = "File not found" });
            string path = "./wwwroot/Files/" + id;
            if (System.IO.File.Exists(path + "/zipped.zip"))
                System.IO.File.Delete(path + "/zipped.zip");
            ZipFile.CreateFromDirectory(path + "/Generated/", path + "/zipped.zip",
                CompressionLevel.Optimal, false);
            var memory = new MemoryStream();
            path += "/zipped.zip";
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
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
            //int a = 0;
            return Ok();
        }
        private bool CheckFileFormat(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() == ".xlsx";
        }
        private bool CheckFileStruct(string filePath)
        {
            return false;
        }
    }
}