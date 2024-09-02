using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class AllocationCostPeriodDto
    {
        public int id { get; set; }
        public int id_documentType { get; set; }
        public string documentType { get; set; }
        public string code_documentType { get; set; }
        public string number { get; set; }
        public int idSate { get; set; }
        public string state { get; set; }
        public string dateTimeEmisionStr { get; set; }
        public DateTime dateTimeEmision { get; set; }
        public string reference { get; set; }
        public string employeeApplicant { get; set; }
        public int? id_employeeApplicant { get; set; }
        public string description { get; set; }
        public Document Document { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public int id_productionCostType { get; set; }
        public bool? accountingValue { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public List<AllocationCostPeriodDetailDto> AllocationCostPeriodDetails { get; set; }
        public List<AllocationCostPeriodProductionExpense> id_AllocationCostProductionExpense { get; set; }
        public List<AllocationCostPeriodTrazabilidadDTO> AllocationCostPeriodTrazabilidads { get; set; }
        public bool IsCalculate { get; set; }
    }

    public class AllocationCostPeriodTrazabilidadDTO
    {
        public int id { get; set; }
        public string number { get; set; }
        public int idSate { get; set; }
        public string state { get; set; }
        public string dateTimeEmisionStr { get; set; }
        public DateTime dateTimeEmision { get; set; }
        public Document Document { get; set; }
    }

    public class AllocationCostPeriodDetailDto
    {
        public int id { get; set; }
        public int id_allocationCostPeriod { get; set; }
        public int id_productionExpense { get; set; }
        public int id_processPlant { get; set; }
        public bool coeficiente { get; set; }
        public decimal valor { get; set; }
    }

    public class AllocationCostPeriodProductionExpense
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string descripction { get; set; }
        public int id_productionCostType { get; set; }
        public int id_processPlant { get; set; }
        public bool coeficiente { get; set; }
        public decimal value { get; set; }
    }
}