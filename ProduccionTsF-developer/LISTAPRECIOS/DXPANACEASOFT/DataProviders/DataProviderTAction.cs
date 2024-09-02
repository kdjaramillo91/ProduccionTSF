using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTAction
    {
        private static DBContext db = null;

        public static IEnumerable TActions()
        {
            db = new DBContext(); ;
            return db.TAction.Where(v => v.isActive && v.isActive).ToList();
        }

        public static TAction TAction(int? id)
        {
            db = new DBContext();
            return db.TAction.FirstOrDefault(v => v.id == id);
        }
    }
}