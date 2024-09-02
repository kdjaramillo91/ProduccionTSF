using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderMenu
    {
        private static DBContext db = null;

        public static Menu Menu(int? id)
        {
            db = new DBContext();
            return db.Menu.FirstOrDefault(i => i.id == id);
        }

        public static List<Menu> Menues()
        {
            db = new DBContext();
            return db.Menu.ToList();
        }

        public static List<Menu> MenuesDifferent(int? id_menu)
        {
            db = new DBContext();
            return db.Menu.Where(m => m.id != id_menu).ToList();
        }
    }
}