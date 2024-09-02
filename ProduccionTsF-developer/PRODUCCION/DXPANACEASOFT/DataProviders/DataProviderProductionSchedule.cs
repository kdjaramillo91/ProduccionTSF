using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DXPANACEASOFT.Models;
using System.Globalization;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionSchedule
    {
        private static DBContext db = null;

        //public static IEnumerable ProductionLotStates()
        //{
        //    db = new DBContext();
        //    return db.ProductionLotState.ToList();
        //}

        public static IEnumerable ProductionSchedulesByCompany(int? id_company)
        {
            db = new DBContext();
            var productionSchedules = db.ProductionSchedule.Where(t => t.id_company == id_company).ToList();
            return productionSchedules;// ?? new List<ProductionSchedule>();
        }

        public static ProductionSchedule ProductionSchedule(int? id_productionSchedule)
        {
            db = new DBContext();
            var productionSchedule = db.ProductionSchedule.FirstOrDefault(t => t.id == id_productionSchedule);
            return productionSchedule;// ?? new List<ProductionSchedule>();
        }

        //public static IEnumerable ProductionSchedulesByCompanyAndCurrentWithout(int? id_company, int? id_current)
        //{
        //    db = new DBContext();
        //    return db.ProductionSchedulePeriod.Where(g => (g.isActive && g.id_company == id_company && !g.ProductionSchedule.Any()) ||
        //                                                    g.id == (id_current ?? 0)).AsEnumerable().Select(s=> new { s.id, periodo = s.name
        //                                                        //periodo = s.dateStar.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateStar.ToString("_dd") + "(" + s.dateStar.ToString("yyyy") + ")" + " - " +
        //                                                        //          s.dateEnd.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateEnd.ToString("_dd") + "(" + s.dateEnd.ToString("yyyy") + ")"
        //                                                    }).ToList();
        //}
    }
}