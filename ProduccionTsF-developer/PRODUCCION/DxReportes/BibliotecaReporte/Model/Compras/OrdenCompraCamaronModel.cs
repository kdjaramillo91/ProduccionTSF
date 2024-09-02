using System;

namespace BibliotecaReporte.Model.Compras
{
    internal class OrdenCompraCamaronModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Valor { get; set; }
        public DateTime Fecha { get; set; }
        public string N { get; set; }
        public string Proveedor { get; set; }
        public string Unidad_de_produccion { get; set; }
        public string Via_de_embarque { get; set; }
        public string Plazo_pago { get; set; }
        public string Forma_pago { get; set; }
        public string Comprador { get; set; }
        public string Observacion { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Cantidadletras { get; set; }
        public string ProcessPlant { get; set; }
        public decimal Suma_total { get; set; }
       
    }
}