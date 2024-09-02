using System.Linq;
using DXPANACEASOFT.Models;
using System.Collections;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderFrameworkContract
    {
        private static DBContext db = null;

        public static IEnumerable AllFrameworkContracts()
        {
            db = new DBContext();
            return db.FrameworkContract.ToList();
        }
        //public static IEnumerable FrameworkContracts()
        //{
        //    db = new DBContext();
        //    return db.FrameworkContract.Where(t=>t.isActive).ToList();
        //}

        public static IEnumerable TypeContractFrameworksWithCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.TypeContractFramework.Where(g => (g.isActive && g.id_company == id_company) || g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        //public static IEnumerable CompaniesByBusinessGroup(int id_businessGroup)
        //{
        //    db = new DBContext();
        //    return db.Company.Where(t => t.id_businessGroup == id_businessGroup && t.isActive).ToList();
        //}

        public static FrameworkContract FrameworkContractById(int? id)
        {
            db = new DBContext();
            var query = db.FrameworkContract.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static TypeContractFramework TypeContractFrameworkById(int? id)
        {
            db = new DBContext();
            var query = db.TypeContractFramework.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}