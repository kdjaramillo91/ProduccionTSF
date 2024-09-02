using DevExpress.Charts.Native;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.InventoryBalance
{
    public class InventoryBalanceModel
    {
        
        public int Anio { get; set; }
        public int Periodo { get; set; }
        public int id_item { get; set; }
        public int id_warehouse { get; set; }
        public int id_warehouseLocation { get; set; }
        public int? id_productionLot { get; set; }
        public string numberLot { get; set; }
        public string internalNumberLot { get; set; }
        public int? id_metricUnit { get; set; }
        public decimal? SaldoActual { get; set; }
        public DateTime?  fechaRecepcion { get; set; }
    }

    public class InvParameterBalance
    {
        public int id_company { get; set; }
        public int id_Item { get; set; }
        public DateTime cut_Date { get; set; }
        public int? id_Warehouse { get; set; }
        public int? id_WarehouseLocation { get; set; }
        public int? id_ProductionLot { get; set; }
        public bool consolidado { get; set; }

    }

    public class InvParameterBalanceGeneral
    {
        public int id_company { get; set; }
        public int? id_Item { get; set; }
        public DateTime? cut_Date { get; set; }
        public int? id_Warehouse { get; set; }
        public int? id_WarehouseLocation { get; set; }
        public int? id_ProductionLot { get; set; }
        public bool consolidado { get; set; }
        public string groupby { get; set; }
        public bool? requiresLot { get; set; }

        public int? id_productionCart { get; set; }
        public string lotMarket { get; set; }

        public string idItemList { get; set; }
        public string idInventoryDetails { get; set; }
    }
}