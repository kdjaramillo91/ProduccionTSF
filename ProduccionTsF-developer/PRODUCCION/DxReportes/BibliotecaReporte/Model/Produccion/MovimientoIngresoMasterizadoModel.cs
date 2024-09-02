using System;
namespace BibliotecaReporte.Model.Produccion
{
   internal  class MovimientoIngresoMasterizadoModel
    {
        public int ID { get; set; }
        public DateTime FECHAEMISION { get; set; }
        public string ESTADO { get; set; }
        public string NLOTE { get; set; }
        public string INLOTE { get; set; }
        public string TIPOITEM1 { get; set; }
        public string CODIGO2 { get; set; }
        public string NPRODUCTO1 { get; set; }
        public string UN { get; set; }
        public string TALLA1 { get; set; }
        public string CENTROCOSTO1 { get; set; }
        public string SUBCENTROCOSTO1 { get; set; }
        public string BODEGA2 { get; set; }
        public string UBODEGA2 { get; set; }
        public decimal Cantidad2 { get; set; }
        public decimal Cantidad3 { get; set; }
        public decimal LBSOKG2 { get; set; }
        public decimal Kgolb2 { get; set; }
        public string RazonMaster { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string LoteMP { get; set; }
        public string NLoteP { get; set; }
    }
}
