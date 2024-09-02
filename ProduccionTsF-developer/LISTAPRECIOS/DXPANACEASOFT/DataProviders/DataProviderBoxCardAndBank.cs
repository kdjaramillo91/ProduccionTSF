using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBoxCardAndBank
    {
        private static DBContext db = null;
        //public static IEnumerable PaymentsMethods(int id_company)
        //{
        //    db = new DBContext();
        //    var model = db.PaymentMethod.Where(t => t.isActive).ToList();

        //    if (id_company != 0)
        //    {
        //        model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
        //    }

        //    return model;
        //}


        public static BoxCardAndBank BoxCardAndBankById(int? id_boxCardAndBank)
        {
            db = new DBContext(); ;
            return db.BoxCardAndBank.FirstOrDefault(v => v.id == id_boxCardAndBank);
        }

        public static IEnumerable BankWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.BoxCardAndBank.Where(g => (g.isActive && (g.TypeBoxCardAndBank.code.Equals("BAN"))) || g.id == id_current).ToList();//BAN Codigo de Banco en TypeBoxCardAndBank
        }

        public static IEnumerable AllBanks()
        {
            db = new DBContext();
            return db.BoxCardAndBank.Where(g => (g.TypeBoxCardAndBank.code.Equals("BAN"))).ToList();//BAN Codigo de Banco en TypeBoxCardAndBank
        }

        public static IEnumerable BoxCardWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.BoxCardAndBank.Where(g => (g.isActive && (g.TypeBoxCardAndBank.code.Equals("CAJA") || g.TypeBoxCardAndBank.code.Equals("TAR"))) || g.id == id_current).ToList();//CAJA, TAR son Codigos de Caja y Tarjeta respectivamente en TypeBoxCardAndBank
        }

        public static IEnumerable TypeBoxCardWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.TypeBoxCardAndBank.Where(g => (g.isActive && (g.code.Equals("CAJA") || g.code.Equals("TAR"))) || g.id == id_current).ToList();//CAJA, TAR son Codigos de Caja y Tarjeta respectivamente en TypeBoxCardAndBank
        }
    }
}