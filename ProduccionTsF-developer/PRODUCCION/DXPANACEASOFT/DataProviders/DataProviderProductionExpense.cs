using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionExpense
	{
		private static DBContext _db = null;

		public static ProductionExpense ProductionExpenseById(int? id)
		{
			_db = new DBContext();
			var query = _db.ProductionExpense.FirstOrDefault(t => t.id == id);
			return query;
		}
		public static IEnumerable ProductionExpenseByIdProductionCost(int? id_productionCost)
		{
			using (_db = new DBContext())
			{
				return _db.ProductionExpense.Where(w => w.isActive && w.id_productionCostType == id_productionCost).ToList();
			}
		}
		public static IEnumerable AccountTemplateByIdProductionCostIdProductionExpense(int? id_productionCost, int? id_productionExpense )
		{
			using (_db = new DBContext())
			{
				return _db.AccountingTemplate
					.Where(w => w.isActive 
					&& w.id_costProduction == id_productionCost
					&& w.id_expenseProduction == id_productionExpense)
					.ToList();
			}
		}
		public static IEnumerable ProductionExpenseByCompany(int id_company)
        {
			using (_db = new DBContext())
			{
				return _db.ProductionExpense.Where(w => w.isActive && w.id_company == id_company).ToList();
			}
        }
		public static IEnumerable ProductionExpenseByCompanyAndCurrent(int? id_company, int? id_current)
		{
			_db = new DBContext();
			return _db.ProductionExpense.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id_productionCostType == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable ProductionExpenseByCompanyAndCurrent(int? id_current)
		{
			_db = new DBContext();
			return _db.ProductionExpense.Where(w => w.id_productionCostType == id_current && w.isActive).ToList();

		}
		public static IEnumerable ProductionExpenseTypeFilter(int? id_company)
		{
			_db = new DBContext();
			return _db.ProductionExpense.Where(g => (g.isActive && g.id_company == id_company)).ToList();
		}

	}
}