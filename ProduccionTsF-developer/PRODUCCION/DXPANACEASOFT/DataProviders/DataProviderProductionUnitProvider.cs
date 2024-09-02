using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;
//using System.Web.Mvc;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderProductionUnitProvider
    {
        private static DBContext db = null;

        public static IEnumerable ProductionUnitProviders()
        {
            db = new DBContext();
            return db.ProductionUnitProvider.ToList();
        }

        public static ProductionUnitProvider ProductionUnitProviderById(int? id_productionUnitProvider)
        {
            db = new DBContext();
            ProductionUnitProvider _productionUnitProvider = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionUnitProvider);
            FishingZone _fishingZone  =DataProviderProductionUnitProvider.GetFishingZone(_productionUnitProvider);

            return _productionUnitProvider;
        }


        public static IEnumerable ProductionUnitProviderByProvider(int? id_productionUnitProviderCurrent, int? id_provider)
        {
            db = new DBContext();

            var productionUnitProviderAux = db.ProductionUnitProvider.Where(t => t.isActive && t.id_provider == id_provider).ToList();

            var productionUnitProviderCurrentAux = db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == id_productionUnitProviderCurrent);
            if (productionUnitProviderCurrentAux != null && !productionUnitProviderAux.Contains(productionUnitProviderCurrentAux)) productionUnitProviderAux.Add(productionUnitProviderCurrentAux);

            return productionUnitProviderAux.Select(s => new {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable AllProductionUnitProviderPoolsByCompany(int? id_company)
        {
            db = new DBContext();

            return db.ProductionUnitProviderPool.Where(s => s.ProductionUnitProvider.Provider.Person.id_company == id_company).Select(s => new { id = s.id, name = (s.name + "(" + s.ProductionUnitProvider.Provider.Person.fullname_businessName + ")") }).ToList();

        }

        private static FishingZone GetFishingZone( ProductionUnitProvider _productionUnitProvider)
        {

            FishingZone _fishingZone = DataProviderFishingSite.FishingSiteById(_productionUnitProvider?.id_FishingSite)?.FishingZone ?? new FishingZone();
            return _fishingZone;
        }

    }
    }
