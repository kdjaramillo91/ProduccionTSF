using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderRemissionGuideReason
    {
        private static DBContext db = null;

        public static IEnumerable RemisisonGuideReason()
        {
            db = new DBContext();
            return db.RemissionGuideReason.Where(r => r.isActive).ToList();
        }
        public static IEnumerable RemisisonGuideReasonsByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.RemissionGuideReason.Where(r => (r.isActive && r.id_company == id_company) || r.id == id_current).ToList();
        }
    }
}