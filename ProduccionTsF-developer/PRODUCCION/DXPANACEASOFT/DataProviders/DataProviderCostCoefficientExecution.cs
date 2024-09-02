using DXPANACEASOFT.Models;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderCostCoefficientExecution
	{
		private const string m_PendienteDocumentState = "01";
		private const string m_AprobadoDocumentState = "03";
		private const string m_AnuladoDocumentState = "05";

		public static IEnumerable<DocumentState> DocumentStatesByDocumentType(int? id_company)
		{
			return id_company.HasValue
				? GetQueryableDocumentState(id_company.Value)
					.ToArray()
				: new DocumentState[] { };
		}
		public static DocumentState GetDocumentStateByCode(int? id_company, string code)
		{
			return id_company.HasValue
				? GetQueryableDocumentState(id_company.Value)
					.FirstOrDefault(t => t.code == code)
				: null;
		}
		private static IQueryable<DocumentState> GetQueryableDocumentState(int id_company)
		{
			return new DBContext()
				.DocumentState
				.Where(t => (t.code == m_PendienteDocumentState)
							|| (t.code == m_AprobadoDocumentState)
							|| (t.code == m_AnuladoDocumentState))
				.Where(t => !t.name.Contains("PENDIENTE "))
				.Where(t => (t.id_company == id_company) && t.isActive);
		}

		public static IEnumerable<InventoryValuationPeriod> AllocationCostYear()
		{
			return new DBContext()
				.InventoryValuationPeriod
				.Where(t => t.isActive)
				.OrderByDescending(t => t.year)
				.ToArray();
		}
	}
}