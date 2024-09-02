using System;

namespace BibliotecaReporte.Model.facturasComercial
{
    internal class FacturaComercialModel
    {
        public string NumeroFactura { get; set; }
        public string RazonSocialComprador { get; set; }
        public string NumeroDae { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaEmbarque { get; set; }
        public decimal ValorFob { get; set; }
        public string NombreCia { get; set; }
        public string RucCia { get; set; }
        public byte[] Logo2Cia { get; set; }
        public string Telefono { get; set; }

        public decimal IGCAPITAL { get; set; }
        public decimal CARTADECREDITO { get; set; }
        public decimal BANCOBOLIVARIANO { get; set; }
        public decimal COBRANZASBANCARIAS { get; set; }
        public decimal NOPROBADAS { get; set; }
        public decimal PRONTOPAGO { get; set; }
        public int valorkilos { get; set; }
        public int valorlibras { get; set; }
        public int totalLibras { get; set; }
       
    }
}