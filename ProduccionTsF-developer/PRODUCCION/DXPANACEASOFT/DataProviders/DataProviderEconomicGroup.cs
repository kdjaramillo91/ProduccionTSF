using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderEconomicGroup
    {
        private static DBContext db = null;

        public static IEnumerable EconomicGroup()
        {
            db = new DBContext();
            var model = db.EconomicGroup.Where(t => t.isActive).ToList();

            return model;

        }

        public static IEnumerable EconomicGroupFilter()
        {
            db = new DBContext();
            var model = db.EconomicGroup.ToList();

            return model;

        }

        public static EconomicGroup EconomicGroupById(int? id)
        {
            db = new DBContext();
            var query = db.EconomicGroup.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable EconomicGroupsByCurrent(int? id_current)
        {
            db = new DBContext();
            return db.EconomicGroup.Where(g => (g.isActive) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

    }
}