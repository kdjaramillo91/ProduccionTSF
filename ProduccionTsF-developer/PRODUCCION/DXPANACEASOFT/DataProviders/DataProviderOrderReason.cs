using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderOrderReason
    {
        private static DBContext db = null;

        public static IEnumerable OrderReasons(int id_company)
        {
            db = new DBContext();
            var model = db.OrderReason.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static IEnumerable Reasons()
        {
            var db = new DBContext();
            return db.OrderReason.Where(p => p.isActive).ToList();
        }

        public static IEnumerable OrderReasonsFilter(int id_company)
        {
            db = new DBContext();
            var model = db.OrderReason.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable OrderReasonsByCompany(int? id_company)
        {
            db = new DBContext();
            var query = db.OrderReason.Where(t => t.id_company == id_company && t.isActive);
            return query.ToList();
        }

        public static OrderReason OrderReasonById(int? id)
        {
            db = new DBContext();
            var query = db.OrderReason.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable OrderReasonByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.OrderReason.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}