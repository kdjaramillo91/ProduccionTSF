using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPermission
    {
        private static DBContext db = null;

        public static Permission Permission(int? id)
        {
            db = new DBContext();
            return db.Permission.FirstOrDefault(i => i.id == id);
        }

        public static List<Permission> Permissions()
        {
            db = new DBContext();
            return db.Permission.ToList();
        }

        public static IEnumerable PermissionAll(int? id_current)
        {
            db = new DBContext();
            var permissionaux = db.Permission.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

            if (id_current != null && id_current > 0)
            {
                var cant = (from de in permissionaux
                            where de.id == id_current
                            select de).ToList().Count;
                if (cant == 0)
                {
                    var Permissioncuuretaux = db.Permission.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

                    permissionaux.AddRange(Permissioncuuretaux);
                }
            }

            return permissionaux.OrderBy(x => x.name);

        }
    }
}