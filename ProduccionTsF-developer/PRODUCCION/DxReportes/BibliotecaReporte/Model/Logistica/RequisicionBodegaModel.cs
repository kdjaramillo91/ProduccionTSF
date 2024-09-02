using System;
namespace BibliotecaReporte.Model.Logistica
{
    internal class RequisicionBodegaModel
    {
        public string Placa { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime RG_DespachureDate { get; set; }
        public string Nameproveedor { get; set; }
        public string CIproveedor { get; set; }
        public string Conductorm { get; set; }
        public string CIconductor { get; set; }
        public string AdressConductor { get; set; }
        public string Observatrans { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Documento { get; set; }
        public string CodeItem { get; set; }
        public string NameItem { get; set; }
        public string Unidades { get; set; }
        public decimal Cantidad_1 { get; set; }
        public string Zonasitioprovedor { get; set; }
        public string Sello1 { get; set; }
        public string Sello2 { get; set; }
        public string Bodegaubicacion { get; set; }
        public string DireccionProvee { get; set; }
        public string NumeroRequisicion { get; set; }

    }
}
