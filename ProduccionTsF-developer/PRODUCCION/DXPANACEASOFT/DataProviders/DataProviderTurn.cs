using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTurn
    {
       private static DBContext db = null;

        public static IEnumerable AllTurns()
        {
            db = new DBContext();
            var model = db.Turn.ToList();

            return model;
        }

        public static Turn TurnById(int? id_turn)
        {
            db = new DBContext(); ;
            return db.Turn.FirstOrDefault(v => v.id == id_turn);
        }

        public static IEnumerable TurnWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.Turn.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

    }
}