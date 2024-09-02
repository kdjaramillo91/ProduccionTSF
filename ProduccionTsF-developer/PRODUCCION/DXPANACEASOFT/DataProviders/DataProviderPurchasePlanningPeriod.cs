using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DXPANACEASOFT.Models;
using System.Globalization;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPurchasePlanningPeriod
    {
        private static DBContext db = null;

        public static IEnumerable ProductionLotStates()
        {
            db = new DBContext();
            return db.ProductionLotState.ToList();
        }

        public static IEnumerable PurchasePlanningPeriodsByCompany(int? id_company)
        {
            db = new DBContext();
            var purchasePlanningPeriods = db.PurchasePlanningPeriod.Where(t => t.id_company == id_company && t.isActive).Select(s => new {s.id, periodo = s.name
                                                                                                                                            //periodo = s.dateStar.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper()+ s.dateStar.ToString("_dd") + "(" + s.dateStar.ToString("yyyy") + ")" + " - " +
                                                                                                                                            //          s.dateEnd.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateEnd.ToString("_dd") + "("+ s.dateEnd.ToString("yyyy") + ")"
            }).ToList();
            return purchasePlanningPeriods;// ?? new List<PurchasePlanningPeriod>();
        }

        public static IEnumerable PurchasePlanningPeriodsByCompanyAndCurrentWithout(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.PurchasePlanningPeriod.Where(g => (g.isActive && g.id_company == id_company && !g.PurchasePlanning.Any()) ||
                                                            g.id == (id_current ?? 0)).AsEnumerable().Select(s=> new { s.id, periodo = s.name
                                                                //periodo = s.dateStar.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateStar.ToString("_dd") + "(" + s.dateStar.ToString("yyyy") + ")" + " - " +
                                                                //          s.dateEnd.ToString("ddd", CultureInfo.CreateSpecificCulture("es-US")).ToUpper() + s.dateEnd.ToString("_dd") + "(" + s.dateEnd.ToString("yyyy") + ")"
                                                            }).ToList();
        }
    }
}