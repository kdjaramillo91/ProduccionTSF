using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Extensions
{
    public static class AllocationCostPeriodDetailMapper
    {
        public static AllocationCostPeriodDetailDto ToDto(this AllocationCostPeriodDetail input)
        {
            if (input == null) return new AllocationCostPeriodDetailDto();

            return new AllocationCostPeriodDetailDto
            {
                id = input.id,
                id_allocationCostPeriod = input.id_allocationCostPeriod,
                id_productionExpense = input.id_productionExpense,
                id_processPlant = input.id_processPlant,
                coeficiente = input.coeficiente,
                valor = input.valor,
            };
        }
    }
}