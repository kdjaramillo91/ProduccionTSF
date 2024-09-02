using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Extensions
{
    public static class CostAllocationMapper
    {
        public static CostAllocationDto ToDto(this CostAllocation input)
        {
            return new CostAllocationDto
            {
                id = input.id,
                anio = input.anio,
                fechaFin = input.fechaFin,
                fechaIncio = input.fechaIncio,
                //id_InventoryPeriodDetail = input.id_InventoryPeriodDetail,
                //id_Warehouse = input.id_Warehouse,
                //Warehouse = input.Warehouse.ToDto(),
                Document = new Document
                {
                    number = input.Document.number,
                    DocumentState = new DocumentState
                    {
                        description = input.Document.DocumentState.description
                    }
                },
                mes = input.mes,
                warehouses = input.CostAllocationWarehouse
                                                .Select(r => r.id_Warehouse.ToString())
                                                .ToArray(),
                id_Warehousessex = input.CostAllocationWarehouse
                                            .Select(r=> new WarehouseModelDto
                                            { 
                                                 Id = r.id_Warehouse,
                                                 Id_InventoryPeriodDetail = r.id_InventoryPeriodDetail
                                            }).ToList(),
                 warehousesNames = input.CostAllocationWarehouse
                                            .Select(r => r.Warehouse.name)
                                            .Aggregate((i,j)=> i+","+j )

            };
        }

    }

    public static class CostAllocationInventoryPeriodDetailMapper
    {
        public static CostAllocationInventoryPeriodDetailDto ToDto(this InventoryPeriodDetail input)
        {
            return new CostAllocationInventoryPeriodDetailDto
            {
                id = input.id,
                dateAccounting = input.dateAccounting,
                dateClose = input.dateClose,
                dateEnd = input.dateEnd,
                dateInit = input.dateInit,
                id_InventoryPeriod = input.id_InventoryPeriod,
                id_PeriodState = input.id_PeriodState,
                isClosed = input.isClosed,
                periodNumber = input.periodNumber
            };
        }
    }

    public static class CostAllocationWarehouseMapper
    {
        public static CostAllocationWarehouseDto ToDto(this Warehouse input)
        {
            return new CostAllocationWarehouseDto
            {
                id = input.id,
                allowsNegativeBalances = input.allowsNegativeBalances,
                code = input.code,
                description = input.description,
                id_inventoryLine = input.id_inventoryLine,
                id_warehouseType = input.id_warehouseType,
                isActive = input.isActive,
                name  = input.name
            };
        }
    }

}