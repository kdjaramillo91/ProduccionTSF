using System;
namespace BibliotecaReporte.Model.Produccion
{
    class TumbadoPlacaIngresoTransferenciaModel
    {
        public string TituloReporte { get; set; }
        public string Bodega { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaEmision { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string CodigoUnidadMedida { get; set; }
        public decimal Cantidad { get; set; }
        public string NumeroSecuencia { get; set; }
        public string NombreUbicacion { get; set; }
        public string CentroCosto { get; set; }
        public string SubCentroCosto { get; set; }
        public string SecuenciaGuiaRemision { get; set; }
        public string SecuenciaLiquidacionMateriales { get; set; }
        public string Descripcion { get; set; }
        public string Numlote { get; set; }
        public string SecTransac { get; set; }
        public string EstadoDocumento { get; set; }
        public string ItemTalla { get; set; }
        public string TipoItem { get; set; }
        public string NombreUsuario { get; set; }
        public string BodegaEgreso { get; set; }
        public string NumeroEgreso { get; set; }
    }
}
