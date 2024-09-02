using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class MargenPorTallasModel
    {
        public string Comisionista { get; set; }
        public decimal Idestado { get; set; }
        public string Producto { get; set; }
        public int AgruparTipoProducto { get; set; }
        public string TipoProducto { get; set; }
        public int AgruparCategoriaProducto { get; set; }
        public string CategoriaProducto { get; set; }
        public int AgruparTallas { get; set; }
        public string Talla { get; set; }
        public decimal Rendimiento { get; set; }
        public decimal Valor { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal ValorRef { get; set; }
        public int PrecioRef { get; set; }
        public int Margen { get; set; }
        public string FILTROPROV { get; set; }
        public DateTime Fi { get; set; }
        public DateTime Ff { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2{ get; set; }
    }
}