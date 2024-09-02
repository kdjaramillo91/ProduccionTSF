using System;

namespace BibliotecaReporte.Model.Inventario
{
   internal  class MovimientoInventarioEgresoTransferenciaAModel
    {
        public string Compania { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public string NDocumento { get; set; }
        public string BodegaEnvio { get; set; }
        public string UbicacionBenvio { get; set; }
        public string Motivoenvio { get; set; }
        public string Despachador { get; set; }
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CntLibras { get; set; }
        public decimal CntKilos { get; set; }
        public string Lote { get; set; }
        public string SecuenciaTransaccional { get; set; }
        public string Tipo { get; set; }
        public string Talla { get; set; }

    }
}
