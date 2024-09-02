using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderVendor
    {
        private static DBContext db = null;

        #region {RA} - Get 
        public static IEnumerable VendorList()
        {
            db = new DBContext();
            var rolVendedor = db.Rol.FirstOrDefault(r => r.name.Equals("Vendedor"));
            if (rolVendedor == null)
                throw new Exception("Rol Vendedor no existe en la base de datos. Tabla Rol name Vendedor");
            return db.Person.Where(p => p.Rol.FirstOrDefault(r => r.id == rolVendedor.id) != null && p.isActive).Select(v => new
            {
                v.id,
                v.identification_number,
                v.fullname_businessName
            }).ToList();
        }

        public static Person VendorById(int id_vendor)
        {
            db = new DBContext();
            return db.Person.FirstOrDefault(p => p.id == id_vendor);
        }

        #endregion
    }
}