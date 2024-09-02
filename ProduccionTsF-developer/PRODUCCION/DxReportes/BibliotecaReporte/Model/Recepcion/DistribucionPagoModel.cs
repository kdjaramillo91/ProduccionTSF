using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class DistribucionPagoModel
    {
        public string Lote { get; set; }
        public string SecTransaccional { get; set; }
        public string Camaronera { get; set; }
        public string Piscina { get; set; }
        public string NombreProveedor { get; set; }
        public decimal RendimientoEnteroSinDistribuir { get; set; }
        public decimal RendimientoColaSinDistribuir { get; set; }
        public decimal RendimientoTotalSinDistribuir { get; set; }
        public decimal TotalEnteroSinDistribuir { get; set; }
        public decimal TotalColaSinDistribuir { get; set; }
        public decimal TotalSinDistribuir { get; set; }
        public decimal RendimientoEnteroConDistribucion { get; set; }
        public decimal RendimientoColaConDistribucion { get; set; }
        public decimal RendimientoTotalConDistribucion { get; set; }
        public decimal TotalEnteroConDistribucion { get; set; }
        public decimal TotalColaConDistribucion { get; set; }
        public decimal TotalConDistribucion { get; set; }
        public string Nombre_Cia { get; set; }
        public string Ruc_Cia { get; set; }
        public string Telephone_Cia { get; set; }
        public decimal SecuenciaLiquidacion { get; set; }
        public string EstadoDocumento { get; set; }
        public string Proceso { get; set; }
    }
}
