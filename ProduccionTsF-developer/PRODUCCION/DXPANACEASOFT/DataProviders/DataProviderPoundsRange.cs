using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPoundsRange
    {

        private static DBContext db = null;

        

        public static IEnumerable PoundsRangebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.PoundsRange.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static PoundsRange PoundsRangeById(int id)
        {
            db = new DBContext();
            return db.PoundsRange.FirstOrDefault(i => i.id == id);
        }

        public static PoundsRange PoundsRangeById(int? id)
        {
            db = new DBContext();
            return db.PoundsRange.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable PoundsRangeByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var toRerturn = db.PoundsRange.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
            return toRerturn;
        }

    }
}