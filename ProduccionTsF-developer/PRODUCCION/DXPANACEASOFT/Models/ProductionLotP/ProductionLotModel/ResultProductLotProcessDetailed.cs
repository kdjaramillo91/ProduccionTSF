using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultProductLotProcessDetailed
    {
        public int IDPL { get; set; }
		public string MPROLIQ { get; set; }
		public string PRODUNnameNombreUnidadLoteProduccion { get; set; }
		public string PRODPRnameProcesoLoteProduccion { get; set; }
		public DateTime? PRODLreceptionDateFechaRecepcion { get; set; }
		public string PRODLnumberNumerodeLote { get; set; }
		public decimal? PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras { get; set; }
		public string PRODLinternalNumberNumeroInterno { get; set; }
		public decimal? PRODLtotalQuantityRemittedTCLREM { get; set; }
		public string WAREnameBodega { get; set; }
		public string WARELOCnameUbicacionBodega { get; set; }
		public string Estado { get; set; }
		public int? EstadoId { get; set; }
		public string FECHADESDE { get; set; }
		public string FECHAHASTA { get; set; }
		public string CODIGO { get; set; }
		public string RESPONSABLE { get; set; }
		public string INTERNALLOT { get; set; }
		public decimal? MERMA { get; set; }
		public string CODEESTADO { get; set; }
		public decimal? RENDIMIENTO { get; set; }
		public string LOTORInumberLoteOrigen   { get; set; }
		public string ITEMmasterCodeCodigoProducto{ get; set; }
		public string ITEMnameNombreItem { get; set; }
		public string ITEMSZnameTalla { get; set; }
		public decimal? PRODLDquantityRecivedCantidadRecibidaDetalle { get; set; }
		public string METRICcodeUnidadMedida { get; set; }
		public string ITY2nameliqTipoProducto { get; set; }
		public decimal? tmpquantityPoundsLiquidationliqlibrastotalrecobidas { get; set; }
		public string PRODLDescriptionObservaciones { get; set; }
		public string CODIGOSTATE { get; set; }
		public decimal? suma { get; set; }
		public decimal? sumaliquidacion { get; set; }
		public decimal? sumadesperdicio { get; set; }
		public decimal? sumamerma { get; set; }
		public decimal? PMINIMUM { get; set; }
		public int PMAXIMUM { get; set; }
		public int PRESEID { get; set; }
		public bool? OCMERMA { get; set; }
		public decimal? LibsDetalle { get; set; }
		public decimal? Libsliquidacion { get; set; }
		public decimal? mermatotal { get; set; }
		public decimal? mermatotalGeneral { get; set; }
		public decimal? desperdiciototal { get; set; }
		public decimal? totalliq { get; set; }
		public decimal? totalmp { get; set; }

	}
}