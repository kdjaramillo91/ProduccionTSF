using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionCost
	{
		public static IEnumerable ProductionCosts()
		{
			return new DBContext()
				.ProductionCost
				.Where(t => t.isActive)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
		public static ProductionCost ProductionCostById(int? id)
		{
			return new DBContext()
				.ProductionCost
				.FirstOrDefault(t => t.id == id);
		}
		public static ProductionCostDetail ProductionCostDetailById(int? id)
		{
			return new DBContext()
				.ProductionCostDetail
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable ProductionCostByCurrent(int? id_current)
		{
			return new DBContext()
				.ProductionCost
				.Where(t => t.isActive || t.id == id_current)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}



		#region Métodos del diseño anterior pendientes de eliminar

		// TODO: Métodos del diseño anterior pendientes de eliminar

		private static DBContext _db = null;

		public static ProductionCosts ProductionCostByIdOld(int? id)
		{
			_db = new DBContext();
			var query = _db.ProductionCosts.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static ProductionCosts ProductionCostTypeByIdOld(int? id)
		{
			_db = new DBContext();
			var query = _db.ProductionCosts.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static IEnumerable ProductionCostTypeFilterOld(int id_company)
		{
			_db = new DBContext();
			var model = _db.ProductionCosts.ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;

		}

		public static IEnumerable ProductionExpenseByCompanyAndCurrentOld(int? id_company, int? id_current)
		{
			_db = new DBContext();
			return _db.ProductionCosts.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		#endregion
	}
}