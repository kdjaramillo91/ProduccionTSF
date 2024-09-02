using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderSettings
    {
        private static DBContext db = null;

        public static Setting Setting(String wscodigo)
        {
            db = new DBContext();
            return db.Setting.Where(w => w.code == wscodigo).FirstOrDefault();
        }
    }
}