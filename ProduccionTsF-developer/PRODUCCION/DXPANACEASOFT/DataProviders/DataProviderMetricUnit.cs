using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderMetricUnit
	{
		private static DBContext db = null;

		public static MetricUnit MetricUnit(int? id)
		{
			db = new DBContext();
			return db.MetricUnit.FirstOrDefault(i => i.id == id && i.isActive);
		}

		public static IEnumerable MetricUnits(int? id_company)
		{
			db = new DBContext();
			var model = db.MetricUnit.Where(t => t.isActive && t.id_company == id_company).ToList();

			//if (id_company != 0)
			//{
			//    model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			//}

			return model;
		}

		public static IEnumerable MetricUnitFilter(int? id_company)
		{
			db = new DBContext();
			var model = db.MetricUnit.ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company).ToList();
			}

			return model;
		}

		public static IEnumerable MetricsUnitsByMectricUnitConversion(int id_metricOrigin)
		{
			db = new DBContext();
			MetricUnit metricOrigin = db.MetricUnit.FirstOrDefault(u => u.id == id_metricOrigin);
			var query = db.MetricUnit.Where(t => t.id_metricType == metricOrigin.id_metricType && t.id != metricOrigin.id && t.isActive).ToList();
			return query;
		}

		public static IEnumerable MectricUnitByMetricsTypes(int id)
		{
			db = new DBContext();
			MetricType metricType = db.MetricType.FirstOrDefault(t => t.id == id && t.isActive);
			return metricType?.MetricUnit?.ToList() ?? new List<MetricUnit>();
		}

		public static IEnumerable MetricUnitsByCompanyAndCurrent(int? id_company, int? id_current)
		{
			db = new DBContext();
			return db.MetricUnit.Where(g => (g.isActive && g.id_company == id_company) ||
												 g.id == (id_current == null ? 0 : id_current)).ToList();
		}

		public static IEnumerable MectricUnitByCompanyMetricsTypesAndCurrent(int? id_company, int? id_metricType, int? id_current)
		{
			db = new DBContext();

			MetricType metricType = db.MetricType.FirstOrDefault(t => t.id == id_metricType && t.isActive && t.id_company == id_company);

			var metricUnitAux = metricType?.MetricUnit?.ToList() ?? new List<MetricUnit>();
			metricUnitAux = metricUnitAux.Where(g => (g.isActive && g.id_company == id_company)).ToList();

			var currentAux = db.MetricUnit.FirstOrDefault(fod => fod.id == id_current);
			if (currentAux != null && !metricUnitAux.Contains(currentAux)) metricUnitAux.Add(currentAux);

			return metricUnitAux;
		}

		public static IEnumerable MectricUnitByCompanyMetricsTypesAndCurrent(int? id_company, int? id_current)
		{
			db = new DBContext();
			var metricUnitAux = new List<MetricUnit>();
			var currentAux = db.MetricUnit.FirstOrDefault(fod => fod.id == id_current);
			if (currentAux != null)
			{
				metricUnitAux = currentAux.MetricType.MetricUnit.Where(w => (w.isActive && w.id_company == id_company) || w.id == id_current).ToList() ?? new List<MetricUnit>();
			}
			return metricUnitAux;
		}

		public static IEnumerable MetricUnitsDestinyByCompanyOriginAndCurrent(int? id_company, int? id_metricOrigin, int? id_current)
		{
			db = new DBContext();
			MetricUnit metricUnitOrigin = db.MetricUnit.FirstOrDefault(mu => mu.id == id_metricOrigin);
			var metricUnitOriginMetricType_id = (metricUnitOrigin != null )? metricUnitOrigin.MetricType.id:0;
			return db.MetricUnit.Where(d => (d.id_company == id_company &&
													d.isActive &&
													d.id != id_metricOrigin &&
													(metricUnitOriginMetricType_id == d.MetricType.id) &&
													db.MetricUnitConversion.FirstOrDefault(muc => (muc.id_metricOrigin == id_metricOrigin &&
																								  muc.id_metricDestiny == d.id &&
																								  muc.id_company == id_company)) == null) ||
																								  d.id == (id_current == null ? 0 : id_current)).ToList();

		}

		public static MetricUnit MetricUnitById(int? id)
		{
			db = new DBContext();
			var query = db.MetricUnit.FirstOrDefault(t => t.id == id);
			return query;
		}

		public static IEnumerable WeightMetriUnit(int? id_Company)
		{
			db = new DBContext();
			int? id_metricType = db.MetricType.FirstOrDefault(fod => fod.code.Equals("PES01")).id;
			var model = db.MetricUnit.Where(w => w.id_metricType == id_metricType && w.id_company == id_Company & w.isActive).ToList();

			return model;
		}


		public static IEnumerable WeightMetriUnitInvoiceExterior(int? id_Company)
		{
			db = new DBContext();
			int? id_metricType = db.MetricType.FirstOrDefault(fod => fod.code.Equals("PES01")).id;
			MetricUnit metricUnitNoDefinido
							= new Models.MetricUnit
							{
								id =999,
								code = "00",
								name = "No Definido",
								description = "No Definido"
							};

			var model = db.MetricUnit
				.Where(w => w.id_metricType == id_metricType
							&& w.id_company == id_Company & w.isActive)
				.ToList();

			model.Add(metricUnitNoDefinido);

			return model.OrderBy(o => o.code);
		}

		public static IEnumerable ListWithCurrent(int? id_current)
		{
			db = new DBContext();
			var model = db.MetricUnit.Where(t => t.id == id_current).ToList();

			return model;
		}

		public static IEnumerable MetricUnitTypeUMPresentation(int? id_company, int? id_item, int? id_current)
		{
			db = new DBContext();
			var item = db.Item.FirstOrDefault(fod => fod.id == id_item);
			var metricUnits = item?.Presentation.MetricUnit.MetricType.MetricUnit.Where(w => (w.isActive && w.id_company == id_company) || w.id == id_current).ToList() ?? new List<MetricUnit>();

			var model = metricUnits.Select(s => new
			{
				id = s.id,
				code = s.code
			}).ToList();

			return model;
		}

		public static MetricUnit MetricUnitByCode(string code)
		{
			db = new DBContext();
			var query = db.MetricUnit.FirstOrDefault(t => t.code == code);
			return query;
		}

		public static MetricUnit MetricUnitByCodeInvoiceExterior(string code)
		{
			db = new DBContext();
			var query = db.MetricUnit.FirstOrDefault(t => t.code == code);
			return query;
		}

	}
}