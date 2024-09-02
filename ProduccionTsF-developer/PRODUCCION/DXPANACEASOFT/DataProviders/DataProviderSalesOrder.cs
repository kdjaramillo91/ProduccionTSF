using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderSalesOrder
    {
        private static DBContext db = null;

        public static SalesOrder SalesOrder(int? id)
        {
            db = new DBContext();
            return db.SalesOrder.FirstOrDefault(t => t.id == id);
        }

        //public static IEnumerable SalesOrderShippingType()
        //{
        //    db = new DBContext();
        //    return db.SalesOrderShippingType.Where(p => p.name != null && p.isActive).ToList();
        //}

        //public static IEnumerable SalesReason()
        //{
        //    db = new DBContext();
        //    return db.SalesReason.Where(p => p.isActive).ToList();
        //}

        public static SalesOrderDetail SalesOrderDetail(int? id_salesDetail)
        {
            db = new DBContext();
            return db.SalesOrderDetail.FirstOrDefault(p => p.id == id_salesDetail);
        }

        public static IEnumerable SalesOrdersByCompanyForProduction(int? id_company)
        {
            db = new DBContext();
            var items = db.SalesOrder.Where(i => i.Document.EmissionPoint.id_company == id_company && i.Document.DocumentState.code.Equals("06") &&//AUTORIZADA
                                                  i.SalesOrderDetail.FirstOrDefault(fod=> fod.quantityDelivered < fod.quantityApproved && fod.Item.isActive) != null);

            return items.Select(s=> new { s.id, s.Document.number}).ToList();
        }

        public static IEnumerable AllSalesOrderByCompany(int? id_company)
        {
            db = new DBContext();

            return db.SalesOrder.Where(s => s.Document.EmissionPoint.id_company == id_company).Select(s => new { id = s.id, name = s.Document.number }).ToList();

        }

    }
}