using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderIceBagRange
    {
        private static DBContext db = null;

        public static IEnumerable IceBagRangebyCompany(int? id_company)
        {
            db = new DBContext();
            return db.IceBagRange.Where(t => t.isActive && t.id_company == id_company).ToList();
        }

        public static IceBagRange IceBagRangeById(int id)
        {

            db = new DBContext();
            return db.IceBagRange.FirstOrDefault(i => i.id == id);

        }

        public static IceBagRange IceBagRangeById(int? id)
        {

            db = new DBContext();
            return db.IceBagRange.FirstOrDefault(i => i.id == id);

        }

        public static IEnumerable IceBagRangeByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            return db.IceBagRange.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
        }
    }
}