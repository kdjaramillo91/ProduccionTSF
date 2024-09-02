using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionCostAllocationType
	{
		public static IEnumerable ProductionCostAllocationTypes()
		{
			return new DBContext()
				.ProductionCostAllocationType
				.Where(t => t.isActive)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
		public static ProductionCostAllocationType ProductionCostAllocationTypeById(int? id)
		{
			return new DBContext()
				.ProductionCostAllocationType
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable ProductionCostAllocationTypeByCurrent(int? id_current)
		{
			return new DBContext()
				.ProductionCostAllocationType
				.Where(t => t.isActive || t.id == id_current)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
	}
}