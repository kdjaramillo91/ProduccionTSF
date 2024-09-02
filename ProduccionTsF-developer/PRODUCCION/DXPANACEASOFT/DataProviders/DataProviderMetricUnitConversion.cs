using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderMetricUnitConversion
    {
        private static DBContext db = null;

        public static MetricUnitConversion MetricUnitConversion(int id)
        {
            db = new DBContext();
            return db.MetricUnitConversion.FirstOrDefault(i => i.id_metricOrigin == id && i.isActive);
        }
        public static MetricUnitConversion MetricUnitConversion(int? id_company, int? id_metricOrigin, int? id_metricDestiny)
        {
            db = new DBContext();
            return db.MetricUnitConversion.FirstOrDefault(muc => muc.id_company == id_company &&
                                                                 muc.id_metricOrigin == id_metricOrigin &&
                                                                 muc.id_metricDestiny == id_metricDestiny &&
                                                                 muc.isActive);
        }

        public static IEnumerable MetricUnitConversions()
        {
            db = new DBContext();
            return db.MetricUnitConversion.Where(t=>t.isActive).ToList();
        }

        public static IEnumerable MetricsUnitsByMectricUnitConversion(int id_metricOrigin)
        {
            db = new DBContext();
            MetricUnit metricOrigin = db.MetricUnit.FirstOrDefault(u => u.id == id_metricOrigin);
            var query = db.MetricUnit.Where(t => t.id_metricType == metricOrigin.id_metricType && t.id != metricOrigin.id &&t.isActive).ToList();
            return query;
        }

        public static IEnumerable MectricUnitByMetricsTypes(int id)
        {
            db = new DBContext();
            MetricType metricType = db.MetricType.FirstOrDefault(t => t.id == id && t.isActive);
            return metricType?.MetricUnit?.ToList() ?? new List<MetricUnit>();
        }

        public static MetricUnitConversion MetricUnitConversionById(int? id_metricOrigin, int? id_metricDestiny, int? id_company)
        {
            db = new DBContext();
            var query = db.MetricUnitConversion.FirstOrDefault(t => t.id_metricOrigin == id_metricOrigin &&
                                                                    t.id_metricDestiny == id_metricDestiny &&
                                                                    t.id_company == id_company);
            return query;
        }

        public static MetricUnitConversion MetricUnitConversionById(int? id)
        {
            db = new DBContext();
            var query = db.MetricUnitConversion.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static MetricUnitConversion GetMetricUnitFactorConversion(int id_metricOrigin, int id_metricDestiny)
        {
            db = new DBContext();

            return db.MetricUnitConversion
                .FirstOrDefault(t => t.id_metricOrigin == id_metricOrigin &&
                    t.id_metricDestiny == id_metricDestiny && t.isActive);
        }

    }
}