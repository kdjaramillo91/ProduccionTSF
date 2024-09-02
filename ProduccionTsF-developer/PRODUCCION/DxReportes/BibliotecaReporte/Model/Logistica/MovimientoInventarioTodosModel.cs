using System;

namespace BibliotecaReporte.Model.Logistica
{
    internal class MovimientoInventarioTodosModel
    {
        public int Id { get; set; }
        public string TituloReporte { get; set; }
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
        public decimal Libras { get; set; }
        public decimal Kilos { get; set; }
    }
}
