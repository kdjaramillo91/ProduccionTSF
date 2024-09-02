using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;


namespace DXPANACEASOFT.DataProviders
{

    public  class DataProviderPurchaseFilter
    {
        private static DBContext db = null;

        public static IEnumerable PurchaseFilter()
        {
            db = new DBContext();
            var model = new List<PurchaseFilter>();

            model.Add(new PurchaseFilter
            {
                id = 1,
                code = "01",
                name = "TOTALMENTE ATENDIDOS"
            });
            model.Add(new PurchaseFilter
            {
                id = 2,
                code = "02",
                name = "PARCIALMENTE ATENDIDOS"
            });
            model.Add(new PurchaseFilter
            {
                id = 3,
                code = "03",
                name = "REQUERIMIENTOS SIN ATENDER"
            });
           
            return model;
        }

        public static PurchaseFilter LogicalOperatorById(int? id_PurchaseFilte)
        {
            List<PurchaseFilter> model = (List<PurchaseFilter>)PurchaseFilter();
            foreach (var m in model)
            {
                if (m.id == id_PurchaseFilte)
                    return m;
            }
            return null;
        }

    }
}