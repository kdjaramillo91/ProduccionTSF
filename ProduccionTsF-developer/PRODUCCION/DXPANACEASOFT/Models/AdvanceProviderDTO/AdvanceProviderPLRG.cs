
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.AdvanceProviderDTO
{
    public class AdvanceProviderPLRG
    {
        public int id { get; set; }

        public int seq_RemissionGuide { get; set; }

        public string OCnumber { get; set; }

        public string RGnumber { get; set; }

        public string Pnumber { get; set; }

        public decimal QuantityPoundsReceived { get; set; }

        public decimal QuantityPoundsScurrid { get; set; }

        public decimal sGrammage { get; set; }

        public decimal Performance { get; set; }
        
    }
}