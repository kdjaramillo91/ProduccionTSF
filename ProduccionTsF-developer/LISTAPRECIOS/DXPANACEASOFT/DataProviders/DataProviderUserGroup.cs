using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderUserGroup
    {
        private static DBContext db = null;

        public static IEnumerable UserGroups()
        {
            db = new DBContext();
            return db.UserGroup.Where(t => t.isActive).ToList();
        }

        public static UserGroup UserGroupById(int? id)
        {
            db = new DBContext();
            var query = db.UserGroup.FirstOrDefault(it => it.id == id && it.isActive);
            return query;
        }
    }
}