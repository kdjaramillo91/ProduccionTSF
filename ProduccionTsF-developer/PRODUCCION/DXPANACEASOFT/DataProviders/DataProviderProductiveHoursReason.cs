using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderProductiveHoursReason
    {
       private static DBContext db = null;

        public static IEnumerable AllContries()
        {
            db = new DBContext();
            var model = db.productiveHoursReason.Where(p => p.isActive).ToList();

            return model;
        }
        public static productiveHoursReason productiveHoursReasonById(int? id_productiveHoursReason)
        {
            db = new DBContext(); ;
            return db.productiveHoursReason.FirstOrDefault(v => v.id == id_productiveHoursReason);
        }

        public static IEnumerable productiveHoursReasonWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.productiveHoursReason.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}