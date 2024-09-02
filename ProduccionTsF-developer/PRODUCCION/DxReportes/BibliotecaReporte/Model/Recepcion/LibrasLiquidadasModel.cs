using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class LibrasLiquidadasModel
    {
        public string PL_number { get; set; }
        public string PL_internalNumber { get; set; }
        public DateTime PL_receptionDate { get; set; }
        public decimal lbsprocesadas { get; set; }
        public decimal lbsremitidas { get; set; }
        public decimal lbsentero { get; set; }
        public decimal lbscola { get; set; }
        public decimal basuraentero { get; set; }
        public decimal basuracola { get; set; }
        public string NamePool { get; set; }
        public string Nameproveedor { get; set; }
        public string ProductionUnitProviderName { get; set; }
        public decimal lbsrecibidas { get; set; }
        public string name_cia { get; set; }
        public string RucCia { get; set; }
        public string telephone_cia { get; set; }
        public string proceso { get; set; }
        public int sequentialLiquidation { get; set; }
        public string ProcesoPlanta { get; set; }
        public string Fi { get; set; }
        public string Ff { get; set; }
    }
}