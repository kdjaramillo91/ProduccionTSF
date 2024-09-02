using System;
namespace BibliotecaReporte.Model.Procesos
{
    internal class MovimientoInventarioIngresoModel
    {
        public string TituloReporte { get; set; }
        public string Bodega { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaEmision { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal Cantidad { get; set; }
        public int NumeroSecuencia { get; set; }
        public string NombreUbicacion { get; set; }
        public string CodigoNaturaleza { get; set; }
        public string SecuenciaGuiaRemision { get; set; }
        public string Descripcion { get; set; }
        public string SecTransac { get; set; }
        public string EstadoDocumento { get; set; }
        public string TipoItem { get; set; }
        public string ItemTalla { get; set; }
        public string LoteOrigenIngreso { get; set; }
        public decimal Kg { get; set; }
        public decimal Lbskg2 { get; set; }
    }
}
