using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCopackingTariff
    {

        private static DBContext db = null;

        public static IEnumerable CopackingTariffbyCompany(int? id_company)
        {
            db = new DBContext();
            return db.CopackingTariff.Where(t => t.id_company == id_company).OrderBy(t=> t.dateInit ).ThenBy(t => t.dateEnd).ToList();
        }

        public static CopackingTariff CopackingTariffById(int id)
        {

            db = new DBContext();
            return db.CopackingTariff.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable GetPersonCopacking(int? id_company)
        {
            var db = new DBContext();
            var list = db.Person.Where(c => c.isActive && ((bool)c.isCopacking) && c.id_company == id_company).Select(s => new
            {
                id = s.id,
                name = s.fullname_businessName,
            }).ToList();
            return list;
        }

        public static IEnumerable GetInventoryLine(int? id_company)
        {
            var db = new DBContext();
            var list = db.InventoryLine.Where(c => c.isActive  && c.id_company == id_company).Select(s => new
            {
                id = s.id,
                code = s.code,
                name = s.name,
            }).ToList();
            return list;
        }

        public static ItemType ItemType(int? id)
        {
            var db = new DBContext();
            return db.ItemType.FirstOrDefault(i => i.id == id);
        }

        public static InventoryLine InventoryLine(int? id)
        {
            var db = new DBContext();
            return db.InventoryLine.FirstOrDefault(i => i.id == id);
        }

        public static IEnumerable CopackingTariffByCompanyWithCurrentAndProviderForLiquidation(int? id_company, int? id_provider, DateTime? dt)
        {
            db = new DBContext();

            var priceListAux = db.CopackingTariff
                                    .Where(t => t.id_company == id_company && (t.id_provider == id_provider) && t.isActive
                                           && (dt >= t.dateInit && dt <= t.dateEnd)).ToList();

            return priceListAux.Select(s => new {
                s.id,
                name = s.name
                + " [" + s.dateInit.ToString("dd/MM/yyyy") + " - "
                + s.dateEnd.ToString("dd/MM/yyyy") + "]"
            }).OrderBy(t => t.id).ToList();
        }
    }
}