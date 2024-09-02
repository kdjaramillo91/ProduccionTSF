using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultRemisionGuideTransportViatic
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string ruc { get; set; }
        public string telefono { get; set; }
        public string documento { get; set; }
        public int NumeroAnticipo { get; set; }
        public DateTime emissionDate { get; set; }
        public string placa { get; set; }
        public string Chofer { get; set; }
        public string CipersonaChofer { get; set; }
        public string Transportista { get; set; }
        public string UsuarioPago { get; set; }
        public decimal viatico { get; set; }


    }
}