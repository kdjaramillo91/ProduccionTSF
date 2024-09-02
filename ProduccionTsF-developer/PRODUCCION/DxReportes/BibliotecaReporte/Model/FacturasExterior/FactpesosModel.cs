using System;
namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class FactpesosModel
    {
        public string Factura { get; set; }
        public DateTime FechaEmisiOn { get; set; }
        public int CartOnes { get; set; }
        public string Size { get; set; }
        public decimal BrutoKilos { get; set; }
        public decimal GlaseoKilos { get; set; }
        public decimal VFOB { get; set; }
        public decimal TSeguro { get; set; }
        public decimal TFlete { get; set; }
        public decimal TFOB { get; set; }
        public decimal Tvalortotal { get; set; }
        public decimal Kgbrutosd { get; set; }
        public decimal Kgnetos { get; set; }
        public decimal Kgglaseoneto { get; set; }
        public string Nproducto { get; set; }
        public string RazonSocialSoldTo { get; set; }
        public byte[] Logo { get; set; }
    }
}
