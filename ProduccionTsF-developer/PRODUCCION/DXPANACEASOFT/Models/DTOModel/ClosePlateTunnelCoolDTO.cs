using System;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.DTOModel
{
	public class ClosePlateTunnelCoolConsultDTO
	{
		public int? id_state { get; set; }
		public string initDate { get; set; }
		public string endtDate { get; set; }
		public string number { get; set; }
		public int? id_turn { get; set; }
		public int? id_machineForProd { get; set; }
		public int? id_person { get; set; }
		public string numberLot { get; set; }
		public int? id_provider { get; set; }
		public int? id_productionUnitProvider { get; set; }
		public int[] items { get; set; }
		public int[] customers { get; set; }
	}
	public class ClosePlateTunnelCoolResultConsultDTO
	{
		public int id { get; set; }
		public string number { get; set; }
		public DateTime closeDate { get; set; }
		public TimeSpan closeTime { get; set; }
		public int id_machineForProd { get; set; }
		public DateTime startEmissionDate { get; set; }
		public string machineForProd { get; set; }
		public string plantProcess { get; set; }
		public string turn { get; set; }
		public string state { get; set; }

		public bool canEdit { get; set; }
		public bool canAproved { get; set; }
		public bool canReverse { get; set; }
		public bool canAnnul { get; set; }
	}

	public class ClosePlateTunnelCoolDTO
	{
		public int id { get; set; }
		public int id_machineProdOpeningDetail { get; set; }
		public int id_documentType { get; set; }
		public string documentType { get; set; }
		public string number { get; set; }
		public string reference { get; set; }
		public DateTime dateTimeEmision { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public string description { get; set; }
		public int idSate { get; set; }
		public string state { get; set; }
		public int id_machineForProd { get; set; }
		public string machineForProd { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public int noOfPerson { get; set; }
		public int idPerson { get; set; }
		public string person { get; set; }
		public DateTime closeDate { get; set; }
		public TimeSpan closeTime { get; set; }
		public List<ClosePlateTunnelCoolDetailDTO> ClosePlateTunnelCoolDetails { get; set; }
		public OpeningClosingPlateLyingTransferDetailDTO[] TransferDetailEntry { get; set; }
		public OpeningClosingPlateLyingTransferDetailDTO[] TransferDetailExit { get; set; }
		public OpeningClosingPlateLyingTransferSummaryDTO[] TransferSummary { get; set; }

		#region Constructor		
		public ClosePlateTunnelCoolDTO()
		{
			this.ClosePlateTunnelCoolDetails = new List<ClosePlateTunnelCoolDetailDTO>();
			this.TransferDetailEntry = new OpeningClosingPlateLyingTransferDetailDTO[] { };
			this.TransferDetailExit = new OpeningClosingPlateLyingTransferDetailDTO[] { };
			this.TransferSummary = new OpeningClosingPlateLyingTransferSummaryDTO[] { };
		}
		#endregion
	}

	public class ClosePlateTunnelCoolDetailDTO
	{
		public int id_inventoryMoveDetail { get; set; }
		public string warehouse { get; set; }
		public string warehouseLocation { get; set; }
		public string productionCart { get; set; }
		public string size { get; set; }
		public string numberInventoryMoveEntry { get; set; }
		public string dateTimeEmisionStr { get; set; }
		public string cod_state { get; set; }
		public string state { get; set; }
		public string nameItem { get; set; }
		public decimal tail { get; set; }
		public decimal whole { get; set; }
		public decimal total { get; set; }
		public string numberLot { get; set; }
		public string plantProcess { get; set; }
		public string customer { get; set; }
		public string provider { get; set; }
        public string nameProviderShrimp { get; set; }
		public string productionUnitProviderPool { get; set; }
		public string machineForProd { get; set; }
		public string numberLiquidationCarOnCar { get; set; }
	}
	public class ClosePlateTunnelCoolPendingNewDTO
	{
		public int id_MachineProdOpeningDetail { get; set; }
		public string numberMachineProdOpening { get; set; }
		public string plantProcess { get; set; }
		public string machineForProd { get; set; }
		public string emissionDateStr { get; set; }
		public DateTime emissionDate { get; set; }
		public int id_turn { get; set; }
		public string turn { get; set; }
		public TimeSpan timeInit { get; set; }
		public TimeSpan? timeEnd { get; set; }
		public string state { get; set; }
	}

	#region Entidad para consulta de Transferencias Entrantes
	public class OpeningClosingPlateLyingTransferDetailDTO
	{
		public string warehouse { get; set; } // Bodega
		public string warehouseLocation { get; set; } // Ubicación
		public string OpeningClosingPlateLying { get; set; } // Tumbada de placa}
		public string dateTimeEmisionStr { get; set; } // fecha de emisión de liquidación carro x carro
		public string cod_state { get; set; } // estado de liquidación carro x carro
		public string state { get; set; } // estado de liquidación carro x carro
		public string machineForProdOrigin { get; set; } // Máquina Origen
		public string machineForProdDestiny { get; set; } // Máquina Destino
		public string productionCart { get; set; } // Coche

		public string numberLot { get; set; } // Número de Lote
		public string nameItem { get; set; } // Nombre de Ítem
		public string size { get; set; } // Talla
		
		public decimal tail { get; set; } // Cola
		public decimal whole { get; set; } // Entero
		public decimal total { get; set; } // Total
	}

	public class OpeningClosingPlateLyingTransferSummaryDTO
	{
		public string transferType { get; set; } // Tipo
		public string machineForProdOrigin { get; set; } // Máquina Origen
		public string machineForProdDestiny { get; set; } // Máquina Destino
		public decimal tail { get; set; } // Cola
		public decimal whole { get; set; } // Entero
		public decimal total { get; set; } // Total
	}
	#endregion
}