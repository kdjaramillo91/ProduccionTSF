using System;

namespace BibliotecaReporte.Model.Procesos
{
    internal class ProcesoInternoDetalladoModel
    {
        public string MPROLIQ { get; set; }
        public string PRODUNnameNombreUnidadLoteProduccion { get; set; }
        public string PRODPRnameProcesoLoteProduccion { get; set; }
        public DateTime PRODLreceptionDateFechaRecepcion { get; set; }
        public string PRODLnumberNumerodeLote { get; set; }
        public string Estado { get; set; }
        public string CODIGO { get; set; }
        public string RESPONSABLE { get; set; }
        public string INTERNALLOT { get; set; }
        public decimal MERMA { get; set; }
        public string CODEESTADO { get; set; }
        public string LOTORInumberLoteOrigen { get; set; }
        public string ITEMmasterCodeCodigoProducto { get; set; }
        public string ITEMnameNombreItem { get; set; }
        public string ITEMSZnameTalla { get; set; }
        public decimal PRODLDquantityRecivedCantidadRecibidaDetalle { get; set; }
        public string ITY2nameliqTipoProducto { get; set; }
        public decimal tmpquantityPoundsLiquidationliqlibrastotalrecobidas { get; set; }
        public decimal Suma { get; set; }
        public decimal Sumaliquidacion { get; set; }
        public int PMINIMUM { get; set; }
        public int PMAXIMUM { get; set; }
        public DateTime Fi { get; set; }
        public DateTime Ff { get; set; }
        public decimal mermatotal { get; set; }
        public decimal mermatotalGeneral { get; set; }
        public decimal desperdiciototal { get; set; }
        public decimal totalliq { get; set; }
        public decimal totallmp { get; set; }
        public decimal sumadesperdicio { get; set; }

    }
}