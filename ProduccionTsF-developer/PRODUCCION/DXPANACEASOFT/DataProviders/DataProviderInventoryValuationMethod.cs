using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderInventoryValuationMethod
    {
        private static DBContext db = null;

        public static IEnumerable InventoryValuationMethod(int id_company)
        {
            db = new DBContext();
            var model = db.InventoryValuationMethod.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }
            return model;
        }

        public static InventoryValuationMethod InventoryValuationMethodById(int? id_InventoryValuationMethod)
        {
            db = new DBContext();
            return db.InventoryValuationMethod.FirstOrDefault(v => v.id == id_InventoryValuationMethod);
        }
    }
}