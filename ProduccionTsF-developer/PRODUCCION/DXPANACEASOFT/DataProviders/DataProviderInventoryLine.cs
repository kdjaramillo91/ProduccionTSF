using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderInventoryLine
	{
		public static IEnumerable InventoryLines(int id_company)
		{
			var db = new DBContext();
			var model = db.InventoryLine
				.Where(t => t.isActive);

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company);
			}

			return model.ToList();
		}

		public static IEnumerable InventoryLineFilter(int id_company)
		{
			var db = new DBContext();
			var model = db.InventoryLine
				.Where(w => w.isActive);

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company);
			}

			return model.ToList();
		}

		public static InventoryLine InventoryLineById(int? id)
		{
			var db = new DBContext();
			return db.InventoryLine
				.FirstOrDefault(t => t.id == id);
		}

		public static IEnumerable InventoryLinesByCompanyAndCurrent(int? id_company, int? id_current)
		{
			var db = new DBContext();
			return db.InventoryLine
				.Where(g => (g.id == id_current || g.isActive) && g.id_company == id_company)
				.ToList();
		}

		public static IEnumerable InventoryLinesCartByCart(int id_company)
		{
			var db = new DBContext();
			var model = db.InventoryLine
				.Where(t => t.isActive && (t.code == "PT" || t.code == "PP"));

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company);
			}

			return model.ToList();
		}
		public static IEnumerable InventoryLinesCartPTByCart(int id_company)
		{
			var db = new DBContext();
			var model = db.InventoryLine
				.Where(t => t.isActive && (t.code == "PT"))
				.ToList();

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company)
					.ToList();
			}

			return model.ToList();
		}
	}
}