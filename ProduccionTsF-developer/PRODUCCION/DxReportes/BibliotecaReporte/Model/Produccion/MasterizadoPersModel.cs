using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class MasterizadoPersModel
    {
        public DateTime FECHAEMISION { get; set; }
        public string NDOCUMENTO { get; set; }
        public string Descripcion { get; set; }
        public string TALLA1 { get; set; }
        public string MARCA1 { get; set; }
        public string BODEGA1 { get; set; }
        public string BODEGA2 { get; set; }
        public decimal Cantidad2 { get; set; }
        public decimal Cantidad3 { get; set; }
        public string CLIENTEPers { get; set; }
        public string PRESENTACION2 { get; set; }
        public decimal Lotepers { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Lote1 { get; set; }
    }
}