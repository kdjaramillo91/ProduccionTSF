using System;
namespace BibliotecaReporte.Model.OrdenesDeProduccion
{
   internal class OrdenProduccionMaterialModel
    {
        public string Codigo { get; set; }
        public string NombreDelProducto { get; set; }
        public string Estado { get; set; }
        public string LineaDeInventario { get; set; }
        public string TipoDeProducto { get; set; }
        public string Categoria { get; set; }
        public string codigo { get; set; }
        public string Ingrediente { get; set; }
        public decimal CantFormulada { get; set; }
        public decimal Cantidad { get; set; }
        public string UM { get; set; }
        public string TipodeDocuemnto { get; set; }
        public string NumeroDeProforma { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Cliente { get; set; }
        public string Destino { get; set; }
        public DateTime FechaEmbarque { get; set; }
        public string FormaDepago { get; set; }
        public string NumeroDocumento { get; set; }
        public string Descripcion { get; set; }
        public string Solicitante { get; set; }
        public byte[] Logo { get; set; }        

    }
}
