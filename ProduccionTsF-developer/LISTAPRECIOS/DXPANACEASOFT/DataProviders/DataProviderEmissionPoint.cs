using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderEmissionPoint
    {
        private static DBContext db = null;

        public static IEnumerable EmissionPoints(int? id_company)
        {
            db = new DBContext();
            var model = db.EmissionPoint.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }
        public static IEnumerable EmissionPointsFilter(int id_company)
        {
            db = new DBContext();
            var model = db.EmissionPoint.ToList();
            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }
            return model;
        }

        public static IEnumerable EmissionPointsByBranchOffice(int id_branchOffice)
        {
            db = new DBContext();
            var query = db.EmissionPoint.Where(t => t.id_branchOffice == id_branchOffice && t.isActive);
            return query.ToList();
        }

        public static EmissionPoint EmissionPointById(int? id)
        {
            db = new DBContext();
            var query = db.EmissionPoint.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable EmissionPointsOfUser(int? id_user)
        {
            db = new DBContext();
            User user = db.User.FirstOrDefault(u => u.id == id_user);

            if(user != null)
            {
                return user.EmissionPoint.ToList();
            }

            return new List<EmissionPoint>();
        }

    }
}