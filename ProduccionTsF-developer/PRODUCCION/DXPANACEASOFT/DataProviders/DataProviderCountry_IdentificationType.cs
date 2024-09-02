using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderCountry_IdentificationType
	{
		private static DBContext db = null;

		public static Country_IdentificationType Country_IdentificationTypeById(int? id_CategoryCustomerType)
		{
			db = new DBContext();

			var CategoryCustomerType = db.Country_IdentificationType.Where(t => t.id == id_CategoryCustomerType).FirstOrDefault();
			return CategoryCustomerType;
		}
	}
}