using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderClientCategory
    {
        private static DBContext db = null;

        public static IEnumerable ClientCategories(int id_company)
        {
            db = new DBContext();
            var model = db.ClientCategory.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable ClientCategoriesFilter(int id_company)
        {
            db = new DBContext();
            var model = db.ClientCategory.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable ClientCategoriesByCompany(int? id_company)
        {
            db = new DBContext();
            var query = db.ClientCategory.Where(t => t.id_company == id_company && t.isActive);
            return query.ToList();
        }

        public static ClientCategory ClientCategoryById(int? id)
        {
            db = new DBContext();
            var query = db.ClientCategory.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable ClientCategoryByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.ClientCategory.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable ClientCategoryAll(int? id_current)
        {
            db = new DBContext();
            var categoryaux = db.ClientCategory.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

            if (id_current != null && id_current > 0)
            {
                var cant = (from de in categoryaux
                            where de.id == id_current
                            select de).ToList().Count;
                if (cant == 0)
                {
                    var Categorycuuretaux = db.ClientCategory.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

                    categoryaux.AddRange(Categorycuuretaux);
                }
            }

            return categoryaux.OrderBy(x => x.name);

        }
        public static int? ClientCategorybyCompanyDefault(int? id_company)
        {
            db = new DBContext();
            return db.ClientCategory.FirstOrDefault(i => i.id_company == id_company && i.byDefault)?.id;
        }
    }
}