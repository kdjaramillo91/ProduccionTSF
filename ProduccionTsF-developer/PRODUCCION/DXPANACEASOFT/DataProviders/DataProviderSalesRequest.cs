using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderSalesRequest
    {
        private static DBContext db = null;
        public static SalesRequest SalesRequest(int? id)
        {
            db = new DBContext();
            return db.SalesRequest.FirstOrDefault(t => t.id == id);
        }
    }

   
}