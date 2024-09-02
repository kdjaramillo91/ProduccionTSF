using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPermission
    {
        private static DBContext db = null;

        public static Permission Permission(int? id)
        {
            db = new DBContext();
            return db.Permission.FirstOrDefault(i => i.id == id);
        }

        public static List<Permission> Permissions()
        {
            db = new DBContext();
            return db.Permission.ToList();
        }
    }
}