using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProvideClassShrimp
    {
        private static DBContext db = null;

        public static IEnumerable<ClassShrimp> GetClassShrimp()
        {
            db = new DBContext();
            return db.ClassShrimp;
        }
    }
}