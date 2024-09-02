using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderController
    {
        private static DBContext db = null;

        public static TController Controller(int? id)
        {
            db = new DBContext();
            return db.TController.FirstOrDefault(i => i.id == id);
        }

        public static List<TController> Controllers()
        {
            db = new DBContext();
            return db.TController.ToList();
        }
    }
}