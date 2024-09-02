using System;
namespace BibliotecaReporte.Model.Procesos
{
    internal class ProcesoInternoModel
    {
        public string Mproliq { get; set; }
        public string ProdunNameNombreUnidadLoteProduccion { get; set; }
        public string ProdprnameProcesoLoteProduccion { get; set; }
        public DateTime PRODLreceptionDateFechaRecepcion { get; set; }
        public string PRODLnumberNumerodeLote { get; set; }
        public decimal PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras { get; set; }
        public string PRODLinternalNumberNumeroInterno { get; set; }
        public decimal PRODLtotalQuantityRemittedTCLREM { get; set; }
        public string WAREnameBodega { get; set; }
        public decimal Rendimiento { get; set; }
        public string LOTORInumberLoteOrigen { get; set; }
        public string ITEMmasterCodeCodigoProducto { get; set; }
        public string ItemsznameTalla { get; set; }
        public decimal ProdldquantityRecivedCantidadRecibidaDetalle { get; set; }
        public string MetricCodeUnidadMedida { get; set; }
        public decimal TmpquantityPoundsLiquidationliqlibrastotalrecobidas { get; set; }
        public string PRODLDescriptionObservaciones { get; set; }
        public string ITEMnameNombreItem { get; set; }
    }
}
