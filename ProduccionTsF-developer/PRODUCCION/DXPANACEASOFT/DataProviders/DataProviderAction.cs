using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderAction
    {
        private static DBContext db = null;

        public static TAction Action(int? id)
        {
            db = new DBContext();
            return db.TAction.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable Actions()
        {
            db = new DBContext();
            return db.TAction.ToList();
        }

        public static IEnumerable ActionsByController(int? id_controller)
        {
            db = new DBContext();
            return db.TAction.Where(a => a.id_controller == id_controller).ToList();
        }
    }
}