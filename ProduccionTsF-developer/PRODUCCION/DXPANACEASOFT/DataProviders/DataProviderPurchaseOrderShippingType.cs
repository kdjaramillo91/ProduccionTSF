using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPurchaseOrderShippingType
    {
        private static DBContext db = null;
        public static IEnumerable PurchaseOrderShippingTypeByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            var model = db.PurchaseOrderShippingType.Where(p => (p.id_company == id_company && p.isActive) || p.id == id_current).ToList();

            return model;
        }
        public static IEnumerable PurchaseOrderShippingType(int id_company)
        {
            db = new DBContext();

            var model = db.PurchaseOrderShippingType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static PurchaseOrderShippingType PurchaseOrderShippingTypeById(int? id)
        {
            db = new DBContext();
            return db.PurchaseOrderShippingType.FirstOrDefault(t => t.id == id);
        }

        public static IEnumerable PurchaseOrderShippingTypeisVehicle(int id_company)
        {
            db = new DBContext();

            var model = db.PurchaseOrderShippingType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company  && p.isVehicleType).ToList();
            }

            return model;
        }


        public static IEnumerable PurchaseOrderShippingTypeisTerrestriel(int id_company)
        {
            db = new DBContext();

            var model = db.PurchaseOrderShippingType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isVehicleType &&  p.isTerrestriel ).ToList();
                
            }

            return model;
        }

    }
}