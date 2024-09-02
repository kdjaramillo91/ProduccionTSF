using System;

namespace BibliotecaReporte.Model.Inventario
{
    internal class TransferEgresoModel
    {
        public int ID { get; set; }
        public string Bodega { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaEmision { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string NumeroSecuencia { get; set; }
        public string NombreUbicacion { get; set; }
        public string CodigoNaturaleza { get; set; }
        public string SecuenciaRequisicion { get; set; }
        public string SecuenciaLiquidacionMateriales { get; set; }
        public string Descripcion { get; set; }
        public string numLote { get; set; }
        public string SecTransac { get; set; }
        public string EstadoDocumento { get; set; }
        public string itemTalla { get; set; }
        public string tipoItem { get; set; }
        public string nombreUsuario { get; set; }
        public string bodegaIngreso { get; set; }
        public decimal Libras { get; set; }
        public decimal Kilos { get; set; }
    }
}