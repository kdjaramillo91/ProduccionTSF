using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderDepartment
    {
        private static DBContext db = null;
       
        public static IEnumerable Departments(int? id_company)
        {db = new DBContext();
            var model = db.Department.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static Department Department(int id)
        {
            db = new DBContext();
            return db.Department.FirstOrDefault(i => i.id == id && i.isActive);
        }

        public static Department DepartmentById(int? id)
        {
            db = new DBContext();
            var query = db.Department.FirstOrDefault(t => t.id == id);
            return query;
        }

    }
}