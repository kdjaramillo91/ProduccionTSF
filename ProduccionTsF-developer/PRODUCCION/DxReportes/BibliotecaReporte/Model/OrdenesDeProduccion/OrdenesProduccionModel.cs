using System;
namespace BibliotecaReporte.Model.OrdenesDeProduccion
{
    internal class OrdenesProduccionModel
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public string Solicitante { get; set; }
        public string Cliente { get; set; }
        public string NumeroProforma { get; set; }
        public string FormaDepago { get; set; }
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
        public string NumeroLote { get; set; }        
        public string Motivo { get; set; }        
        public string VentaUsuario { get; set; }        
        public string DirectorVenta { get; set; }        
    }
}
