using System;
namespace BibliotecaReporte.Model.Recepcion
{
   internal class LiquidacionCompraCamaronModel
    {
        public decimal Precio { get; set; }
        public string N { get; set; }
        public string Lote { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public string InpNumeroProveedor { get; set; }
        public string MinisterialAgreementPLProveedor { get; set; }
        public string TramitNumberPLProveedor { get; set; }
        public string Piscina { get; set; }
        public string GuiaRemision { get; set; }
        public string ItemSizeName { get; set; }
        public decimal PLTOTALTOPAY { get; set; }
        public decimal CABEZABASURA { get; set; }
        public decimal COLABASURA { get; set; }
        public decimal AVANCEROUNDED { get; set; }
        public string NameItem { get; set; }
        public decimal Kilolibrasform { get; set; }
        public string NombreProveedor { get; set; }
        public string RucProveedor { get; set; }
        public string Lista { get; set; }
        public string NombreTipoItem { get; set; }
        public byte[] Logo { get; set; }
        public string Comprador { get; set; }
        public string TipoProceso { get; set; }
        public decimal LibrasRemitidas { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Cajas { get; set; }
        public decimal BasuraCola { get; set; }
        public decimal Resta { get; set; }
        public decimal Sobrante { get; set; }
        public string Observacion { get; set; }
        public string EstadoDocumento { get; set; }
        public int Secuencial { get; set; }
        public string Camaronera { get; set; }
        public decimal PorcCola { get; set; }
        public decimal PorcColaProductoEntero { get; set; }
        public decimal PL_wholeSubtotal { get; set; }
        public decimal TotalPagarLote { get; set; }
        public string Proceso { get; set; }
        public string Clase { get; set; }
    }
}
