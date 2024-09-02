using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCatalogue
    {
        private static DBContext db = null;

        public static tbsysCatalogue GetCatalogueById(int? id_catalogue)
        {
            db = new DBContext();
            return db.tbsysCatalogue.FirstOrDefault(i => i.id == id_catalogue);
        }

        public static IEnumerable CatalogueFilter()
        {
            db = new DBContext();
            var model = db.tbsysCatalogue.ToList();

            if(model != null)
            {
                model = model.ToList();
            }

            return model;
        }

        public static tbsysCatalogueDetail GetCatalogueDetailById(int id_Setting)
        {
            db = new DBContext();
            return db.tbsysCatalogueDetail.FirstOrDefault(i => i.id == id_Setting);
        }


        public static List<tbsysCatalogueDetail> GetTbsysCatalogueDetailBySetting(string code)
        {
            db = new DBContext();
            var idCatalogue = db.tbsysCatalogue.FirstOrDefault(e => e.code == code)?.id;
            if (idCatalogue.HasValue)
            {
                return db.tbsysCatalogueDetail.Where(r => r.id_Catalogue == idCatalogue.Value).ToList();
            }
            else
            {
                return null;
            }            
        }

        public static IEnumerable GetAllCatalogues()
        {
            db = new DBContext();
            
            return db.tbsysCatalogue.ToList();
        }
    }
}