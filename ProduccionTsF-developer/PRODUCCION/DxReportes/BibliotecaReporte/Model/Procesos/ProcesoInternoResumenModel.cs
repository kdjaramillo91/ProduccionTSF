using System;

namespace BibliotecaReporte.Model.Procesos
{
    internal class ProcesoInternoResumenModel
    {
        public string PRODUNnameNombreUnidadLoteProduccion { get; set; }
        public string PRODPRnameProcesoLoteProduccion { get; set; }
        public string Estado { get; set; }
        public decimal Suma { get; set; }
        public decimal Sumaliquidacion { get; set; }
        public DateTime Fi { get; set; }
        public DateTime Ff { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public decimal Sumadesperdicio { get; set; }
        public decimal Sumamerma { get; set; }
        public decimal LibsDetalle { get; set; }
        public decimal Libsliquidacion { get; set; }
        

    }
}