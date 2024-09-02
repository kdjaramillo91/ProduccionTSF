using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Auxiliares.IntegrationProcessService
{
	public class IntegrationProcessOutputAux
	{
		public int idObjectParent1 { get; set; }
		public int idObjectParent2 { get; set; }
		public int idObjectParent3 { get; set; }
		public int idObjectParent4 { get; set; }
        public int idObjectParent5 { get; set; }
        public string textoObjectParent1 { get; set; }
		public string textoObjectParent2 { get; set; }
		public string textoObjectParent3 { get; set; }
		public decimal decimalObjectParent1 { get; set; }

		public List<IntegrationProcessOutputAuxDetail> ObjectChild { get; set; }

		public IntegrationProcessOutputAux()
		{
			this.ObjectChild = new List<IntegrationProcessOutputAuxDetail>();
		}
	}

	public class IntegrationProcessOutputAuxDetail
	{
		public int idObjectChild1 { get; set; }
		public int idObjectChild2 { get; set; }
		public int idObjectChild3 { get; set; }

		public int idObjectChild4 { get; set; }
		public int idObjectChild5 { get; set; }

		public DateTime fechaObjectChild1 { get; set; }
		public decimal decimalObjectChild1 { get; set; }
	}

	public class IntegrationProcessGlossAux
	{
		public int idDocument { get; set; }
		public string gloss { get; set; }
	}

	public class IntegrationProcessGlossDataAux
	{
		public string field { get; set; }
		public string valueGloss { get; set; }
	}

	public class IntegrationProcessGlossDataAuxHead
	{
		public int idDocument { get; set; }
		public List<IntegrationProcessGlossDataAux> infoGloss { get; set; }
	}

	public class IntegrationProcessDetailTotal
	{
		public int idDocument { get; set; }
		public decimal valueTotal { get; set; }
	}
}