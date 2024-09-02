using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderInventoryValueValuationMethod
    {
        private static DBContext db = null;

        public static IEnumerable InventoryValuationMethods()
        {
            db = new DBContext();
            return db.InventoryValuationMethod.Where(t=>t.isActive).ToList();
        }
    }
}