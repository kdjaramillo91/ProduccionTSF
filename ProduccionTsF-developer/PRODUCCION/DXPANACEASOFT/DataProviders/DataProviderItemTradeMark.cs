using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderItemTrademark
    {
        private static DBContext db = null;

        public static IEnumerable ItemTrademarks(int id_company)
        {
            db = new DBContext();
            var model = db.ItemTrademark.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }


        public static IEnumerable ItemTrademarksFilter(int id_company)
        {
            db = new DBContext();
            var model = db.ItemTrademark.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static ItemTrademark ItemTrademarkById(int? id)
        {
            db = new DBContext();
            var query = db.ItemTrademark.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable ItemTrademarksByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            if (id_company.HasValue)
            {
                var tradeMarks = db.ItemTrademark
                    .Where(e => e.id_company == id_company.Value && e.isActive)
                    .ToList();

                if (id_current.HasValue)
                {
                    tradeMarks = tradeMarks.Where(e => e.id == id_current.Value).ToList();
                }

                return tradeMarks;
            }

            return null;
        }
    }
}