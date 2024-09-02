using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class IngresoCamaronStatusModel
    {
        public string PL_number { get; set; }
        public string PL_internalNumber { get; set; }
        public DateTime PL_receptionDate { get; set; }
        public decimal Lbsremitidas { get; set; }
        public string Namepool { get; set; }
        public string Nameproveedor { get; set; }
        public string RemissionGuide { get; set; }
        public string Name_cia { get; set; }
        public string Ruc_cia { get; set; }
        public string Telephone_cia { get; set; }
        public byte[] Foto { get; set; }
        public decimal Rendimiento { get; set; }
        public decimal Lbsrecibidas { get; set; }
        public decimal Gramagentero { get; set; }
        public string Camaronera { get; set; }
        public string Producto { get; set; }
        public string Proceso { get; set; }
        public string Estadolote { get; set; }
        public string ProcesoPlanta { get; set; }
        public DateTime Fi { get; set; }
        public DateTime Ff { get; set; }
    }
}