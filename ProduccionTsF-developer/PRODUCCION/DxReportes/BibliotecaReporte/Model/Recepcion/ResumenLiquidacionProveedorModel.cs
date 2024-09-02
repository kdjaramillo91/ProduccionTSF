namespace BibliotecaReporte.Model.Recepcion
{
    internal class ResumenLiquidacionProveedorModel
    {
        public string Proveedor { get; set; }
        public decimal LibrasDespachadas { get; set; }
        public decimal KilosCabeza { get; set; }
        public decimal LibrasCola { get; set; }
        public decimal RendmimientoCabeza { get; set; }
        public decimal RendimientoCola { get; set; }
        public decimal ValorCabeza { get; set; }
        public decimal ValorCola { get; set; }
        public decimal DolaresTotal { get; set; }
        public decimal PrecioCabeza { get; set; }
        public decimal PrecioCola { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}