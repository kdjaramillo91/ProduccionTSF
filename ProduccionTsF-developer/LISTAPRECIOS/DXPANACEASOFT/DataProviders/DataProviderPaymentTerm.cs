using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPaymentTerm
    {
        private static DBContext db = null;

        public static IEnumerable AllPaymentTerms()
        {
            db = new DBContext();
            var model = db.PaymentTerm.ToList();

            return model;
        }
        public static IEnumerable AllPaymentTermsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.PaymentTerm.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static IEnumerable PaymentsTerms(int id_company)
        {
            db = new DBContext();
            var model = db.PaymentTerm.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }
            return model;
        }

        public static PaymentTerm PaymentTermById(int? id_paymentTerm)
        {
            db = new DBContext(); ;
            return db.PaymentTerm.FirstOrDefault(v => v.id == id_paymentTerm);
        }

        public static IEnumerable InvoiceExteriorPaymentsTermsByPaymentsMethodandCurrent(int? id_PaymentsMethod, int? id_PaymentTerm)
        {
            db = new DBContext();
            var model = db.PaymentMethodPaymentTerm.Where(r => ( r.id_paymentMethod == id_PaymentsMethod && r.isActive) || ((r.id_paymentMethod == id_PaymentsMethod && r.isActive) &&  r.id_paymentTerm  == (id_PaymentTerm == null ? 0 : id_PaymentTerm)))
                .Select(r => new { id = r.PaymentTerm.id, code = r.PaymentTerm.code, description = r.PaymentTerm.description })
                .ToList();

            return model;
        }


    }
}