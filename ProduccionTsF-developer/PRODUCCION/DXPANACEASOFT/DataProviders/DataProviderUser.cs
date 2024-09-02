using DXPANACEASOFT.Models;
using System.Collections;
using System.Data.Entity;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderUser
	{
		private static DBContext db = null;

		public static IEnumerable Users()
		{
			db = new DBContext();
			return db.User.Where(t => t.isActive).ToList();
		}

		public static User UserById(int id)
		{
			db = new DBContext();
			return db.User
				.Include(u => u.UserMenu.Select(m => m.Menu))
				.Where(it => it.id == id)
				.FirstOrDefault();
		}

		public static IEnumerable UserAll(int? id_current)
		{
			db = new DBContext();
			var permissionaux = db.User.Where(g => (g.isActive)).Select(p => new { p.id, name = p.username }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in permissionaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var Permissioncuuretaux = db.User.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.username });

					permissionaux.AddRange(Permissioncuuretaux);
				}
			}

			return permissionaux.OrderBy(x => x.name);

		}

		public static IEnumerable GetAllObjectPermissions()
		{
			db = new DBContext();

			var objectPermission = db.ObjectPermission.Where(e => e.isActive);

			return objectPermission.ToList();
		}

		public static ObjectPermission GetObjectsPermission(int? id_objectPermission)
        {
			db = new DBContext();

			var objectPermission = db.ObjectPermission.FirstOrDefault(e => e.id == id_objectPermission);

			return objectPermission;
		}		

		public static IEnumerable UserObjects(int? id)
        {
			db = new DBContext();

			var objectPermissionUser = db.ObjectPermissionUser.Where(e => e.id == id);

			return objectPermissionUser;
		}

		public static ObjectPermissionUser GetObjectPermissionUser(int? idUser)
        {
			db = new DBContext();

			var objectPermissionUser = db.ObjectPermissionUser.FirstOrDefault(e => e.id == idUser);

			return objectPermissionUser;
		}
	}
}