using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class RecepcionModel
    {
        public int PL_id { get; set; }

        public string PL_number { get; set; }
        public string PL_internalNumber { get; set; }
        public DateTime PL_recepctionDate { get; set; }
        public decimal lbsremitidas { get; set; }
        public string NamePool { get; set; }
        public string Nameproveedor { get; set; }
        public string ProductionUnitProviderName { get; set; }
        public string RemissionGuide { get; set; }
        public string name_cia { get; set; }
        public string ruc_cia { get; set; }
        public string telephone_cia { get; set; }
        public decimal lbsrecibidas { get; set; }
        public string proceso { get; set; }
        public decimal lbsrecibent { get; set; }
        public decimal lbsrecibcola { get; set; }
        public decimal gramagentero { get; set; }
        public decimal gramagcola { get; set; }
        public decimal rentero { get; set; }
        public decimal rentcola { get; set; }
        public string Inp { get; set; }
        public string AcuerdoTramite { get; set; }
        public string ProcesoPlanta { get; set; }
    
        public string Fi { get; set; }
        public string Ff { get; set; }
    }
}