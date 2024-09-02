using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderNotification
    {
        private static DBContext db = null;

        public static Notification Notification(int? id)
        {
            db = new DBContext();
            return db.Notification.FirstOrDefault(i => i.id == id);
        }

        public static SettingNotification SettingNotification(int? id)
        {
            db = new DBContext();
            return db.SettingNotification.FirstOrDefault(i => i.id == id);
        }

        public static int CountNewNotificationsForUser(int? id_user)
        {
            db = new DBContext();
            return db.Notification.Count(n => n.id_user == id_user && !n.reading);
        }
    }
}