using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderIdentificationType
    {
        private static DBContext db = null;

        public static IdentificationType IdentificationType(int id)
        {
            db = new DBContext();
            return db.IdentificationType.FirstOrDefault(i => i.id == id);
        }

        public static IdentificationType IdentificationTypeById(int? id)
        {
            db = new DBContext();
            return db.IdentificationType.FirstOrDefault(i => i.id == id);
        }
        public static IEnumerable IdentificationType()
        {
            db = new DBContext();
            return db.IdentificationType.Where(t=>t.is_Active).ToList();
        }

        public static IEnumerable AllIdentificationType()
        {
            db = new DBContext();
            return db.IdentificationType.ToList();
        }

        public static IEnumerable IdentificationTypeWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.IdentificationType.ToList();
        }
        public static IEnumerable Country_IdentificationTypeById(int? id_current)
        {
            db = new DBContext();
            var country_IdentificationType = db.Country_IdentificationType.FirstOrDefault(g => g.id_country == id_current);

             var model = db.IdentificationType.Where(t => t.is_Active).ToList();

            if (id_current != 0)
            {
                model = model.Where(p => country_IdentificationType.id_identificationType == p.id).ToList();
            }

            return model;

        }
    }
}