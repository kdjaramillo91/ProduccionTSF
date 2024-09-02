using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class DistribucionPagoResumenModel
    {
        public int Id { get; set; }
        public string NombreProducto { get; set; }
        public string Clase { get; set; }
        public string Talla { get; set; }
        public string ProcesoDetalle { get; set; }
        public decimal RendimientoTotal { get; set; }
        public string UM { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
