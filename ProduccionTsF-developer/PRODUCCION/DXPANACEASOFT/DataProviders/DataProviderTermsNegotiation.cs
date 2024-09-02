using DXPANACEASOFT.Models;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderTermsNegotiation
	{
		private static DBContext db = null;

		public static IEnumerable TermsNegotiation()
		{
			db = new DBContext();
			return db.TermsNegotiation
				.Where(g => g.isActive)
				.ToList();
		}

		public static TermsNegotiation TermsNegotiationDefault()
		{
			string code_TermsNegotiation = null;
			db = new DBContext();

			Setting setting = db.Setting.FirstOrDefault(r => r.code == "TERMNEG");
			if (setting != null)
			{
				code_TermsNegotiation = setting.value;
			}

            if (!string.IsNullOrEmpty(code_TermsNegotiation))
            {
				return db.TermsNegotiation.FirstOrDefault(e => e.code == code_TermsNegotiation && e.isActive);

			}
            else
            {
				return null;
            }
		}

		public static TermsNegotiation TermsNegotiationById(int? id_TermsNegotiation)
		{
			db = new DBContext();
			return db.TermsNegotiation.FirstOrDefault(v => v.id == id_TermsNegotiation);
		}

		public static IEnumerable AllTermsNegotiation()
		{
			db = new DBContext();
			var model = db.TermsNegotiation.ToList();

			return model;
		}

		public static IEnumerable TermsNegotiationWithCurrent(int? id_current)
		{
			db = new DBContext();
			return db.TermsNegotiation.Where(g => (g.isActive) || g.id == id_current).ToList();
		}

		public static IEnumerable TermsNegotiationAll(int? id_current)
		{
			db = new DBContext();
			var TermsNegotiationaux = db.TermsNegotiation.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in TermsNegotiationaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var TermsNegotiationcuuretaux = db.TermsNegotiation.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

					TermsNegotiationaux.AddRange(TermsNegotiationcuuretaux);
				}
			}

			return TermsNegotiationaux;

		}
	}
}