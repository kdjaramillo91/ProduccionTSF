using System;
namespace BibliotecaReporte.Model.Recepcion
{
    internal class LibrasLiquidadasSequencialModel
    {
        public string PL_number { get; set; }
        public string PL_InternalNumber { get; set; }
        public DateTime PL_ReceptionDate { get; set; }
        public decimal Lbsprocesadas { get; set; }
        public decimal Lbsremitidas { get; set; }
        public decimal Lbsentero { get; set; }
        public decimal Lbscola { get; set; }
        public decimal Basuraentero { get; set; }
        public decimal BasuraCola { get; set; }
        public string NamePool { get; set; }
        public string Nameproveedor { get; set; }
        public int Lbsrecibidas { get; set; }
        public string Ruc_cia { get; set; }
        public string Telephone_cia { get; set; }
        public string ProcessPlant { get; set; }
        public string Proceso { get; set; }
        public int SequentialLiquidation { get; set; }
        public string Name { get; set; }
        public string Ff { get; set; }
        public string Fi { get; set; }

    }
}
