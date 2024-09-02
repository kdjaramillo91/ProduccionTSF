namespace BibliotecaReporte.Model.Procesos
{
    internal class ResumenReprocesoModel
    {
        public string LineaInventario { get; set; }
        public string Proceso { get; set; }
        public string TipoProducto { get; set; }
        public int G1 { get; set; }
        public int G2 { get; set; }
        public decimal LibrasEgreso { get; set; }
        public decimal LibrasReproceso { get; set; }
        public decimal LibrasBajas { get; set; }
        public decimal Porcentaje { get; set; }
        public int G3 { get; set; }
        public string Fi { get; set; }
        public string Ff { get; set; }
    }
}