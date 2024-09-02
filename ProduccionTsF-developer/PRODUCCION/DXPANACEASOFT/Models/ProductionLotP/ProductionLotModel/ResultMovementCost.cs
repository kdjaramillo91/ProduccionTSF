using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultMovementCost
    {
        public string periodo { get; set; }
        public string periodoValores { get; set; }
        public string nameBodega { get; set; }
        public string grupo { get; set; }
        public decimal valores { get; set; }
    }
}