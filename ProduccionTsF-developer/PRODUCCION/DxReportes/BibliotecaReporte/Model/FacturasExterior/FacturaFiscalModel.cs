using System;

namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class FacturaFiscalModel
    {
        public int ID { get; set; }
        public string NumeroFactura { get; set; }
        public string RazonSocialComprador { get; set; }

        public string NumeroDae { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaEmbarque { get; set; }
        public decimal ValorFob { get; set; }
        public decimal PesoKilos { get; set; }
        public decimal PesoLibras { get; set; }
        public decimal TotalLibras { get; set; }
        public string NombreCia { get; set; }
        public string RucCia { get; set; }
        public byte[] LogoCia { get; set; }
        public byte[] Logocompany { get; set; }
        public string Telefono { get; set; }
    }
}