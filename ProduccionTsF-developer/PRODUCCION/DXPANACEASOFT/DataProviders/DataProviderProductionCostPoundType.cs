using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionCostPoundType
	{
		public static IEnumerable ProductionCostPoundTypes()
		{
			return new DBContext()
				.ProductionCostPoundType
				.Where(t => t.isActive)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}
		public static ProductionCostPoundType ProductionCostPoundTypeById(int? id)
		{
			return new DBContext()
				.ProductionCostPoundType
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable ProductionCostPoundTypeByCurrent(int? id_current)
		{
			return new DBContext()
				.ProductionCostPoundType
				.Where(t => t.isActive || t.id == id_current)
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();
		}

		public class EnableProductionCostValue
		{
			public bool Value { get; set; }
			public string Text { get; set; }
		}
		public static IEnumerable GetEnableProductionCostValues()
		{
			return new[]
			{
				new EnableProductionCostValue{ Value = true, Text = "SÍ" },
				new EnableProductionCostValue{ Value = false, Text = "NO" },
			};
		}
	}
}