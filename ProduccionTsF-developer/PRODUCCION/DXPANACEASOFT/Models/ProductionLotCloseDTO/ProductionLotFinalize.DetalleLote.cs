using System;
using System.EnterpriseServices;

namespace DXPANACEASOFT.Models.ProductionLotCloseDTO
{
	partial class ProductionLotFinalize
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		[System.Serializable()]
		public class DetalleLote
		{
			public int Orden { get; set; }
			public string NumeroLote { get; set; }
			public string SecuenciaTransaccional { get; set; }
            //public string Producto { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CantidadLibras { get; set; }
            public decimal CantidadKilos { get; set; }
            
		}
	}
}
