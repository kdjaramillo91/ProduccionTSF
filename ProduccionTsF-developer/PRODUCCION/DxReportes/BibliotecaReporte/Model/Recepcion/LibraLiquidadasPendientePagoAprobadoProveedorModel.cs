using System;

namespace BibliotecaReporte.Model.Produccion
{
    internal class LibraLiquidadasPendientePagoAprobadoProveedorModel
    {
        public string PL_internalNumber { get; set; }
        public DateTime PL_receptionDate { get; set; }
        public DateTime PL_liquidationDate { get; set; }
        public decimal Lbsprocesadas { get; set; }
        public decimal Lbsremitidas { get; set; }
        public decimal PL_totalToPay { get; set; }
        public string Nameproveedor { get; set; }
        public string ProductionUnitProviderName { get; set; }
        public decimal Lbsrecibidas { get; set; }
        public string Ruc_cia { get; set; }
        public string Telephone_cia { get; set; }
        public byte[] Foto2 { get; set; }
        public string Proceso { get; set; }
        public string Pricelist { get; set; }
        public int SequentialLiquidation { get; set; }
        public string Camaronera { get; set; }
        public decimal Anticipo { get; set; }
        public decimal Gramage { get; set; }
        public string Estado { get; set; }
        public string ProcesoPlanta { get; set; }
        public string Fi { get; set; }
        public string Ff { get; set; }
    }
}