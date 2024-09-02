using System;
namespace BibliotecaReporte.Model.Logistica
{
   internal  class FleteFluvialModel
    {
        public int Flete { get; set; }
        public int Anticipo { get; set; }
        public int Ajuste { get; set; }
        public int Valordias { get; set; }
        public int Extension { get; set; }
        public int Total { get; set; }
        public string CarRegistration { get; set; }
        public string Proveedor { get; set; }
        public DateTime FechaEmision { get; set; }
        public string GuiaRemision { get; set; }
        public string DuenoTransporte { get; set; }
        public string CiaFactura { get; set; }
        public string NombreCia { get; set; }
        public string Ruc { get; set; }
        public string NumDoc { get; set; }
        public string EstadoDocumento { get; set; }
        public string TelefonoCia { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string DescripcionGuia { get; set; }
        public decimal FleteCanceladoFluvial { get; set; }
        public string NumeroFactura { get; set; }
        public string DescripcionLiquidacion { get; set; }
    }
}
