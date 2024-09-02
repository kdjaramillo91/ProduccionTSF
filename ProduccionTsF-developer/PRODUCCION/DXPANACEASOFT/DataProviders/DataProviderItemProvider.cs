using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderItemProvider
    {
        private static DBContext db = null;

        public static ItemProvider ItemProvider(int id_provider)
        {
            db = new DBContext();
            return db.ItemProvider.FirstOrDefault(i => i.id_provider == id_provider);
        }

        public static IEnumerable ItemProviders(int id_item)
        {
            db = new DBContext();
            return db.ItemProvider.Where(i => i.id_item == id_item);
        }
    }
}