using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderSecuritySealState
    {
        private static DBContext db = null;

        public static IEnumerable SecuritySealStates(int id_company)
        {
            db = new DBContext();
            var model = db.SecuritySealState.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable SecuritySealStatesFilter(int id_company)
        {
            db = new DBContext();
            var model = db.SecuritySealState.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).ToList();
            }

            return model;
        }

        public static SecuritySealState SecuritySealStateById(int? id_securitySealState)
        {
            db = new DBContext();
            return db.SecuritySealState.FirstOrDefault(t => t.id == id_securitySealState);
        }
    }
}