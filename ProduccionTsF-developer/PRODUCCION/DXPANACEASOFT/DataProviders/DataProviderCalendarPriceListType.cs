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

        public static IEnumerable CalendarPriceListTypes(int id_company)
        {
            db = new DBContext();

            var model = db.CalendarPriceListType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static CalendarPriceListType CalendarPriceListType(int id)
        {
            db = new DBContext();
            return db.CalendarPriceListType.FirstOrDefault(p => p.id == id && p.isActive);
        }

        public static CalendarPriceListType CalendarPriceListTypeById(int? id_calendarPriceListType)
        {
            db = new DBContext();
            return db.CalendarPriceListType.FirstOrDefault(v => v.id == id_calendarPriceListType);
        }

    }
}