using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ReviewBuilder.Models;
using ReviewBuilder.Controllers;
using System.Threading;

namespace ReviewBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Load Template.docx from {0}",Environment.CurrentDirectory);
            using (var file = new FileStream(Environment.CurrentDirectory+"/Template.docx", FileMode.Open))
            {
                file.CopyTo(ApplicationContext.templateFile);
            }
            Console.WriteLine("Load TemplateStruct.xml from {0}",Environment.CurrentDirectory);
            var q = XElement.Load(Environment.CurrentDirectory+"/TemplateStruct.xml");
            foreach (var e in q.Elements("STR"))
            {
                var addr = (string)e.Attribute("address").Value;
                var val = (string)e.Value;
                ApplicationContext.checkCellList.TryAdd(addr, val);
            }
            var t = Task.Run(() =>
            {
                while (true)
                {
                    DataManagementController.ManageData();
                    Thread.Sleep(10000);
                }
            });


            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

    }
}
