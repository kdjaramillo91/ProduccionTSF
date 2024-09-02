using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTaxType
    {
        private static DBContext db = null;

        public static IEnumerable TaxTypes(int id_company)
        {
            db = new DBContext();
            var model = db.TaxType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable TaxTypeFilter(int id_company)
        {
            db = new DBContext();
            var model = db.TaxType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable TaxTypesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.TaxType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static TaxType TaxTypeById(int? id)
        {
            db = new DBContext();
            var query = db.TaxType.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}