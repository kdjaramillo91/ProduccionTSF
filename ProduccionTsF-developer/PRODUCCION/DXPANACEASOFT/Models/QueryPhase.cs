using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class QueryPhase
    {
        public int id_phase { get; set; }
        public Phase phase { get; set; }
        public string listId_BusinessOportunity { get; set; }
        public int quantity { get; set; }
        public decimal amountExpected { get; set; }
        public decimal weightedAmount { get; set; }
        public decimal percent { get; set; }

    }
}