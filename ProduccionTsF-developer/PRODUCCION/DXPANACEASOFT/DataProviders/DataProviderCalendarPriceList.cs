using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCalendarPriceList
    {
        private static DBContext db = null;

        public static IEnumerable CalendarPriceLists(int id_company)
        {
            db = new DBContext();

            //var model = db.CalendarPriceList.ToList();

            //if (id_company != 0)
            //{
             var model = db.CalendarPriceList.Where(p => p.id_company == id_company).AsEnumerable().Select(s => new {
                    s.id,
                    name = s.CalendarPriceListType.name + " [" + s.startDate.ToString("dd/MM/yyyy") +  " - " + 
                                                            s.endDate.ToString("dd/MM/yyyy") + "]"
                }).OrderBy(t => t.id).ToList();
            //}

            return model;
        }

        public static IEnumerable CalendarPriceListCurrentsAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var calendarPriceListAux = db.CalendarPriceList.Where(g => (g.isActive && g.id_company == id_company)).ToList();

            var nowAux = DateTime.Now;
            calendarPriceListAux = calendarPriceListAux.AsEnumerable().Where(w => DateTime.Compare(w.startDate.Date, nowAux.Date) <= 0 && DateTime.Compare(nowAux.Date, w.endDate.Date) <= 0).ToList();

            var currentAux = db.CalendarPriceList.FirstOrDefault(fod => fod.id == id_current);
            if (currentAux != null && !calendarPriceListAux.Contains(currentAux)) calendarPriceListAux.Add(currentAux);

            return calendarPriceListAux.Select(s => new {
                s.id,
                name = s.CalendarPriceListType.name + " [" + s.startDate.ToString("dd/MM/yyyy") + " - " + 
                                                            s.endDate.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();

        }

        public static CalendarPriceList CalendarPriceList(int id)
        {
            db = new DBContext();
            return db.CalendarPriceList.FirstOrDefault(p => p.id == id);// && p.isActive);
        }

        public static CalendarPriceList CalendarPriceListById(int? id_CalendarPriceList)
        {
            db = new DBContext();
            return db.CalendarPriceList.FirstOrDefault(v => v.id == id_CalendarPriceList);
        }

    }
}