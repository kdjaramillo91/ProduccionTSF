using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPaymentMethod
    {
        private static DBContext db = null;
        public static IEnumerable PaymentsMethods(int id_company)
        {
            db = new DBContext();
            var model = db.PaymentMethod.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable AllPaymentMethods()
        {
            db = new DBContext();
            var model = db.PaymentMethod.ToList();

            return model;
        }
        public static IEnumerable EPPaymentMethods()
        {
            db = new DBContext();
            var model = db.PaymentMethod.Where(w => w.codeProgram == "EP").ToList();

            return model;
        }

        public static IEnumerable AllPaymentMethodsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.PaymentMethod.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static PaymentMethod PaymentMethodById(int? id_paymentMethod)
        {
            db = new DBContext(); ;
            return db.PaymentMethod.FirstOrDefault(v => v.id == id_paymentMethod);
        }

        public static IEnumerable EPPaymentMethodWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.PaymentMethod.Where(g => (g.isActive) || g.id == id_current && g.codeProgram== "EP").ToList();
        }
        public static IEnumerable PaymentMethodWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.PaymentMethod.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable FXPaymentMethods()
        {
            db = new DBContext();
            var model = db.PaymentMethod.Where(w => w.codeProgram == "FX" && w.isActive).ToList();

            return model;
        }
    }
}