using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderDocumentState
    {
        private static DBContext db = null;

        public static IEnumerable DocumentStates()
        {
            db = new DBContext();
            return db.DocumentState.Where(d=>d.isActive).OrderBy(t => t.id).ToList();
        }
    }
}