using DXPANACEASOFT.Models.Dto;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Extensions
{
    public static class AllocationCostPeriodMapper
    {
        public static AllocationCostPeriodDto ToDto(this AllocationCostPeriod input)
        {
            int idCost = 0;
            
            if(input.id_productionCostType.HasValue)
            {
                idCost = input.id_productionCostType.Value;
            }

            int idEmployee = 0;
            if (input.id_employeeApplicant.HasValue)
            {
                idEmployee = input.id_employeeApplicant.Value;
            }

            return new AllocationCostPeriodDto
            {
                id = input.id,
                anio = input.anio,
                mes = input.mes,

                accountingValue = input.accountingValue,                

                id_productionCostType = idCost,
                id_employeeApplicant = idEmployee,

                Document = new Document
                {
                    number = input.Document.number,
                    emissionDate = input.Document.emissionDate,

                    DocumentState = new DocumentState
                    {
                        description = input.Document.DocumentState.description
                    }
                },
            };
        }

    }
}