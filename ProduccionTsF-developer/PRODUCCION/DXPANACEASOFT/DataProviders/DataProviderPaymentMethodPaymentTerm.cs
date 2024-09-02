using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderPaymentMethodPaymentTerm
    {
        private static DBContext db = null;

   

        public static PaymentMethodPaymentTerm PaymentMethodPaymentTermById(int? id_PaymentMethodPaymentTerm)
        {
            db = new DBContext();



            var PaymentMethodPaymentTerm = db.PaymentMethodPaymentTerm.Where(t => t.id == id_PaymentMethodPaymentTerm ).FirstOrDefault();




            return PaymentMethodPaymentTerm;
        }

      


    }
}