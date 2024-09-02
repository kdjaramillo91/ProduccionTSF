using System;

namespace BibliotecaReporte.Model.Logistica
{
    internal class LogisticaGuiaExcelModel
    {
        public int Secuencial { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ProveedorCompleto { get; set; }
        public string Comprador { get; set; }
        public string FechaDespacho { get; set; }
        public decimal LibrasProgramadas { get; set; }
        public decimal LibrasRemitidas { get; set; }
        public string Estado { get; set; }
        public string NombreCompania { get; set; }
        public string RucCompania { get; set; }
        public string TelefonoCompania { get; set; }
        public string TipoGuia { get; set; }
        public string PlantaProceso { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
    }
}