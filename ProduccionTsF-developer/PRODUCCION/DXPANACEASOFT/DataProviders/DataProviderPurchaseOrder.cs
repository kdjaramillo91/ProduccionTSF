using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPurchaseOrder
    {
        private static DBContext db = null;

        public static PurchaseOrder PurchaseOrder(int? id)
        {
            db = new DBContext();
            return db.PurchaseOrder.FirstOrDefault(t => t.id == id);
        }

        public static IEnumerable PurchaseOrderShippingType()
        {
            db = new DBContext();
            return db.PurchaseOrderShippingType.Where(p => p.name != null && p.isActive).ToList();
        }

        public static IEnumerable PurchaseReason()
        {
            db = new DBContext();
            return db.PurchaseReason.Where(p => p.isActive).ToList();
        }

        public static PurchaseOrderDetail PurchaseOrderDetail(int? id_purchaseDetail)
        {
            db = new DBContext();
            return db.PurchaseOrderDetail.FirstOrDefault(p => p.id == id_purchaseDetail);
        }
    }
}