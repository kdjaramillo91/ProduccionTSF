using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderCostCenter
	{
		private static DBContext db = null;

		public static IEnumerable CostCenters()
		{
			db = new DBContext();
			var model = db.CostCenter.Where(t => t.isActive && t.id_higherCostCenter == null).ToList();

			return model;
		}

		public static IEnumerable AllCostCenters()
		{
			db = new DBContext();
			var model = db.CostCenter.Where(w=> w.id_higherCostCenter == null && w.isActive).ToList();

			return model;
		}

		public static IEnumerable SubCostCenters()
		{
			db = new DBContext();
			var model = db.CostCenter.Where(t => t.isActive && t.id_higherCostCenter != null).ToList();

			return model;
		}

		public static CostCenter SubcostCenterById(int? idCurrent)
        {
			db = new DBContext();
			var model = db.CostCenter.FirstOrDefault(g => g.id == idCurrent);

			return model;
		}

		public static IEnumerable SubCostCentersByCostCenter(int? idHigherCostCenterd)
		{

			if (idHigherCostCenterd == null) new List<CostCenter>();

			db = new DBContext();
			var model = db.CostCenter.Where(t => t.isActive && t.id_higherCostCenter == idHigherCostCenterd).ToList();

			return model;

		}

		public static IEnumerable SubCostCentersByCostCenterAndCurrent(int? idHigherCostCenterd, int? idCurrent)
		{

			//if (idHigherCostCenterd == null) new List<CostCenter>();

			db = new DBContext();
			var model = db.CostCenter.Where(t => (t.isActive && t.id_higherCostCenter == idHigherCostCenterd) || t.id == idCurrent).ToList();

			return model;

		}

		public static IEnumerable AllSubCostCenters()
		{
			db = new DBContext();
			var model = db.CostCenter.Where(t => t.id_higherCostCenter != null).ToList();

			return model;
		}

		public static CostCenter CostCenterById(int? id)
		{
			db = new DBContext();
			return db.CostCenter.FirstOrDefault(t => t.id == id);
		}

		public static IEnumerable CostCentersWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.CostCenter.Where(g => (g.id == id_current || g.isActive)).ToList();
		}
	}

}