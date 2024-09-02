using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class LiquidacionCarroXCarroLoteModel
    {
        public int IdLiquidacionCarro { get; set; }
        public string NombreMaquina { get; set; }
        public string NombreTurno { get; set; }
        public DateTime FechaEmision { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public decimal LibrasProcesadas { get; set; }
        public string TipoProceso { get; set; }
    }
}
