using System;
namespace BibliotecaReporte.Model.Procesos
{
   internal class MatrizProcesoInterno2Model
    {
        public int ID { get; set; }
        public string Tipo { get; set; }
        public string UnidadDeProduccion { get; set; }
        public string Proceso { get; set; }
        public DateTime FechaDeRecepcion { get; set; }
        public string LoteSecuenciaInterna { get; set; }
        public string NumeroDeLote { get; set; }
        public string Bodega { get; set; }
        public string Estado { get; set; }
        public string Responsable { get; set; }
        public string NombreDeLote { get; set; }
        public string CodigoDeItem { get; set; }
        public string NombreDeItem { get; set; }
        public string Talla { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadDeMedida { get; set; }
        public string TipoDeProducto { get; set; }
        public decimal Libras { get; set; }
        public string Observacion { get; set; }
        public string Presentacion { get; set; }
        public decimal Minimo { get; set; }
        public int Maximo { get; set; }
        public int UnidadDeLaPresentacion { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public string Grupo { get; set; }
        public string Subgrupo { get; set; }        
    }
}
