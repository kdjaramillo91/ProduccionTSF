
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.PLDTO
{
    public class PLliqTotalDTO
    {
        public int id_item { get; set; }

        public int? id_metricUnit { get; set; }

        public decimal quantity { get; set; }

        public decimal? quantityEntered { get; set; }

        public decimal? quantityPounds { get; set; }
        
    }
}