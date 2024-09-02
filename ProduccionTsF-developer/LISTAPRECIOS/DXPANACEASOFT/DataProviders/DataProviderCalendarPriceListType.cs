using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCalendarPriceListType
    {
        private static DBContext db = null;

        public static CalendarPriceListType CalendarPriceListType(int id)
        {
            db = new DBContext();
            return db.CalendarPriceListType.Where(w => w.id == id).FirstOrDefault();
        }

        public static IEnumerable CalendarPriceListTypes(int id_company)
        {
            db = new DBContext();
            var model = db.CalendarPriceListType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable CalendarPriceListByCompany(int? id_company)
        {
            db = new DBContext();
            return db.CalendarPriceListType.Where(g => (g.isActive && g.id_company == id_company)).Select(p => new { p.id, name = p.name }).ToList();
        }
    }
}