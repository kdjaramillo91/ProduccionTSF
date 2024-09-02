using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;


namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderRemissionGuideTravelType
    {
        private static DBContext db = null;

        public static IEnumerable RemissionGuideTravelType(int id_company)
        {
            db = new DBContext();
            var model = db.RemissionGuideTravelType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static RemissionGuideTravelType RemissionGuideTravelTypeById(int? id_remissionGuideTravelType)
        {
            using (DBContext db = new DBContext())
            {
                return db.RemissionGuideTravelType.FirstOrDefault(i => i.id == id_remissionGuideTravelType);
            }

        }
    }
}