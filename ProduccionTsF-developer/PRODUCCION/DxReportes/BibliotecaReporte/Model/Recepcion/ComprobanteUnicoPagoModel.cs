using System;
namespace BibliotecaReporte.Model.Recepcion
{
   internal class ComprobanteUnicoPagoModel
    {
        public decimal PrecioUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Redimientototal { get; set; }
        public string GRecepcion { get; set; }
        public string Guiarecepcion { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public string Ruc_proveedor { get; set; }
        public decimal Pl_totalQuantityRecived { get; set; }
        public decimal Pl_totalQuantityLiquidationAdjust { get; set; }
        public decimal Pl_wholeSubtotal { get; set; }
        public decimal PL_wholeSubtotalAdjust { get; set; }
        public decimal Pl_subtotalTailAdjust { get; set; }
        public string Imp { get; set; }
        public string AcuerdoMinisterial { get; set; }
        public string NumeroTramite { get; set; }
        public string NamePool { get; set; }
        public string NameProveedor { get; set; }
        public string GuiaTransporte { get; set; }
        public string Guia_Proveedor { get; set; }
        public string Name_Cia { get; set; }
        public string Ruc_Cia { get; set; }
        public string Telephone_Cia { get; set; }
        public decimal Grammage { get; set; }
        public string Typename { get; set; }
        public string Name_item_short { get; set; }
        public byte[] Logo { get; set; }
        public string Itemsizename { get; set; }
        public decimal WholeGarbagePounds { get; set; }
        public decimal PoundsGarbageTail { get; set; }
        public string Lista { get; set; }
        public decimal BasuraCola { get; set; }
        public string Clase { get; set; }
        public decimal Resta { get; set; }
        public decimal Avancerounded { get; set; }
        public decimal Sobrante { get; set; }
        public string Descripcion { get; set; }
        public int Secuencial { get; set; }
        public int RendimientoCola { get; set; }
        public decimal TotalPagarLote { get; set; }
        public string EstadoDocumento { get; set; }
        public byte[] FirmaAutorizadoPor { get; set; }
        public string Proceso { get; set; }

    }
}
