using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderDivision
    {
        private static DBContext db = null;

        public static IEnumerable Divisions(int id_company)
        {
            db = new DBContext();
            var model = db.Division.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable DivisionsFilter(int id_company)
        {
            db = new DBContext();
            var model = db.Division.ToList();
            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company ).ToList();
            }
            return model;
        }

        public static IEnumerable AllDivisions()
        {
            db = new DBContext();
            return db.Division.ToList();
        }
        public static IEnumerable DivisionsByCompany(int? id_company)
        {
            db = new DBContext();
            var query = db.Division.Where(d => d.id_company == id_company && d.isActive);
            return query.ToList();
        }
            
        public static Division DivisionById(int? id)
        {
            db = new DBContext();
            var query = db.Division.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable DivisionsByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.Division.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}