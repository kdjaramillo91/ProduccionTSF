using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderSimpleFormula
	{
		public static SimpleFormula SimpleFormulaById(int? id)
		{
			return new DBContext()
				.SimpleFormula
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable Types(string id_current)
		{
			return new ListaTipoFormulaSimple()
				.LsTipoFormulaSimple
				.Select(det => new SimpleComboBox()
				{
					id = det.Codigo,
					name = det.Descripcion,
				})
				.Where(t => t.id == (id_current ?? t.id))
				.ToList();
		}
		public static IEnumerable GetSimpleFormulaByType(string type)
		{
			return new DBContext()
				.SimpleFormula
				.Where(t => t.type == type)
				.ToList();
		}
		public static IEnumerable Datasources(string id_current)
		{
			return new[]
			{
				new SimpleComboBox { id = "InventoryReason", name = "Motivos de Inventario" },
			}
			.Where(t => t.id == (id_current ?? t.id))
			.ToList();
		}
		public static SimpleComboBox TypeById(string id_current)
		{
			return new[]
			{
				new SimpleComboBox { id = "TIPOBODEGA", name = "Tipo Bodega" },
			}
			.FirstOrDefault(w => w.id == id_current);
		}
		public static SimpleComboBox DatasourceById(string id_current)
		{
			return new[]
			{
				new SimpleComboBox { id = "InventoryReason", name = "Motivos de Inventario" },
			}
			.FirstOrDefault(w => w.id == id_current);
		}
		public static List<SimpleComboBox> SimpleDataSourceDataById(string id_datasource)
		{
			if (id_datasource == "InventoryReason")
			{
				return new DBContext()
					.InventoryReason
					.Where(t => t.isActive)
					.Select(t => new SimpleComboBox()
					{
						id = t.id.ToString(),
						name = t.name,
					}
					).ToList();
			}
			else
			{
				return new List<SimpleComboBox>();
			}
		}
	}
}