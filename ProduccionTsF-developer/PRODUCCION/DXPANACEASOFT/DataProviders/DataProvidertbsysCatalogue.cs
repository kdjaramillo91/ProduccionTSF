using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProvidertbsysCatalogue
    {
       private static DBContext db = null;

        public static IEnumerable TbsysCatalogueDetails(int? id_tbsysCatalogue)
        {
            db = new DBContext();
            var model = db.tbsysCatalogueDetail.Where(t => t.isActive).ToList();

            if (id_tbsysCatalogue != 0)
            {
                model = model.Where(p => p.id_Catalogue == id_tbsysCatalogue).ToList();
            }

            return model;
        }

        public static IEnumerable AlltbsysCatalogueDetailByCode(string code_tbsysCatalogue)
        {
            db = new DBContext();
            var id_tbsysCatalogueAux = db.tbsysCatalogue.FirstOrDefault(fod=> fod.code == code_tbsysCatalogue)?.id;
            var model = db.tbsysCatalogueDetail.Where(p => p.id_Catalogue == id_tbsysCatalogueAux).ToList();

            return model;
        }
        
        public static IEnumerable CountryWithCurrent(int? id_current)
        {
            db = new DBContext();
            return db.tbsysCatalogueDetail.Where(g => (g.isActive) || g.id == id_current).ToList();
        }

        public static tbsysCatalogueDetail TbsysCatalogueDetailById(int? id_current)
        {
            db = new DBContext();
            var model = db.tbsysCatalogueDetail.FirstOrDefault(g => (g.isActive) && g.id == id_current);

            return model;
        }

    }
}