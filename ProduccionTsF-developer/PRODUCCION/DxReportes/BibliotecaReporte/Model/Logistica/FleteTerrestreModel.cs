using System;

namespace BibliotecaReporte.Model.Logistica
{
    internal class FleteTerrestreModel
    {
        public string CarRegistration { get; set; }
        public int Flete { get; set; }
        public int Anticipo { get; set; }
        public int Ajuste { get; set; }
        public int Valordias { get; set; }
        public int Extension { get; set; }
        public int Total { get; set; }
        public string Proveedor { get; set; }
        public int Guiaremision { get; set; }
        public DateTime FechaGuiaRemision { get; set; }
        public string DuenoTransporte { get; set; }
        public string CiaFactura { get; set; }
        public string Nombrecia { get; set; }
        public string Ruc { get; set; }
        public string Numdoc { get; set; }
        public string Estadodocumento { get; set; }
        public string Telefonocia { get; set; }
        public decimal FleteCancelado { get; set; }
        public decimal FleteCanceladoTotal { get; set; }
        public string DescripcionRG { get; set; }
        public string Chofer { get; set; }
        public string DescripcionGeneral { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
    }
}
