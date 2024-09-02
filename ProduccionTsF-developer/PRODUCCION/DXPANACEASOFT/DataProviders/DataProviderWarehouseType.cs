using DXPANACEASOFT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderWarehouseType
	{
		public static IEnumerable WarehousesTypes(int id_company)
		{
			var model = new DBContext()
				.WarehouseType
				.Where(t => t.isActive);

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company);
			}

			return model.ToList();

		}
		public static IEnumerable WarehousesTypeFilter(int id_company)
		{
			var model = new DBContext()
				.WarehouseType
				.AsEnumerable();

			if (id_company != 0)
			{
				model = model
					.Where(p => p.id_company == id_company);
			}

			return model.ToList();

		}
		public static IEnumerable WarehousesTypesByCompany(int? id_company)
		{
			return new DBContext()
				.WarehouseType
				.Where(t => t.id_company == id_company && t.isActive)
				.ToList();
		}
		public static WarehouseType WarehouseTypeById(int? id)
		{
			return new DBContext()
				.WarehouseType
				.FirstOrDefault(t => t.id == id);
		}
		public static IEnumerable WarehouseTypesByCompanyAndCurrent(int? id_company, int? id_current)
		{
			return new DBContext()
				.WarehouseType
				.Where(g => (g.isActive && g.id_company == id_company)
							|| g.id == (id_current == null ? 0 : id_current))
				.ToList();
		}
		public static IEnumerable ProductionCostings()
		{
			var lsProductionCosting = new List<SimpleComboBox>();
			lsProductionCosting.Add(new SimpleComboBox { id = "SI", name = "SI" });
			lsProductionCosting.Add(new SimpleComboBox { id = "NO", name = "NO" });

			return lsProductionCosting;
		}
		public static IEnumerable PoundsTypes()
		{
			var lsProductionCosting = new List<SimpleComboBox>();
			ListaTipoLibrasCosteoProducion lsTipoLibras = new ListaTipoLibrasCosteoProducion();
			foreach (var det in lsTipoLibras.LsTipoLibraCosteoProduccion)
			{
				lsProductionCosting.Add(new SimpleComboBox { id = det.Codigo, name = det.Descripcion });
			}

			return lsProductionCosting;
		}
	}
}