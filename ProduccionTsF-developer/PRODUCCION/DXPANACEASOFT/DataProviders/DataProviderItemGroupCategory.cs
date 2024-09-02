using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderItemGroupCategory
    {
        private static DBContext db = null;

        public static IEnumerable ItemGroupCategories(int id_company)
        {
            db = new DBContext();
            var model = db.ItemGroupCategory.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable ItemCategoriesOfGroup(int? id_itemGroup)
        {
            db = new DBContext();
            int id = id_itemGroup ?? 0;
            return db.ItemGroupCategory.Where(g => g.ItemGroupItemGroupCategory.Any(a => a.id_itemGroup == id) && g.isActive).ToList();

            //return db.ItemGroupCategory.Where(g => g.id_itemGroup == id && g.isActive).ToList();
        }

		public static IEnumerable ItemCategories()
		{
			db = new DBContext();
			return db.ItemGroupCategory.Where(g => g.isActive).ToList();

			//return db.ItemGroupCategory.Where(g => g.id_itemGroup == id && g.isActive).ToList();
		}

		public static ItemGroupCategory ItemGroupCategoryById(int? id_itemGroupCategory)
        {
            db = new DBContext();
            var query = db.ItemGroupCategory.FirstOrDefault(t => t.id == id_itemGroupCategory);
            return query;
        }
    }
}