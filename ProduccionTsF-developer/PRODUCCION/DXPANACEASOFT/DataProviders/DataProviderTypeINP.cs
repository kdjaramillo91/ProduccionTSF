using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTypeINP
    {
        private static DBContext db = null;

        public static IEnumerable TypeINP(int? id_company)
        {
            db = new DBContext();
            var model = db.TypeINP.Where(t => (bool)t.isActive).ToList();
            
            return model;
        }
        public static IEnumerable AllTypeINP(int? id_company)
        {
            db = new DBContext();
            var model = db.TypeINP.ToList();

            return model;
        }
        public static TypeINP TypeINPById(int? id_TypeINP)
        {
            db = new DBContext(); ;
            return db.TypeINP.FirstOrDefault(v => v.id == id_TypeINP);
        }
        public static IEnumerable TypeINPWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.TypeINP.Where(g => ((bool)g.isActive) || g.id == id_current).ToList();
        }

    }
}