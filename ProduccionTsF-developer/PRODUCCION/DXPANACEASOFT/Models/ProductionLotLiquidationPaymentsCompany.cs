using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class ProductionLotLiquidationPaymentsCompany
    {
        public string liquidationPaymentDate { get; set; }
        public string provider { get; set; }
        public string number { get; set; }
        public List<int> list_id_productionLotLiquidationPayment { get; set; }
        public List<ProductionLotLiquidationPaymentDetailReport> listProductionLotLiquidationPaymentDetail { get; set; }
        public decimal logisticTotalLbs { get; set; }
        public decimal logisticPrecioLbs { get; set; }
        public decimal logisticTotalToPay { get; set; }
        public decimal liquidationPaymentTotalLbs { get; set; }
        public decimal liquidationPaymentTotalToPay { get; set; }
        public decimal rteFlePorCientoValor { get; set; }
        public string rteFlePorCientoName { get; set; }
        public decimal total { get; set; }
        public Company company { get; set; }

    }
}