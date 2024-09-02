using System;

namespace BibliotecaReporte.Model.Compras
{
    internal class RequerimientoCompraModel
    {
        public string NombreCia { get; set; }
        public string RucCia { get; set; }
        public string TelefonoCia { get; set; }
        public string DireccionCia { get; set; }
        public string NumeroDocumento { get; set; }
        public string Estado { get; set; }
        public string Solicitante { get; set; }
        public DateTime FechaEmision { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public string NombreProveedor { get; set; }
        public string GramajeDesde { get; set; }
        public string GramajeHasta { get; set; }
        public string ColorReferencia { get; set; }
        public decimal CantidadRequerida { get; set; }
        public decimal CantidadAprobada { get; set; }
        public decimal CantidadPendiente { get; set; }       
    }
}