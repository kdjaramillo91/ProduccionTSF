using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderFishingSite
    {
        private static DBContext db = null;
        public static IEnumerable FishingSites(int? id_company)
        {
            db = new DBContext();
            var model = db.FishingSite.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable FishingSites(int? id_company, int? id_fishingZone)
        {
            db = new DBContext();
            var model = db.FishingSite.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive && p.id_FishingZone == id_fishingZone).ToList();
            }

            return model;
        }

        public static IEnumerable AllFishingSites(int? id_company)
        {
            db = new DBContext();
            var model = db.FishingSite.Where(p => p.id_company == id_company).ToList();

            return model;
        }
        public static FishingSite FishingSiteById(int? id_FishingSite)
        {
            db = new DBContext(); ;
            return db.FishingSite.FirstOrDefault(v => v.id == id_FishingSite);
        }

        public static FishingSite FishingSiteFishingZoneById(int? id_FishingSite)
        {
            db = new DBContext(); 
            var fishingSite = db.FishingSite.Include("FishingZone").FirstOrDefault(v => v.id == id_FishingSite);

            return new FishingSite
            {
                id = fishingSite.id,
                code = fishingSite.code,
                name = $"{fishingSite.FishingZone.name} | {fishingSite.name}"
            };
        }
        public static IEnumerable FishingSiteWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.FishingSite.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable FishingSiteByZone(int? id_zone)
        {
            db = new DBContext();

            var fishingSitesList = db.FishingSite.Where(w => (w.isActive) && w.id_FishingZone == id_zone).ToList();
            
            return fishingSitesList;
        }
        public static IEnumerable FishingSitesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var model = db.FishingSite.Include("FishingZone").Where(t => (t.isActive && t.id_company == id_company) || t.id == id_current).ToList();
            //int[] fishingZoneIds = model
            //                        .Where(r=> r.id_FishingZone != null )
            //                        .Select(r => r.id_FishingZone.Value)
            //                        .ToArray();
            //var fishingZoneList = db.FishingZone.Where(r => fishingZoneIds.Contains(r.id)).ToArray();

            return model.Select(r => new FishingSite
            {
                 id = r.id,
                 name = $"{r.FishingZone.name} | {r.name}"
            }).ToArray();
             
        }

    }
}