using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderSalesQuotation
    {
        private static DBContext db = null;

        public static IEnumerable SalesQuotations(int id_emissionPoint)
        {
            db = new DBContext();

            var model = db.SalesQuotation.ToList();

            if (id_emissionPoint != 0)
            {
                model = model.Where(q => q.Document.id_emissionPoint == id_emissionPoint).ToList();
            }

            return model;

        }

        public static SalesQuotation SalesQuotation(int? id)
        {
            db = new DBContext();
            return db.SalesQuotation.FirstOrDefault(t => t.id == id );
            
        }
          
    }
}