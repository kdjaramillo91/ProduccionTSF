using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTariffHeading
    {

        private static DBContext db = null;

        public static IEnumerable TariffHeadingAll()
        {
            db = new DBContext();
            return db.TariffHeading.Where(r => r.isActive).ToList();
        }
        
        public static TariffHeading TariffHeadingDefault()
        {
            string code_TariffHeading = null;
            db = new DBContext();

            Setting setting = db.Setting.FirstOrDefault(r => r.code == "PARANC");
            if (setting != null)
            {
                code_TariffHeading = setting.value;
            }

            return db.TariffHeading.FirstOrDefault(g => g.isActive && ((code_TariffHeading != null) ? g.code == code_TariffHeading : false));
        }


        public static IEnumerable AllTariffHeading()
        {
            db = new DBContext();
            var model = db.TariffHeading.ToList();

            return model;
        }

        public static TariffHeading TariffHeadingById(int? id_tariffHeading)
        {
            db = new DBContext(); ;
            return db.TariffHeading.FirstOrDefault(v => v.id == id_tariffHeading);
        }

        public static IEnumerable TariffHeadingWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.TariffHeading.Where(g => (g.isActive) || g.id == id_current).ToList();
        }
    }
}