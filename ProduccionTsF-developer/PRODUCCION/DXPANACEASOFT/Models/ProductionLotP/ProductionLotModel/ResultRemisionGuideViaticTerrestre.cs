using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultRemisionGuideViaticTerrestre
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string ruc { get; set; }
        public string telefono { get; set; }
        public string documento { get; set; }
        public int NumeroViaticoPersonal { get; set; }
        public DateTime emissionDate { get; set; }
        public string cipersona { get; set; }
        public string persona { get; set; }
        public string rol { get; set; }
        public string UsuarioPago { get; set; }
        public decimal viatico { get; set; }

    }
}