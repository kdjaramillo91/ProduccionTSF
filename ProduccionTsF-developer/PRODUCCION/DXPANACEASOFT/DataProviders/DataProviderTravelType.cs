using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTravelType
    {
        private static DBContext db = null;

        public static IEnumerable TravelTypes()
        {
            db = new DBContext();
            return db.RemissionGuideTravelType.Where(t => t.isActive).ToList();
        }

        public static RemissionGuideTravelType TravelType(int? id)
        {
            db = new DBContext();
            return db.RemissionGuideTravelType.FirstOrDefault(t => t.id == id);
        }

        public static RemissionGuideTravelType TravelTypeGone(int? id)
        {
            db = new DBContext();
            return db.RemissionGuideTravelType.FirstOrDefault(t => t.id == id && t.code == "IDA");
        }

        public static RemissionGuideTravelType TravelTypeBack(int? id)
        {
            db = new DBContext();
            return db.RemissionGuideTravelType.FirstOrDefault(t => t.id == id && t.code == "REGRESO");
        }
    }
}