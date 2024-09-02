using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{

   
    public class DataProviderIntegrationProcess
    {
        private static DBContext db = null;
        public static IEnumerable States(int id_company)
        {
            db = new DBContext();
            var model = db.IntegrationState.Where(t => (Boolean)t.isActive   ).ToList();

           return model;
        }

        public static IntegrationState StatesById(int id)
        {

            db = new DBContext();
            var model = db.IntegrationState.FirstOrDefault(t =>  t.id == id);
            return model;

        }

        public static IntegrationState StatesByCode(string code, int id_company)
        {
            db = new DBContext();
            // RECODE : [ (Boolean) ]
            var model = db.IntegrationState.FirstOrDefault(t => t.code  == code && t.isActive);
            return model;
        }
    }
}