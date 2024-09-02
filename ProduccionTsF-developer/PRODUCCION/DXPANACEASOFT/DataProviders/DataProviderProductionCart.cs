using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionCart
    {
       private static DBContext db = null;

        public static IEnumerable AllProductionCarts()
        {
            db = new DBContext();
            var model = db.ProductionCart.ToList();

            return model;
        }

        public static IEnumerable ProductionCarts()
        {
            db = new DBContext();
            var model = db.ProductionCart.Where(g => (g.isActive)).ToList();

            return model;
        }

        public static ProductionCart ProductionCartById(int? id_productionCarts)
        {
            db = new DBContext(); ;
            return db.ProductionCart.FirstOrDefault(v => v.id == id_productionCarts);
        }

        public static IEnumerable ProductionCartWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.ProductionCart.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

    }
}