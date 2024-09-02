using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderInvoiceExterior
    {

        private static DBContext db = null;

        public static Invoice InvoiceExteriorById(int? id_invoice)
        {
            db = new DBContext();
            return db.Invoice.FirstOrDefault(t => t.id == id_invoice);
        }
        public static IEnumerable InvoiceExteriorMotive()
        {
            var aSelectListItems = new List<SelectListItem>();
            aSelectListItems.Add(new SelectListItem
            {
                Text = "Reasignación de Proforma",
                Value = "1"
            });

            return aSelectListItems;

        }
    }
}