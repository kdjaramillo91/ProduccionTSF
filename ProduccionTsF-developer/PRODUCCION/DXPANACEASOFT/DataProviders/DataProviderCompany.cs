using System.Linq;
using DXPANACEASOFT.Models;
using System.Collections;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCompany
    {
        private static DBContext db = null;

        public static IEnumerable AllCompanies()
        {
            db = new DBContext();
            return db.Company.ToList();
        }
        public static IEnumerable Companies()
        {
            db = new DBContext();
            return db.Company.Where(t => t.isActive).ToList();
        }

        public static IEnumerable CompaniesWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.Company.Where(g => (g.isActive) || g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable CompaniesByBusinessGroup(int id_businessGroup)
        {
            db = new DBContext();
            return db.Company.Where(t => t.id_businessGroup == id_businessGroup && t.isActive).ToList();
        }

        public static Company CompanyById(int? id)
        {
            db = new DBContext();
            var query = db.Company.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}
