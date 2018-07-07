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
            using (var file = new FileStream("Template.docx", FileMode.Open))
            {
                file.CopyTo(ApplicationContext.templateFile);
            }
            var q = XElement.Load("TemplateStruct.xml");
            foreach (var e in q.Elements("STR"))
            {
                var addr = (string)e.Attribute("address").Value;
                var val = (string)e.Value;
                ApplicationContext.checkCellList.Add(addr, val);
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
