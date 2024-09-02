using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultRemisionGuieT
    {
        public int id { get; set; }
        public int Secuencial { get; set; }
        public string NumeroGuia { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ProveedorCompleto { get; set; }
        public string FechaDespacho { get; set; }
        public decimal LibrasProgramadas { get; set; }
        public decimal LibrasRemitidas { get; set; }
        public string Comprador { get; set; }
        public string TipoGuia { get; set; }
        public string Estado { get; set; }
        public string PlantaProceso { get; set; }
        public string NombreCompania { get; set; }
        public string RucCompania { get; set; }
        public string TelefonoCompania { get; set; }
    }
}