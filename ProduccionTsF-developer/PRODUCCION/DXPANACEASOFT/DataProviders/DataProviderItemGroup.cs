using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderItemGroup
	{
		private static DBContext db = null;

		public static IEnumerable ItemGroups(int id_company)
		{
			db = new DBContext();
			var model = db.ItemGroup.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company &&
								p.id_parentGroup == null).ToList();
			}

			return model;
		}

		public static IEnumerable ItemGroupFilter(int id_company)
		{
			db = new DBContext();
			var model = db.ItemGroup.ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable ItemGroupsSubGroupsByCompanyAndCurrent(int? id_company, int? id_itemGroupCurrent)
		{
			db = new DBContext();

            if (id_company.HasValue)
            {
				var grupos = db.ItemGroup
					.Where(e => e.id_company == id_company.Value && e.isActive)
					.ToArray();

                if (id_itemGroupCurrent.HasValue)
                {
					grupos = grupos.Where(e => e.id == id_itemGroupCurrent.Value).ToArray();
				}

				return grupos;
			}

			return null;
		}

		public static IEnumerable ItemGroupsDifferentOfGroupCurrentByCompanyAndCurrentParent(int? id_groupCurrent, int? id_company, int? id_itemGroupCurrentParent)
		{
			db = new DBContext();
			return db.ItemGroup.Where(g => (g.id != id_groupCurrent && g.isActive &&
										   g.id_company == id_company) || g.id == (id_itemGroupCurrentParent == null ? 0 : id_itemGroupCurrentParent)).ToList();
		}

		public static IEnumerable ItemSubGroups()
		{
			db = new DBContext();
			return db.ItemGroup.Where(g => g.id_parentGroup != null && g.isActive).ToList();
		}

		public static IEnumerable ItemSubGroupsOfGroup(int? id_itemGroup)
		{
			db = new DBContext();
			int id = id_itemGroup ?? 0;
			return db.ItemGroup.Where(g => g.id_parentGroup == id && g.isActive).ToList();
		}

		public static ItemGroup ItemGroupById(int? id_itemGroup)
		{
			db = new DBContext();
			var query = db.ItemGroup.FirstOrDefault(t => t.id == id_itemGroup);
			return query;
		}

		public static IEnumerable ItemGroupCategories()
		{
			db = new DBContext();
			return db.ItemGroupCategory.Where(t => t.isActive).ToList();
		}

		public static IEnumerable ItemCategoriesOfGroup(int? id_itemGroup)
		{
			db = new DBContext();
			int id = id_itemGroup ?? 0;
			return db.ItemGroupCategory.Where(g => g.ItemGroupItemGroupCategory.Any(a => a.id_itemGroup == id) && g.isActive).ToList();
		}


		public static ItemGroupCategory ItemGroupCategoriesById(int? id_itemGroupCategory)
		{
			db = new DBContext();
			int id = id_itemGroupCategory ?? 0;
			var query = db.ItemGroupCategory.FirstOrDefault(t => t.id == id && t.isActive);
			return query;
		}

		public static IEnumerable ItemGroupsSimplified(int? id_company)
		{
			var db = new DBContext();
			return db.ItemGroup
				.Where(t => t.isActive && t.id_company == id_company && t.id_parentGroup > 0)
				.Select(t => new
				{
					t.id,
					t.name,
					t.id_parentGroup,
				})
				.ToArray();
		}
	}
}