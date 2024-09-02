using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTransportTariff
    {

        private static DBContext db = null;

        public static IEnumerable TransportTariffbyCompany(int? id_company)
        {
            db = new DBContext();
            return db.TransportTariff.Where(t => t.isActive && t.id_company == id_company).OrderBy(t=> t.dateInit ).ThenBy(t => t.dateEnd).ToList();
        }

        public static TransportTariff TransportTariffById(int id)
        {

            db = new DBContext();
            return db.TransportTariff.FirstOrDefault(i => i.id == id);
        }






    }
}