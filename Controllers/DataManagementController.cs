using System;
using System.Threading.Tasks;
using ReviewBuilder.Models;

namespace ReviewBuilder.Controllers
{
    public class DataManagementController
    {
        public static void ManageData()
        {
            Parallel.ForEach(ApplicationContext.UsersData, (dt) =>
            {
                if (dt.Value.uploadFile != new DateTime())
                {
                    var diff = (DateTime.Now).Subtract(dt.Value.uploadFile).TotalHours;
                    if (diff > 12)
                    {
                        UserData tmp;
                        bool c = ApplicationContext.UsersData.TryRemove(dt.Key, out tmp);
                        Console.WriteLine("{0}\tФайл {1} был загружен слишком давно." +
                            "Попытка удалить данные вернула {2}", DateTime.Now, dt.Key, c);
                    }
                }
                if (dt.Value.downloadedTime != new DateTime())
                {
                    var diff = (DateTime.Now).Subtract(dt.Value.downloadedTime).TotalMinutes;
                    if (diff > 60)
                    {
                        UserData tmp;
                        bool c = ApplicationContext.UsersData.TryRemove(dt.Key, out tmp);
                        Console.WriteLine("{0}\tПользователь {1} скачал файлы слишком давно." +
                            "Попытка удалить данные вернула {2}", DateTime.Now, dt.Key, c);
                    }
                }
            });
            GC.Collect();
        }
    }
}