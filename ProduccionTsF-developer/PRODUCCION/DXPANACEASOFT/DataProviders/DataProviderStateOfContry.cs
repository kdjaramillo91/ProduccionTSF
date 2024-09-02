using DXPANACEASOFT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderStateOfContry
	{
		private static DBContext db = null;

		public static IEnumerable StateOfContryByCountryAndCurrent(int? id_Country, int? id_current)
		{
			db = new DBContext();

			var StateOfContryAux = db.StateOfContry.Where(t => t.id == id_Country && t.isActive).ToList() ?? new List<StateOfContry>();

			var currentAux = db.StateOfContry.FirstOrDefault(fod => fod.id == id_current);
			if (currentAux != null && !StateOfContryAux.Contains(currentAux)) StateOfContryAux.Add(currentAux);

			return StateOfContryAux;
		}
	}
}