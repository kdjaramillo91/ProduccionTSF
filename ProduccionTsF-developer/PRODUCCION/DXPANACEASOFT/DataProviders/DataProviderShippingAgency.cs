using DXPANACEASOFT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderShippingAgency
	{
		private static DBContext db = null;

		public static IEnumerable ShippingAgencyAll()
		{
			db = new DBContext();
			return db.ShippingAgency.Where(r => r.isActive).OrderBy(o => o.name).ToList();
		}

		public static IEnumerable AllShippingAgency()
		{
			db = new DBContext();
			var model = db.ShippingAgency.ToList();

			return model;
		}

		public static ShippingAgency ShippingAgencyById(int? id_shippingAgency)
		{
			db = new DBContext(); ;
			return db.ShippingAgency.FirstOrDefault(v => v.id == id_shippingAgency);
		}

		public static IEnumerable ShippingAgencyWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.ShippingAgency.Where(g => (g.isActive) || g.id == id_current).ToList();
		}

		public static IEnumerable StateOfContryByCountryAndCurrent(int? id_Country, int? id_current)
		{
			db = new DBContext();



			var StateOfContryAux = db.StateOfContry.Where(t => t.id == id_Country && t.isActive).ToList() ?? new List<StateOfContry>();


			var currentAux = db.StateOfContry.FirstOrDefault(fod => fod.id == id_current);
			if (currentAux != null && !StateOfContryAux.Contains(currentAux)) StateOfContryAux.Add(currentAux);

			return StateOfContryAux;
		}

		public static IEnumerable ShippingAgencyAll(int? id_current)
		{
			db = new DBContext();
			var ShippingAgencyaux = db.ShippingAgency.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in ShippingAgencyaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var ShippingAgencycuuretaux = db.ShippingAgency.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

					ShippingAgencyaux.AddRange(ShippingAgencycuuretaux);
				}
			}

			return ShippingAgencyaux.OrderBy(x => x.name);

		}
	}
}