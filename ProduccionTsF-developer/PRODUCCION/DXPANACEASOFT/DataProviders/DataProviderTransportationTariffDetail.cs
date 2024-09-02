using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTransportationTariffDetail
    {


        private static DBContext db = null;

        public static IEnumerable TransportationTariffDetailbyCompany(int? id_company)
        {
            db = new DBContext();
            return db.TransportTariffDetail.Where(t => t.isActive && t.id_company == id_company).OrderBy(t => t.orderTariff).ToList();
        }

        public static TransportTariff TransportTariffTypeById(int id)
        {

            db = new DBContext();
            return db.TransportTariff.FirstOrDefault(i => i.id == id);
        }
    }
}