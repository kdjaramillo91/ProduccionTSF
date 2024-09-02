using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderFishingZone
    {

        private static DBContext db = null;
        public static IEnumerable FishingZone(int? id_company)
        {
            db = new DBContext();
            var model = db.FishingZone.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
      }
        public static IEnumerable AllFishingZone(int? id_company)
        {
            db = new DBContext();
            var model = db.FishingZone.Where(p => p.id_company == id_company).ToList();

            return model;
        }
        public static FishingZone FishingZoneById(int? id_FishingZone)
        {
            db = new DBContext(); ;
            return db.FishingZone.FirstOrDefault(v => v.id == id_FishingZone);
        }
        public static IEnumerable FishingZoneWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.FishingZone.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
        public static IEnumerable FishingZonesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var model = db.FishingZone.Where(t => (t.isActive && t.id_company == id_company) || t.id == id_current).ToList();

            return model;
        }
        public static IEnumerable FishingZones(int? id_company)
        {
            db = new DBContext();
            var model = db.FishingZone.Where(t => (t.isActive && t.id_company == id_company)).ToList();

            return model;
        }

    }
}