using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionCostExecutionType
	{
		public static IEnumerable ProductionCostExecutionTypes()
		{
			return new DBContext()
				.ProductionCostExecutionType
				.Where(t => t.isActive)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
		public static ProductionCostExecutionType ProductionCostExecutionTypeById(int? id)
		{
			return new DBContext()
				.ProductionCostExecutionType
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable ProductionCostExecutionTypeByCurrent(int? id_current)
		{
			return new DBContext()
				.ProductionCostExecutionType
				.Where(t => t.isActive || t.id == id_current)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
	}
}