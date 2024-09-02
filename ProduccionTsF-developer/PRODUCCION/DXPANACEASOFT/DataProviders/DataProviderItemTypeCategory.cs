using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderItemTypeCategory
	{
		private static DBContext db = null;

		public static IEnumerable ItemsTypesCategories(int? id_company)
		{
			db = new DBContext();
			var model = db.ItemTypeCategory.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static ItemTypeCategory ItemTypeCategoryById(int? id)
		{
			db = new DBContext();
			return db.ItemTypeCategory.FirstOrDefault(c => c.id == id);
		}

		public static IEnumerable ItemsTypesCategoriesByItemType(int? id_itemType)
		{
			db = new DBContext();
			// var query = db.ItemTypeCategory.Where(t => t.id_itemType == id_itemType && t.isActive);
			var query = db.ItemTypeCategory.Where(g => g.ItemTypeItemTypeCategory.Any(a => a.id_itemType == id_itemType) && g.isActive).ToList();
			return query.ToList();
		}

		public static IEnumerable ItemsTypesCategoriesSimplified(int? id_company)
		{
			var db = new DBContext();

			return (
				from categorias in db.ItemTypeCategory
				join categoriasTipos in db.ItemTypeItemTypeCategory
					on categorias.id equals categoriasTipos.id_itemTypeCategory
				where categorias.isActive && categorias.id_company == id_company
				select new
				{
					categorias.id,
					categorias.name,
					categoriasTipos.id_itemType,
				})
				.ToArray();
		}
	}
}