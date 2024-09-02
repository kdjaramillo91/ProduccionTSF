using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderUser
    {
        private static DBContext db = null;

        public static IEnumerable Users()
        {
            db = new DBContext();
            return db.User.Where(t=>t.isActive).ToList();
        }

        public static User UserById(int id)
        {
            db = new DBContext();
            var query = db.User.FirstOrDefault(it => it.id == id && it.isActive);
            return query;
        }

    }
}