using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public static class DataProviderCity
    {
        private static DBContext db = null;

   

        public static City CityById(int? id_City)
        {
            db = new DBContext();



            var City = db.City.Where(t => t.id == id_City ).FirstOrDefault();




            return City;
        }

		//public static IEnumerable CityAll(int? id_current)
		//{
		//    db = new DBContext();
		//    var Cityaux = db.City.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

		//    if (id_current != null && id_current > 0)
		//    {
		//        var cant = (from de in Cityaux
		//                    where de.id == id_current
		//                    select de).ToList().Count;
		//        if (cant == 0)
		//        {
		//            var Citycuuretaux = db.City.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

		//            Cityaux.AddRange(Citycuuretaux);
		//        }
		//    }

		//    return Cityaux.ToList();

		//}
		public static IEnumerable CityCountry(int? id_current)
		{
			db = new DBContext();
			var model = db.City.Where(g => g.isActive && id_current == g.id_country).Select(p => new { p.id, code = p.code, name = p.name });



			return model.ToList();
		}

		public static IEnumerable CityAll(int? id_current)
        {
            db = new DBContext();
            var model = db.City.Where(g => g.isActive || id_current == g.id).Select(p => new { p.id, name = p.name });



            return model.ToList();
        }

    }
}