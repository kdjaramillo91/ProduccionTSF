using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderClientCategoryTypeBusiness
	{
		private static DBContext db = null;

		public static ClientCategoryTypeBusiness ClientCategoryTypeBusinessById(int? id_ClientCategoryTypeBusiness)
		{
			db = new DBContext();

			var ClientCategoryTypeBusiness = db.ClientCategoryTypeBusiness.Where(t => t.id == id_ClientCategoryTypeBusiness).FirstOrDefault();
			return ClientCategoryTypeBusiness;
		}
	}
}