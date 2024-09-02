using DXPANACEASOFT.Models;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderRol
    {
        private static DBContext db = null;

        #region Constantes
        public const string m_RolComprador = "Comprador";
        public const string m_RolClienteExterior = "Cliente Exterior";
        public const string m_RolClienteLocal = "Cliente Local";
        public const string m_RolProveedor = "Proveedor";
        #endregion

        public static Rol Rol(int id)
        {
            db = new DBContext();
            return db.Rol.FirstOrDefault(i => i.id == id);
        }

        public static Rol RoById(int? id)
        {
            db = new DBContext();
            return db.Rol.FirstOrDefault(i => i.id == id);
        }
        
        public static Rol[] RoByIds(int[] ids)
        {
            db = new DBContext();
            return db.Rol
                .Where(i => ids.Contains(i.id))
                .ToArray();
        }

        public static IEnumerable<Rol> Rols()
        {
            db = new DBContext();
            return db.Rol;
        }

        public static List<Rol> Rol()
        {
            db = new DBContext();
            return db.Rol.ToList();
        }

        public static List<Rol> RolCustomerProvider()
        {
            db = new DBContext();
            return db.Rol
                .Where(i => i.name == m_RolClienteExterior || i.name == m_RolProveedor || i.name == m_RolClienteLocal)
                .ToList();
        }

        public static List<Rol> ListWithCurrent(int? id_rol)
        {
            db = new DBContext();
            return db.Rol.Where(w => w.id == id_rol).ToList();
        }
    }
}