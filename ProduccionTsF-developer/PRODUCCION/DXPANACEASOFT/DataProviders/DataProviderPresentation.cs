using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPresentation
    {
        private static DBContext db = null;

        public static IEnumerable Presentations(int id_company)
        {
            db = new DBContext();
            var model = db.Presentation.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }


        public static IEnumerable PresentationsByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.Presentation.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static Presentation Presentation(int? id)
        {
            db = new DBContext();
            return db.Presentation.FirstOrDefault(t => t.id == id && t.isActive);
        }

        public static Presentation PresentationById(int? id)
        {
            db = new DBContext();
            var query = db.Presentation.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}