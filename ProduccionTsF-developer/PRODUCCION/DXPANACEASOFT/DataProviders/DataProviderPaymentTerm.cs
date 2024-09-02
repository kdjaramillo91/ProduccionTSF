using DXPANACEASOFT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderPaymentTerm
	{
		private static DBContext db = null;

		public static IEnumerable AllPaymentTerms()
		{
			db = new DBContext();
			var model = db.PaymentTerm.ToList();

			return model;
		}
		public static IEnumerable AllPaymentTermsByCompany(int? id_company)
		{
			db = new DBContext();

			return db.PaymentTerm.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

		}

		public static IEnumerable PaymentsTerms(int id_company)
		{
			db = new DBContext();
			var model = db.PaymentTerm.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}
			return model;
		}

		public static IEnumerable PaymentsTermsForPurchaseOrder(int id_company)
		{
			db = new DBContext();
			var model = db.PaymentTerm.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}
			var lstRestriction = db.CodeProgramGroupTable.Where(w => w.code == "01PTFPO").ToList();

			List<PaymentTerm> lstFinal = new List<PaymentTerm>();
			if (lstRestriction != null && lstRestriction.Count > 0)
			{
				lstFinal = (from v in model
							join d in lstRestriction on v.id equals d.id_foreignKey
							orderby d.orderNatural
							select v).ToList();
			}
			else
			{
				lstFinal = model;
			}

			return lstFinal;
		}

		public static PaymentTerm PaymentTermById(int? id_paymentTerm)
		{
			db = new DBContext(); ;
			return db.PaymentTerm.FirstOrDefault(v => v.id == id_paymentTerm);
		}

		public static IEnumerable PaymentTermAll(int? id_current)
		{
			db = new DBContext();
			var PaymentTermyaux = db.PaymentTerm.Where(g => (g.isActive)).Select(p => new { p.id, name = p.name }).ToList();

			if (id_current != null && id_current > 0)
			{
				var cant = (from de in PaymentTermyaux
							where de.id == id_current
							select de).ToList().Count;
				if (cant == 0)
				{
					var PaymentTermycuuretaux = db.PaymentTerm.Where(g => (g.id == id_current)).Select(p => new { p.id, name = p.name });

					PaymentTermyaux.AddRange(PaymentTermycuuretaux);
				}
			}

			return PaymentTermyaux.OrderBy(x => x.name);

		}

		public static IEnumerable InvoiceExteriorPaymentsTermsByPaymentsMethodandCurrent(int? id_PaymentsMethod, int? id_PaymentTerm)
		{
			db = new DBContext();

            if (id_PaymentsMethod.HasValue)
            {
				var terminos = db.PaymentMethodPaymentTerm
					.Where(r => r.id_paymentMethod == id_PaymentsMethod && r.isActive)
					.ToList();

				return terminos
					.Select(r => new
					{
						r.PaymentTerm.id,
						r.PaymentTerm.code,
						r.PaymentTerm.description
					})
					.ToList();
			}

			return null;
			
		}

	}
}