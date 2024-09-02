using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderShippingLineShippingAgency
	{
		private static DBContext db = null;

		public static ShippingLineShippingAgency ShippingLineShippingAgencyById(int? id_ShippingLineShippingAgency)
		{
			db = new DBContext();

			var ShippingLineShippingAgency = db.ShippingLineShippingAgency.Where(t => t.id == id_ShippingLineShippingAgency).FirstOrDefault();
			return ShippingLineShippingAgency;
		}
	}
}