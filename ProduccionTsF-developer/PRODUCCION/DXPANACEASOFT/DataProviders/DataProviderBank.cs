using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderBank
	{
		private static DBContext db = null;

		public static IEnumerable GetAllActive()
		{
			db = new DBContext();
			var list = db.BoxCardAndBank.Where(c => c.isActive).ToList();
			return list;
		}

		public static IEnumerable GetAllBankActive()
		{
			db = new DBContext();

			var idTypeBoxCardAndBank = db.TypeBoxCardAndBank.FirstOrDefault(e => e.code == "BAN")?.id;
            if (idTypeBoxCardAndBank.HasValue)
            {
				return db.BoxCardAndBank.Where(c => c.isActive && c.id_typeBoxCardAndBank == idTypeBoxCardAndBank.Value).ToList();
			}
            else
            {
				return null;
            }
		}
	}
}