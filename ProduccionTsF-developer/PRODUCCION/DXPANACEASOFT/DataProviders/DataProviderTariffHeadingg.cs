using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTariffHeadingg
    {
       private static DBContext db = null;

        public static IEnumerable TariffHeadinggContries()
        {
            db = new DBContext();
            var model = db.TariffHeading.Where(t => t.isActive).ToList();
                      
            return model;
        }

        public static IEnumerable AllTariffHeadingg()
        {
            db = new DBContext();
            var model = db.TariffHeading.ToList();

            return model;
        }
        public static TariffHeading TariffHeadinggById(int? id_TariffHeadingg)
        {
            db = new DBContext(); 
            return db.TariffHeading.FirstOrDefault(v => v.id == id_TariffHeadingg);
        }

        public static IEnumerable TariffHeadinggWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.TariffHeading.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable CityWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.City.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable StateOfTariffHeadinggWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.StateOfContry.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
        public static IEnumerable AllTariffHeading()
        {
            db = new DBContext();
            var model = db.TariffHeading.ToList();

            return model;
        }

        public static TariffHeading TariffHeadingById(int? id_tariffHeading)
        {
            db = new DBContext(); ;
            return db.TariffHeading.FirstOrDefault(v => v.id == id_tariffHeading);
        }

        public static IEnumerable TariffHeadingWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.TariffHeading.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}