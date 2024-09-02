using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderItemTrademarkModel
    {
        private static DBContext db = null;

        public static IEnumerable ItemTrademarkModels()
        {
            db = new DBContext();
            return db.ItemTrademarkModel.Where(t => t.isActive).ToList();
        }

        public static IEnumerable ItemTrademarkModelsByTrademark(int? id_trademark)
        {
            db = new DBContext();
            int id = id_trademark ?? 0;
            var query = db.ItemTrademarkModel.Where(t => t.id_trademark == id);
            return query.ToList();
        }

        public static ItemTrademarkModel ItemTrademarkModelById(int? id)
        {
            db = new DBContext();
            var query = db.ItemTrademarkModel.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}