using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class MovimientoEgresoMasterizadoModel
    {
        public DateTime FECHAEMISION { get; set; }
        public string ESTADO { get; set; }
        public string CODIGO1 { get; set; }
        public string TIPOITEM1 { get; set; }
        public string NPRODUCTO1 { get; set; }
        public string UN { get; set; }
        public string TALLA1 { get; set; }
        public string BODEGA1 { get; set; }
        public string UBODEGA1 { get; set; }
        public decimal Cantidad1 { get; set; }
        public string CENTROCOSTO2 { get; set; }
        public string SUBCENTROCOSTO2 { get; set; }
        public decimal LBSOKG1 { get; set; }
        public decimal Kgolb1 { get; set; }
        public string RazonEgreso { get; set; }
        public string SecEgreso { get; set; }
        public string Lotenew { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Inlote { get; set; }
        public string NLOTE { get; set; }
        public string LoteMP { get; set; }

    }
}
