using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderRemissionGuideAssignedStaffRol
    {
        private static DBContext db = null;
        public static IEnumerable RemissionGuideAssignedStaffRol(int id_company)
        {
            db = new DBContext();

            var model = db.PurchaseOrderShippingType.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static RemissionGuideAssignedStaffRol RemissionGuideAssignedStaffRolById(int? id)
        {
            db = new DBContext();
            return db.RemissionGuideAssignedStaffRol.FirstOrDefault(t => t.id == id);
        }
    }
}