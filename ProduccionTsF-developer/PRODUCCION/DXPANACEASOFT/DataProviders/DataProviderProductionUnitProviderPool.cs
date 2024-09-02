using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderProductionUnitProviderPool
	{
		private static DBContext db = null;
		public static IEnumerable ProductionUnitProviderPool()
		{
			db = new DBContext();
			return db.ProductionUnitProvider.ToList();
		}
		public static IEnumerable ProductionUnitProviderPools()
		{
			db = new DBContext();
			return db.ProductionUnitProviderPool.ToList();
		}
		public static ProductionUnitProviderPool ProductionUnitProviderPoolById(int? id_productionUnitProviderPool)
		{
			db = new DBContext();

			var model = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPool);

			if(model != null)
			{
                model.certification_name = db.Certification.FirstOrDefault(c => c.id == model.id_certification)?.name;
            }

            return model;

		}
		public static IEnumerable ProductionUnitProviderPoolByUnitProvider(int? id_productionUnitProviderPoolCurrent, int? id_productionUnitProvider)
		{
			db = new DBContext();

			var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();

			var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
			if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);

			return productionUnitProviderPoolAux.Select(s => new {
				s.id,
				s.name
			}).OrderBy(t => t.id).ToList();
		}

		public static IEnumerable ProductionUnitProviderPoolByProductionUnitProvider(int? id_productionUnitProviderPoolCurrent, int? id_productionUnitProvider)
		{
			db = new DBContext();

			var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();

			var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
			if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);

			return productionUnitProviderPoolAux.Select(s => new {
				s.id,
				name = s.code + "-" + s.name
			}).OrderBy(t => t.id).ToList();
		}
		public static List<ProductionUnitProviderPool> ProductionUnitProviderPoolBachList(int id_productionUnitProvider, int id_user, string poolprefix, string poolsuffix, int cantcreate)
		{
			db = new DBContext();

			List<ProductionUnitProviderPool> listproductionUnitproviderpool = new List<ProductionUnitProviderPool>();
			int secuencia = 0;
			var productionUnitproviderpool = db.ProductionUnitProviderPool.Where(g => g.id_productionUnitProvider == id_productionUnitProvider);

			if (productionUnitproviderpool != null)
			{
				listproductionUnitproviderpool.AddRange(productionUnitproviderpool);
				secuencia = listproductionUnitproviderpool.Count;
			}

			if (cantcreate > secuencia)
			{
				int create = cantcreate - secuencia;
				secuencia = secuencia + 1;

				if (poolprefix == null) poolprefix = "";
				if (poolsuffix == null) poolsuffix = "";

				for (int i = 1; i <= create; i++)
				{
					ProductionUnitProviderPool ProductionUnitProviderPooltemp = new ProductionUnitProviderPool()
					{
						id_productionUnitProvider = id_productionUnitProvider,
						code = poolprefix + secuencia.ToString() + poolsuffix,
						dateCreate = DateTime.Now,
						dateUpdate = DateTime.Now,
						id_userCreate = id_user,
						id_userUpdate = id_user,
						isActive = true,
						name = poolprefix + secuencia.ToString() + poolsuffix,

					};
					listproductionUnitproviderpool.Add(ProductionUnitProviderPooltemp);
					secuencia = secuencia + 1;
				}

			}

			return listproductionUnitproviderpool;
		}

		public static IEnumerable ProductionUnitProviderPoolByProvider(int? id_productionUnitProviderPoolCurrent, int? id_provider)
		{
			db = new DBContext();

			var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.ProductionUnitProvider.id_provider == id_provider).ToList();

			var productionUnitProviderPoolCurrentAux = db.ProductionUnitProviderPool.FirstOrDefault(fod => fod.id == id_productionUnitProviderPoolCurrent);
			if (productionUnitProviderPoolCurrentAux != null && !productionUnitProviderPoolAux.Contains(productionUnitProviderPoolCurrentAux)) productionUnitProviderPoolAux.Add(productionUnitProviderPoolCurrentAux);

			return productionUnitProviderPoolAux.Select(s => new {
				s.id,
				name = s.code + "-" + s.name
			}).OrderBy(t => t.id).ToList();
		}

		public static IEnumerable AllProductionUnitProviderPoolsByCompany(int? id_company)
		{
			db = new DBContext();

			//return db.ProductionUnitProviderPool.Where(s => s.ProductionUnitProvider.Provider.Person.id_company == id_company).Select(s => new { id = s.id, name = (s.name + "(" + s.ProductionUnitProvider.Provider.Person.fullname_businessName + ")") }).ToList();
			return db.ProductionUnitProviderPool.Where(s => s.ProductionUnitProvider.Provider.Person.id_company == id_company).Select(s => new { id = s.id, name = (s.code + "-" + s.name) }).ToList();

		}

        public static IEnumerable ComboProductionUnitProviderPoolByUnitProvider( int? id_productionUnitProvider)
        {
            db = new DBContext();

            var productionUnitProviderPoolAux = db.ProductionUnitProviderPool.Where(t => t.isActive && t.id_productionUnitProvider == id_productionUnitProvider).ToList();

            return productionUnitProviderPoolAux.Select(s => new {
				s.id,
                s.name
            }).OrderBy(t => t.name).ToList();
        }
    }
}