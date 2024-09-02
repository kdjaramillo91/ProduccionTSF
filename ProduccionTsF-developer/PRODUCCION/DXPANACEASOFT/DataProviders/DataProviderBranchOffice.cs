using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBranchOffice
    {
        private static DBContext db = null;

        public static IEnumerable BranchOffices(int id_company)
        {
            db = new DBContext();
            var model = db.BranchOffice.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable BranchOfficesFilter(int id_company)
        {
            db = new DBContext();
            var model = db.BranchOffice.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable AllBranchOffices()
        {
            db = new DBContext();
            return db.BranchOffice.ToList();
        }
        public static IEnumerable BranchOfficesByDivision(int? id_division)
        {
            db = new DBContext();
            var query = db.BranchOffice.Where(t => t.id_division == id_division && t.isActive);
            return query.ToList();
        }

        public static BranchOffice BranchOfficeById(int? id)
        {
            db = new DBContext();
            var query = db.BranchOffice.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable BranchOfficesByDivisionAndCurrent(int? id_division, int? id_current)
        {
            db = new DBContext();
            return db.BranchOffice.Where(g => (g.isActive && g.id_division == id_division) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}