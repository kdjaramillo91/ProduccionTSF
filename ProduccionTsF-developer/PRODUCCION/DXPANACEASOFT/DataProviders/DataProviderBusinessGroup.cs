using System.Collections;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessGroup
    {
       private static DBContext db = null;

        public static IEnumerable BusinessGroups()
        {
            db = new DBContext();
            return db.BusinessGroup.ToList();
        }

        public static BusinessGroup BusinessGroupById(int? id)
        {
            db = new DBContext();
            var query = db.BusinessGroup.FirstOrDefault(t => t.id == id);
            return query;
        }
    }
}