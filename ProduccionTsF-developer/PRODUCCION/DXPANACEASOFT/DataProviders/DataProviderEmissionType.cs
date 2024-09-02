using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderEmissionType
    {
        private static DBContext db = null;

        public static EmissionType EmissionType(int? id)
        {
            db = new DBContext();
            return db.EmissionType.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable EmissionsTypes(int id_company)
        {
            db = new DBContext();
            return db.EmissionType.Where(e => e.id_company == id_company && e.isActive).ToList();
        }
    }
}