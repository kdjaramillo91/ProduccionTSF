using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderBusinessTurn
    {
        private static DBContext db = null;

        public static IEnumerable BusinessTurns(int id_company)
        {
            db = new DBContext();
            var model = db.BusinessLine.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable BusinessTurnsFilter(int id_company)
        {
            db = new DBContext();
            var model = db.BusinessLine.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable BusinessTurnsByCompany(int? id_company)
        {
            db = new DBContext();
            var query = db.BusinessLine.Where(t => t.id_company == id_company && t.isActive);
            return query.ToList();
        }

        public static BusinessLine BusinessTurnById(int? id)
        {
            db = new DBContext();
            var query = db.BusinessLine.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable BusinessTurnByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.BusinessLine.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}