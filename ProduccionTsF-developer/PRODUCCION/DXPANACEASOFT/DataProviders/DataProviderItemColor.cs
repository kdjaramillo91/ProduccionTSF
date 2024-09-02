using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderItemColor
    {
        private static DBContext db = null;

        public static IEnumerable ItemsColors()
        {
            db = new DBContext();
            return db.ItemColor.Where(t=>t.isActive).ToList();
        }

        public static ItemColor ItemColor(int id)
        {
            db = new DBContext();
            return db.ItemColor.FirstOrDefault(i => i.id == id);
        }

        public static ItemColor ItemColorById(int? id)
        {
            db = new DBContext();
            var query = db.ItemColor.FirstOrDefault(t => t.id == id);
            return query;
        }

    }
}