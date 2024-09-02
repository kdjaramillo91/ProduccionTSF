 using System.Collections;
 using System.Collections.Generic;
 using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderProductionLotState
    {
        private static DBContext db = null;

        public static IEnumerable ProductionLotStates()
        {
            db = new DBContext();
            return db.ProductionLotState.ToList();
        }
        public static IEnumerable AllProductionLotStatesByCompany(int? id_company)
        {
            db = new DBContext();

            return db.ProductionLotState.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static IEnumerable ProductionLotStatesByCompany(int? id_company)
        {
            db = new DBContext();
            List<ProductionLotState> productionLotStates = db.ProductionLotState.Where(t => t.id_company == id_company && t.isActive).ToList();
            return productionLotStates ?? new List<ProductionLotState>();
        }

        public static ProductionLotState ProductionLotStateById(int? id)
        {
            db = new DBContext();
            return db.ProductionLotState.FirstOrDefault(t => t.id == id);
        }
    }
}