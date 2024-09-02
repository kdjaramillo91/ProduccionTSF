using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DXPANACEASOFT.Models;
using System.Globalization;
using System;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionSchedulePeriod
    {
        private static DBContext db = null;

        //public static IEnumerable ProductionLotStates()
        //{
        //    db = new DBContext();
        //    return db.ProductionLotState.ToList();
        //}
        public static ProductionSchedulePeriod ProductionSchedulePeriodById(int? id)
        {
            db = new DBContext();
            var query = db.ProductionSchedulePeriod.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable ProductionSchedulePeriodsByCompany(int? id_company)
        {
            db = new DBContext();
            var productionSchedulePeriods = db.ProductionSchedulePeriod.Where(t => t.id_company == id_company && t.isActive).Select(s => new {s.id, periodo = s.name
                                                                                                                                            //periodo = s.dateStar.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper()+ s.dateStar.ToString("_dd") + "(" + s.dateStar.ToString("yyyy") + ")" + " - " +
                                                                                                                                            //          s.dateEnd.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateEnd.ToString("_dd") + "("+ s.dateEnd.ToString("yyyy") + ")"
            }).ToList();
            return productionSchedulePeriods;// ?? new List<PurchasePlanningPeriod>();
        }

        public static IEnumerable ProductionSchedulePeriodsByCompanyAndCurrentWithout(int? id_company, int? id_current)
        {
            db = new DBContext();
            var todayAux = DateTime.Now;
            return db.ProductionSchedulePeriod.AsEnumerable().Where(g => (g.isActive && g.id_company == id_company &&
                                                           (DateTime.Compare(todayAux.Date, g.dateStar.Date) >= 0 && 
                                                            DateTime.Compare(g.dateEnd.Date, todayAux.Date) >= 0)/*!g.ProductionSchedule.Any()*/) ||
                                                            g.id == (id_current ?? 0)).Select(s=> new { s.id, periodo = s.name
                                                                //periodo = s.dateStar.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateStar.ToString("_dd") + "(" + s.dateStar.ToString("yyyy") + ")" + " - " +
                                                                //          s.dateEnd.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateEnd.ToString("_dd") + "(" + s.dateEnd.ToString("yyyy") + ")"
                                                            }).ToList();
        }
    }
}