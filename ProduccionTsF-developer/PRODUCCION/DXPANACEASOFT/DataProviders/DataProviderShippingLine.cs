using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderShippingLine
	{
		private static DBContext db = null;

		public static IEnumerable InvoiceExteriorShippingLineByShippingAgencyandCurrent(int? id_ShippingAgency, int? id_ShippingLine)
		{
			db = new DBContext();

			var model = db.ShippingLineShippingAgency.Where(r =>
						(r.id_ShippingAgency == id_ShippingAgency  /* && activo por fecha */)
						  || ((r.id_ShippingAgency == id_ShippingAgency   /* && activo por fecha */ )
								 && r.id_ShippingLine == (id_ShippingLine == null ? 0 : id_ShippingLine)))
				.Select(r => new { id = r.ShippingLine.id, code = r.ShippingLine.code, description = r.ShippingLine.name })
				.ToList();

			return model;
		}

		public static ShippingLine ShippingLineById(int? id_ShippingLine)
		{
			db = new DBContext();



			var ShippingLine = db.ShippingLine.Where(t => t.id == id_ShippingLine).FirstOrDefault();




			return ShippingLine;
		}

		public static IEnumerable ShippingLineAll(int? id_current)
		{
			db = new DBContext();
			var ShippingLineaux = db.ShippingLine.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in ShippingLineaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var ShippingLinecuuretaux = db.ShippingLine.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

					ShippingLineaux.AddRange(ShippingLinecuuretaux);
				}
			}

			return ShippingLineaux.OrderBy(x => x.name);

		}
	}
}