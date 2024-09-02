using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderInventoryControlType
    {
        private static DBContext db = null;

        public static IEnumerable InventoryControlType(int id_company)
        {
            db = new DBContext();
            var model = db.InventoryControlType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static InventoryControlType InventoryControlTypeById(int? id_InventoryControlType)
        {
            db = new DBContext(); 
            return db.InventoryControlType.FirstOrDefault(v => v.id == id_InventoryControlType);
        }
    }
}