using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel
{
    public class ResultSupplierLiquidationResumen
    {
        public int numLiquidaciones { get; set; }
        public string etiquetaComprador { get; set; }
        public string proveedor { get; set; }
        public decimal? LibrasDespachadas { get; set; }
        public decimal? LibrasRemitidas { get; set; }
        public decimal? LibrasProcesadas { get; set; }
        public decimal? KilosCabeza { get; set; }
        public decimal? LibrasCola { get; set; }
        public decimal? RendimientoCabeza { get; set; }
        public decimal? RendimientoCola { get; set; }
        public decimal? ValorCabeza { get; set; }
        public decimal? ValorCola { get; set; }
        public decimal? dolaresTotal { get; set; }
        public decimal? PrecioCabeza { get; set; }
        public decimal? PrecioCola { get; set; }
        public decimal? PrecioTotal { get; set; }

    }
}