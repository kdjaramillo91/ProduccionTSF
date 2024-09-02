using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderGrammage
    {
        private static DBContext db = null;

        public static IEnumerable GrammagesByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.Grammage.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable Grammages(int? id_company, int? id_current)
        {
            db = new DBContext();
            var model = db.Grammage.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = db.Grammage.Where(p => (p.id_company == id_company && p.isActive) || p.id == id_current).ToList();
            }

            return model;
        }


        public static Grammage GrammageById(int? id)
        {
            db = new DBContext();
            var query = db.Grammage.FirstOrDefault(t => t.id == id);
            return query;
        }


    }
}