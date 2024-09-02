using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderPort
	{
		private static DBContext db = null;

		public static IEnumerable PortAll()
		{

			db = new DBContext();
			return db.Port.Where(r => r.isActive).ToList();
		}

		public static IEnumerable PortOnlyCityCountry(int id_Company, Boolean? isCountrySource = false)
		{

			db = new DBContext();

			// TODO:: ENUM CODIGOS
			Setting setting = null;
			/* if ((Boolean)isCountrySource)
			 {*/
			setting = db.Setting.FirstOrDefault(r => r.id_company == id_Company && r.isActive && r.code == "PORI");
			//}
			// && r.City.Country.code == (((Boolean)isCountrySource && setting != null) ? setting.value : r.City.Country.code)
			var model = db.Port.Where(r => r.isActive);
			if (setting != null)
			{
				if ((Boolean)isCountrySource)
				{
					model = model.Where(t => t.City.Country.code == setting.value);
				}
				else
				{
					model = model.Where(t => t.City.Country.code != setting.value);
				}


			}



			List<Port> _puertosCiudad = new List<Port>();


			int idCityPass = 0;


			model.Select(r => new
			{
				id = r.id,
				code = r.code,
				name = r.nombre,
				codeCity = r.City.code,
				cityName = r.City.name,
				idCity = r.City.id,
				countryName = r.City.Country.name

			}).ToList().ForEach(f =>
			{

				if (idCityPass == 0 || idCityPass != f.idCity)
				{
					_puertosCiudad.Add(new Port
					{
						id = f.id,
						code = f.codeCity,
						nombre = f.cityName,

					});
				}
				idCityPass = f.idCity;
			});

			return _puertosCiudad.Select(r => new
			{
				id = r.id,
				code = r.code,
				nombre = r.nombre.ToUpper(),
				name = r.nombre.ToUpper()
			}).ToList();
		}


		public static IEnumerable PortAllCityCountry(int id_Company, Boolean? isCountrySource = false)
		{

			db = new DBContext();

			// TODO:: ENUM CODIGOS
			Setting setting = null;
			setting = db.Setting.FirstOrDefault(r => r.id_company == id_Company && r.isActive && r.code == "PORI");

			var model = db.Port.Where(r => r.isActive);
			if (setting != null)
			{
				var idCountry = db.Country.FirstOrDefault(e => e.code == setting.value)?.id ?? 0;
				model = isCountrySource ?? false
					? model.Where(t => t.City.id_country == idCountry)
					: model = model.Where(t => t.City.id_country != idCountry);
			}

			return model.Select(r => new
			{
				id = r.id,
				code = r.code,
				name = r.nombre,
				idCity = r.City.id,
				cityName = r.City.name,
				countryName = r.City.Country.name

			}).OrderBy(o => o.name).ToList();
		}

		public static IEnumerable PortTerminalAllCityCountry()
		{
			db = new DBContext();

			var idPortType = db.portType.FirstOrDefault(e => e.code == "TMI")?.id ?? 0;

			var model = db.Port.Where(r => r.isActive && r.id_portType == idPortType);
			return model.Select(r => new
			{
				id = r.id,
				code = r.code,
				name = r.nombre,
				idCity = r.City.id,
				cityName = r.City.name,
				countryName = r.City.Country.name

			}).OrderBy(o => o.name).ToList();
		}

		public static IEnumerable AllPorts()
		{
			db = new DBContext();
			var model = db.Port.ToList();

			return model;
		}

		public static Port PortById(int? id_port)
		{
			db = new DBContext();
			return db.Port.FirstOrDefault(v => v.id == id_port);
		}

		public static IEnumerable PortWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.Port.Where(g => (g.isActive) || g.id == id_current).ToList();
		}

		public static IEnumerable PortAll(int? id_current)
		{
			db = new DBContext();
			var Portaux = db.Port.Where(g => (g.isActive)).Select(p => new { p.id, name = p.nombre }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in Portaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var Portcuuretaux = db.Port.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.nombre });

					Portaux.AddRange(Portcuuretaux);
				}
			}

			return Portaux;

		}
		public static IEnumerable AllTypePorts()
		{
			db = new DBContext();
			var list = db.portType.Where(c => c.isActive).ToList();

			return list;
		}
	}
}