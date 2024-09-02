using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class CostAllocationDto
    {

        public int id { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public int id_Warehouse { get; set; }
        public string[] warehouses { get; set; }
        public List<WarehouseModelDto> id_Warehousessex { get; set; }
        public string warehousesNames { get; set; }
        //public int id_InventoryPeriodDetail { get; set; }
        public System.DateTime fechaIncio { get; set; }
        public System.DateTime fechaFin { get; set; }
        public string EstadoPeriodoBodega { get; set; }

        public Document Document { get; set; }
        public CostAllocationInventoryPeriodDetailDto InventoryPeriodDetail { get; set; }
        public CostAllocationWarehouseDto Warehouse { get; set; }
        public List<CostAllocationResumidoDto> CostAllocationResumido { get; set; }
        public List<CostAllocationDetalleDto> CostAllocationDetalle { get; set; }

        public bool IsCalculate { get; set; }
        public bool IsChangeResumen { get; set; }
    }


    public class CostAllocationInventoryPeriodDetailDto
    {
        public int id { get; set; }
        public int id_InventoryPeriod { get; set; }
        public int periodNumber { get; set; }
        public System.DateTime dateInit { get; set; }
        public System.DateTime dateEnd { get; set; }
        public int id_PeriodState { get; set; }
        public bool isClosed { get; set; }
        public Nullable<System.DateTime> dateClose { get; set; }
        public Nullable<System.DateTime> dateAccounting { get; set; }
    }

    public class CostAllocationWarehouseDto
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int id_warehouseType { get; set; }
        public int id_inventoryLine { get; set; }
        public bool allowsNegativeBalances { get; set; }
        public bool isActive { get; set; }
    }

}