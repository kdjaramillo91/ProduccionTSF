using System;
namespace BibliotecaReporte.Model.Produccion
{
   internal  class MasterizadoModel
    {
        public DateTime FECHAEMISION { get; set; }
        public DateTime FECHAINICIO { get; set; }
        public DateTime FECHAFIN { get; set; }
        public string NDOCUMENTO { get; set; }
        public string Descripcion { get; set; }
        public string ESTADO { get; set; }
        public string RESPONSABLE { get; set; }
        public string TURNO { get; set; }
        public string INLOTE { get; set; }
        public string CODIGO1 { get; set; }
        public string NPRODUCTO1 { get; set; }
        public string TALLA1 { get; set; }
        public string MARCA1 { get; set; }
        public string BODEGA1 { get; set; }
        public decimal Cantidad1 { get; set; }
        public string PRESENTACION1 { get; set; }
        public decimal LBSOKG1 { get; set; }
        public decimal Kgolb1 { get; set; }
        public string SecEgreso { get; set; }
        public string CODIGO2 { get; set; }
        public string NPRODUCTO2 { get; set; }
        public string TALLA2 { get; set; }
        public string MARCA2 { get; set; }
        public string BODEGA2 { get; set; }
        public string OP { get; set; }
        public string CLIENTE { get; set; }
        public string PRESENTACION2 { get; set; }
        public decimal KILOS2 { get; set; }
        public decimal LIBRAS2 { get; set; }
        public decimal LBSOKG2 { get; set; }
        public decimal Kgolb3 { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public decimal Cantidad2 { get; set; }
        public string LoteMP { get; set; }

    }
}
