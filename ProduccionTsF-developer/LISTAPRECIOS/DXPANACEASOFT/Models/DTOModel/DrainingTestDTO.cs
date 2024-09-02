using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class DrainingTestConsultDTO
    {
        public int? id_state { get; set; }
        public string initDate { get; set; }
        public string endtDate { get; set; }
        public string number { get; set; }
        public string numberLote { get; set; }
        public string reference { get; set; }
    }

    public class DrainingTestResultConsultDTO
    {
        public int id { get; set; }
        public string number { get; set; }
        public string secTransaction { get; set; }
        public DateTime dateTimeTesting { get; set; }
        public DateTime emissionDate { get; set; }
        public string numberLot { get; set; }
        public string numberRemissionGuide { get; set; }
        public string provider { get; set; }
        public decimal poundsRemitted { get; set; }
        public decimal poundsDrained { get; set; }
        public decimal poundsProjected { get; set; }
        public string analist { get; set; }
        public string state { get; set; }

        public bool canEdit { get; set; }
        public bool canAproved { get; set; }
        public bool canReverse { get; set; }
        public bool canAnnul { get; set; }
    }

    public class DrainingTestDTO
    {
        public int id { get; set; }
        public int idAnalist { get; set; }
        public string analist { get; set; }
        public int id_documentType { get; set; }
        public string documentType { get; set; }
        public string number { get; set; }
        public string reference { get; set; }
        public string description { get; set; }
        public int temp { get; set; }
        public int idSate { get; set; }
        public string state { get; set; }
        public DateTime dateTimeTesting { get; set; }
        public DateTime dateTimeEmision { get; set; }
        public int drawersNumberSampling { get; set; }
        public decimal poundsDrained { get; set; }
        public decimal poundsAverage { get; set; }
        public int poundsProjected { get; set; }
        public List<DrainingTestDetail> DrainingTestDetails { get; set; }
        public ResultProdLotReceptionDetail ReceptionDetail { get; set; }

    }

    public class DrainingTestPendingNewDTO
    {
        public int idProductionLotReceptionDetail { get; set; }
        public string number { get; set; }
        public string numberLot { get; set; }
        public string dateReception { get; set; }
        public string numberRemissionGuide { get; set; }
        public string provider { get; set; }
        public string shrimper { get; set; }
        public string item { get; set; }
        public decimal poundsRemitted { get; set; }
        public int countKavetas { get; set; }
        public string metricUnit { get; set; }
}
}