using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class CierreLiquidacionRendimientoResumenModel
    {
        public string NombreMaquina { get; set; }
        public string NombreTurno { get; set; }
        public DateTime FechaEmision { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public decimal LibrasProcesadas { get; set; }
        public string TipoProceso { get; set; }
    }
}
