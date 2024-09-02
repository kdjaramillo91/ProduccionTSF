using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderRate
    {
        private static DBContext db = null;

        public static Rate Rate(int? id)
        {
            db = new DBContext();
            return db.Rate.FirstOrDefault(r => r.id == id && r.isActive);
        }

        public static IEnumerable Rates()
        {
            db = new DBContext();
            return db.Rate.Where(t=>t.isActive).ToList();
        }

        public static IEnumerable RatesByTaxType(int? id_taxType)
        {
            db = new DBContext();
            return db.Rate.Where(r => r.id_taxType == id_taxType && r.isActive).ToList();
        }

        public static Rate RateById(int? id)
        {
            db = new DBContext();
            var query = db.Rate.FirstOrDefault(t => t.id == id);
            return query;
        }

        public static IEnumerable RatesByCompany(int? id_company)
        {
            db = new DBContext();
            var model = db.Rate.Where(t => t.isActive && t.id_company == id_company).ToList();

            //if (id_company != 0)
            //{
            //    model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            //}

            return model;
        }

        public static IEnumerable RatesByCompanyTaxTypeAndCurrent(int? id_company, int? id_taxType, int? id_current)
        {
            db = new DBContext();
            var model = db.Rate.Where(t => (t.isActive && t.id_company == id_company && t.id_taxType == id_taxType) ||
                                                 t.id == id_current).ToList();

            
            return model;
        }
    }
}