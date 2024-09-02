using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderGroupPersonByRol
    {
        private static DBContext db = null;

        public static IEnumerable GroupPersonByRols(int id_company)
        {
            db = new DBContext();
            var model = db.GroupPersonByRol.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable GroupPersonByRolsByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.GroupPersonByRol.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }
        public static IEnumerable AllGroupPersonByRolsByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.GroupPersonByRol.Where(p => p.id_company == id_company).ToList();

            return model;
        }

        public static GroupPersonByRol GroupPersonByRolById(int? id)
        {
            db = new DBContext();
            return db.GroupPersonByRol.FirstOrDefault(v => v.id == id);
        }

        public static GroupPersonByRol GroupPersonByRol(int? id)
        {
            db = new DBContext();
            return db.GroupPersonByRol.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable GroupPersonByRolByCompanyCurrentAndRol(int? id_company, int? id_current, int? id_rol)
        {
            db = new DBContext();
            return db.GroupPersonByRol.Where(g => (g.isActive && g.id_company == id_company && g.id_rol == id_rol) ||
                                                 g.id == (id_current == null ? 0 : id_current))
                                                 .Select(s=> new { id = s.id, name =  s.name + "(" + (s.Rol.name) + ")"}).ToList();
        }

        public static IEnumerable GroupPersonByRolByCompanyCurrentAndRols(int? id_company, int? id_current, List<string> rols)
        {
            db = new DBContext();
            var groupPersonByRolsAux = db.GroupPersonByRol.Where(g => (g.isActive && g.id_company == id_company ) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
            //.Select(s => new { id = s.id, name = s.name + "(" + (s.Rol.name) + ")" }).ToList();
            List<GroupPersonByRol> resultAux = new List<GroupPersonByRol>();
            foreach (var rol in rols)
            {
                var tempGroupPersonByRols = groupPersonByRolsAux.Where(g => (g.Rol.name == rol) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
                foreach (var group in tempGroupPersonByRols)
                {
                    resultAux.Add(group);
                }
            }
                return resultAux.Select(s => new { id = s.id, name = s.name + "(" + (s.Rol.name) + ")" }).ToList();
        }

    }
}