using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderDataType
    {
        private static DBContext db = null;

        public static IEnumerable DataTypes(int id_company)
        {
            db = new DBContext();
            var model = db.DataType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable DataTypeFilter(int id_company)
        {
            db = new DBContext();
            var model = db.DataType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }
            return model;
        }


        public static DataType DataTypeById(int? id)
        {
            db = new DBContext();
            var query = db.DataType.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}