using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderMetricType
    {
        private static DBContext db = null;

        public static IEnumerable MetricTypes(int id_company)
        {
            db = new DBContext();
            var model = db.MetricType.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable MetricTypeFilter(int id_company)
        {
            db = new DBContext();
            var model = db.MetricType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static IEnumerable MetricTypesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.MetricType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static MetricType MetricType(int? id)
        {
            db = new DBContext();
            return db.MetricType.FirstOrDefault(t => t.id == id && t.isActive);
        }

        public static IEnumerable DataTypeByMetricType(int id_metrictype)
        {
            db = new DBContext();
            var query = db.DataType.Where(t => t.id == id_metrictype && t.isActive);
            return query.ToList();
        }

        public static MetricType MetricTypeById(int? id)
        {
            db = new DBContext();
            var query = db.MetricType.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}