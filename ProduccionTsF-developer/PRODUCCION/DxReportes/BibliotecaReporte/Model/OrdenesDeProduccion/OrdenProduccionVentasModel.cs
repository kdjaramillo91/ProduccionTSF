using System;

namespace BibliotecaReporte.Model.OrdenesDeProduccion
{
    internal class OrdenProduccionVentasModel
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set;  }
        public string Solicitante { get; set; }
        public string Cliente { get; set; }
        public string NumeroProforma { get; set; }
        public string FormaDePago { get; set; }
        public DateTime FechaEmbarque { get; set; }
        public string Destino { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public string DescripcionDetalle { get; set; }
        public string Marca { get; set; }
        public string Talla { get; set; }
        public string Empaque { get; set; }
        public decimal Cartones { get; set; }
        public decimal Kilos { get; set; }
        public decimal Libras { get; set; }
        public string VentaUsuario { get; set; }
        public string DirectorVenta { get; set; }
        public string NumeroLote { get; set; }
        public string Motivo { get; set; }
        public string Instrucciones { get; set; }
        public string RucProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string TelefonoProveedor { get; set; }
        public string DireccionProveedor { get; set; }
        public string CodigoPlanta { get; set; }
        public string FDA { get; set; }

        public Detalles[] IntruccionesDetalle { get; set; }
        public class Detalles
        {
            public string Document { get; set; }
            public int Copy { get; set; }
            public int Digital { get; set; }
        }
    }
}
