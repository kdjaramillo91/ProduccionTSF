using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderEnvironmentType
    {
        private static DBContext db = null;

        public static EnvironmentType EnvironmentType(int? id)
        {
            db = new DBContext();
            return db.EnvironmentType.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable EnvironmentsTypes(int id_company)
        {
            db = new DBContext();
            return db.EnvironmentType.Where(e => e.id_company == id_company && e.isActive).ToList();
        }
    }
}