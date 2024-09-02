using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderRise
    {
        private static DBContext db = null;
        public static IEnumerable CategoryRises()
        {
            db = new DBContext();
            var model = db.CategoryRise.Where(t => t.isActive).ToList();

            return model;
        }


        public static CategoryRise CategoryRiseById(int? id_categoryRise)
        {
            db = new DBContext(); ;
            return db.CategoryRise.FirstOrDefault(v => v.id == id_categoryRise);
        }

        public static IEnumerable CategoryRiseWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.CategoryRise.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable ActivityRises()
        {
            db = new DBContext();
            var model = db.ActivityRise.Where(t => t.isActive).ToList();

            return model;
        }


        public static ActivityRise ActivityRiseById(int? id_activityRise)
        {
            db = new DBContext(); ;
            return db.ActivityRise.FirstOrDefault(v => v.id == id_activityRise);
        }

        public static IEnumerable ActivityRiseWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.ActivityRise.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}