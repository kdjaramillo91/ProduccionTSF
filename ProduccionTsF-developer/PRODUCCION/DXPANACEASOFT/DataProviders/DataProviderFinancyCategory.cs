using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderFinancyCategory
    {
        private static DBContext db = null;

        public static IEnumerable GetAllActive()
        {
            db = new DBContext();
            var list = db.FinancyCategory.Where(c => c.isActive).ToList();
            return list;
        }
		public static IEnumerable AllFinancyCategorys()
		{
			db = new DBContext();
			var model = db.FinancyCategory.ToList();
			return model;
		}

		public static FinancyCategory FinancyCategoryById(int? id_FinancyCategory)
		{
			db = new DBContext();
			return db.FinancyCategory.FirstOrDefault(v => v.id == id_FinancyCategory);
		}

		public static IEnumerable FinancyCategoryWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.FinancyCategory.Where(g => (g.isActive) || g.id == id_current).ToList();
		}
	}
}