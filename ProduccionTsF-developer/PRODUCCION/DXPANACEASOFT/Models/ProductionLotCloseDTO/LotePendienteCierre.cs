using System;

namespace DXPANACEASOFT.Models.ProductionLotCloseDTO
{
    public class LotePendienteCierre
    {
        public int IdLote { get; set; }
        public string TipoLote { get; set; }
        public string SecuenciaTransaccional { get; set; }
        public string NumeroLote { get; set; }
        public string LoteJuliano { get; set; }
        public DateTime FechaProceso { get; set; }
        public string UnidadProduccion { get; set; }
        public string Proceso { get; set; }
        public decimal Stock { get; set; }
        public string Estado { get; set; }
    }

}