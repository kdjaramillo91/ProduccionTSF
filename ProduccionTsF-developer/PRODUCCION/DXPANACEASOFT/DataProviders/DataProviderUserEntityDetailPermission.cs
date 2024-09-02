using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderUserEntityDetailPermission
	{
		private static DBContext db = null;

		public static UserEntityDetailPermission UserEntityDetailPermissionById(int? id_UserEntityDetailPermission)
		{
			db = new DBContext();

			var UserEntityDetailPermission = db.UserEntityDetailPermission.Where(t => t.id == id_UserEntityDetailPermission).FirstOrDefault();
			return UserEntityDetailPermission;
		}
	}
}