using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderItemType
	{
		private static DBContext db = null;

		public static IEnumerable ItemTypes(int? id_company)
		{
			db = new DBContext();
			var model = db.ItemType.Where(t => t.isActive && t.id_company == id_company).ToList();

			//if (id_company != 0)
			//{
			//    model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			//}
			return model;
		}


		public static ItemType ItemTypeById(int? id)
		{
			db = new DBContext();
			return db.ItemType.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable ItemsTypesByInventoryLine(int? id_inventoryLine)
		{
			db = new DBContext();
			var query = db.ItemType.Where(t => t.id_inventoryLine == id_inventoryLine && t.isActive);
			return query.ToList();
		}

		public static IEnumerable ItemsTypesByInventoryLineCompanyCurrent(int? id_company, int? id_inventoryLine, int? id_current)
		{
			db = new DBContext();
			var query = db.ItemType.Where(g => (g.id == id_current || g.isActive) &&
												g.id_company == id_company &&
												g.id_inventoryLine == id_inventoryLine).ToList();
			return query.ToList();
		}

		public static IEnumerable ItemsTypesByCompanyAndCurrent(int? id_company, int? id_current)
		{
			db = new DBContext();
			return db.ItemType.Where(g => (g.id == id_current || g.isActive) && g.id_company == id_company).ToList();
		}

		public static IEnumerable ItemTypesSimplified(int? id_company)
		{
			var db = new DBContext();
			return db.ItemType
				.Where(t => t.isActive && t.id_company == id_company)
				.Select(t => new
				{
					t.id,
					t.name,
					t.id_inventoryLine,
				})
				.ToArray();
		}
	}
}