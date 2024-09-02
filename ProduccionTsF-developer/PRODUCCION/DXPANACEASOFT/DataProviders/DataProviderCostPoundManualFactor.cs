using System.Linq;
using DXPANACEASOFT.Models;
using System.Collections;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCostPoundManualFactor
    {
        private static DBContext db = null;

        public static CostPoundManualFactor CostPoundManualFactorById(int? id)
        {
            db = new DBContext();
            var query = db.CostPoundManualFactor.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}
