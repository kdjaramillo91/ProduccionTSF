using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderVehicleType
    {
        private static DBContext db = null;

        public static IEnumerable VehiculeTypes()
        {
            db = new DBContext(); ;
            return db.VehicleType.Where(v => v.isActive).Select(s => new { id = s.id, name = s.description, shippingType = s.PurchaseOrderShippingType.name }).ToList();
        }

        public static VehicleType VehicleType(int? id_vehicleType)
        {
            db = new DBContext(); ;
            return db.VehicleType.FirstOrDefault(v => v.id == id_vehicleType);
        }

        public static VehicleType VehicleTypeById(int? id)
        {

            db = new DBContext();
            return db.VehicleType.FirstOrDefault(i => i.id == id);

        }

        public static IEnumerable VehicleTypeMetricUnitByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.MetricUnit.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable VehicleTransportTariffTypesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.PurchaseOrderShippingType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}