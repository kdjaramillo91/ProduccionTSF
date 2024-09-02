using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCustomerState
    {
        private static DBContext db = null;

        public static IEnumerable GetAllActive()
        {
            db = new DBContext();
            var list = db.CustomerState.Where(c => c.isActive).ToList();
            return list;
        }
    }
}