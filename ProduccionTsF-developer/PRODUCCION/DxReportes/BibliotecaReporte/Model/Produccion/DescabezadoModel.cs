using System;
namespace BibliotecaReporte.Model.Produccion
{
   internal class DescabezadoModel
    {
        public DateTime FechaIngreso { get; set; }
        public int Turno { get; set; }
        public string Proveedor { get; set; }
        public string Piscina { get; set; }
        public string Lote { get; set; }
        public decimal Gramage { get; set; }
        public string Color { get; set; }
        public decimal Rechazo { get; set; }
        public decimal Directo { get; set; }
        public int Kavetas { get; set; }
        public int NPersonas { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public decimal RendimientoManual { get; set; }
        public string Observacion { get; set; }
        public int SecuenciaAgrupacion { get; set; }
        public decimal SumaRechazo { get; set; }
        public decimal SumaDirecto { get; set; }
        public string Contador { get; set; }
        public byte[] Logo { get; set; }
    }
}
