using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCountry
    {
       private static DBContext db = null;

        public static IEnumerable Contries(int id_company)
        {
            db = new DBContext();
            var model = db.Country.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
            }

            return model;
        }

        public static IEnumerable AllContries(int? id_company)
        {
            db = new DBContext();
            var model = db.Country.Where(p => p.id_company == id_company).ToList();

            return model;
        }
        public static Country CountryById(int? id_country)
        {
            db = new DBContext(); ;
            return db.Country.FirstOrDefault(v => v.id == id_country);
        }

        public static IEnumerable CountryWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.Country.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable CityWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.City.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static IEnumerable StateOfContryWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.StateOfContry.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}