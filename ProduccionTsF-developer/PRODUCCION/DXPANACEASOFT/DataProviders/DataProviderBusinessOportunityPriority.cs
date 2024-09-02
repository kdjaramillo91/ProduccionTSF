using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessOportunityPriority
    {
        private static DBContext db = null;

        public static IEnumerable Priorities(int id_company)
        {
            db = new DBContext();
            var model = db.BusinessOportunityPriority.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable BusinessOportunityPrioritiesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            return db.BusinessOportunityPriority.Where(s => (s.id_company == id_company && s.isActive) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.ToList();

        }

        public static BusinessOportunityPriority PriorityById(int? id_priority)
        {
            db = new DBContext(); ;
            return db.BusinessOportunityPriority.FirstOrDefault(v => v.id == id_priority);
        }
    }
}