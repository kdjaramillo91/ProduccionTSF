using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class SummaryProductionLotPaymentDetail
    {
        public int id { get; set; }
        public string titleTotal { get; set; }
        public decimal? subTotalProceso { get; set; }
        public decimal total { get; set; }
        public decimal? percentPerformanceTotal { get; set; }
        public string metricUnitProcess { get; set; }
        public decimal totalProm { get; set; }
        public decimal? percentajeTotal { get; set; }
        public decimal? diferencia { get; set; }

    }
}