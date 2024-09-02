using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderCategoryCustomerType
	{
		private static DBContext db = null;

		public static CategoryCustomerType CategoryCustomerTypeById(int? id_CategoryCustomerType)
		{
			db = new DBContext();

			var CategoryCustomerType = db.CategoryCustomerType.Where(t => t.id == id_CategoryCustomerType).FirstOrDefault();
			return CategoryCustomerType;
		}
	}
}