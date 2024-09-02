using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;



namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPurchaseReason
    {
        private static DBContext db = null;

        public static IEnumerable PurchaseReason(int id_company)
        {
            db = new DBContext();
            var model = db.PurchaseReason.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;

        }

        public static PurchaseReason PurchaseReasonById(int? id_purchaseReason)
        {
            using (DBContext db = new DBContext())
            {
                return db.PurchaseReason.FirstOrDefault(i => i.id == id_purchaseReason);
            }
           
        }
    }
}