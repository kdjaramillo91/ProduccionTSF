using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTransportTariffType
    {

        private static DBContext db = null;

        public static IEnumerable TransportTariffTypebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.TransportTariffType.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static TransportTariffType TransportTariffTypeById(int? id)
        {

            db = new DBContext();
            return db.TransportTariffType.FirstOrDefault(i => i.id == id);

        }

        public static IEnumerable GetTransportTariffTypeShippingType()
        {
            var db = new DBContext();
            var list = db.PurchaseOrderShippingType.Where(c => c.isActive).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name
            }).ToList();
            return list;
        }

        public static PurchaseOrderShippingType PurchaseOrderShippingTypeById(int? id)
        {
            db = new DBContext();
            var query = db.PurchaseOrderShippingType.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable PurchaseOrderShippingFilter(int id_company)
        {
            db = new DBContext();
            var model = db.PurchaseOrderShippingType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;

        }

        public static IEnumerable TransportTariffTypeTypesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.PurchaseOrderShippingType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }

        public static IEnumerable TransportTariffTypeByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var transportTariffTypeList = db.TransportTariffType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
            return transportTariffTypeList;
        }

    }
}