using System;
using ReviewBuilder.Models;

namespace ReviewBuilder.Controllers
{
    public class DataManagementController
    {
        //DataManagementController dm = null;
        public static void ManageData()
        {
            foreach (var dt in ApplicationContext.UsersData)
            {
                if (dt.Value.downloadedTime != new DateTime())
                {
                    var diff = (DateTime.Now).Subtract(dt.Value.downloadedTime).TotalMinutes;
                    //Console.WriteLine("Пользователь {0} скачал файл {1} секунд назад", dt.Key,
                    //    diff);
                    if (diff > 60)
                    {
                        UserData tmp;
                        bool c = ApplicationContext.UsersData.TryRemove(dt.Key, out tmp);
                        Console.WriteLine("Пользователь {0} скачал файлы слишком давно. Попытка удалить данные вернула {1}", dt.Key, c);
                    }
                }
            }
        }
    }
}