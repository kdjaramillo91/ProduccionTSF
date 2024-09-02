using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPersonType
    {
        private static DBContext db = null;

        public static PersonType PersonType(int id)
        {
            db = new DBContext();
            return db.PersonType.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable PersonTypes()
        {
            db = new DBContext();
            return db.PersonType.Where(t=>t.isActive).ToList();
        }

    }
}