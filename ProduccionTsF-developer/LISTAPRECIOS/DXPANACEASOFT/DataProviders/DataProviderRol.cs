using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderRol
    {
        private static DBContext db = null;

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

        public static List<Rol> ListWithCurrent(int? id_rol)
        {
            db = new DBContext();
            return db.Rol.Where(w=> w.id == id_rol).ToList();
        }
    }
}