using System;
namespace BibliotecaReporte.Model.Recepcion
{
    internal class LibrasLiquidadasPendientesPagoPAprobadoProveedorModel
    {
        public string PL_InternalNumber { get; set; }
        public DateTime PL_ReceptionDate { get; set; }
        public DateTime PL_LiquidationDate { get; set; }
        public decimal LbsProcesadas { get; set; }
        public decimal LbsRemitidas { get; set; }
        public decimal PL_TotalToPay { get; set; }
        public string NameProveedor { get; set; }
        public string ProductionUnitProviderName { get; set; }
        public decimal LbsRecibidas { get; set; }
        public string Ruc_cia { get; set; }
        public string Telephone_cia { get; set; }
        public byte[] Foto2 { get; set; }
        public string Proceso { get; set; }
        public string PriceList { get; set; }
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
