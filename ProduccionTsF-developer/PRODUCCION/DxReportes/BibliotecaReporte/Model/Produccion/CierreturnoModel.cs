namespace BibliotecaReporte.Model.Produccion
{
    internal class CierreturnoModel
    {
        public string Turno { get; set; }    
        public string Lote { get; set; }
        public string Proveedor { get; set; }
        public string NumeroLoquidacion { get; set; }
        public string LiquidacionTurno { get; set; }
        public decimal Cola { get; set; }
        public decimal Entero { get; set; }
        public decimal Total { get; set; }
        public string Observacion { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Fecha { get; set; }
        public decimal Horas { get; set; }
        public decimal LibrasEnviadas { get; set; }
        public string Estado { get; set; }
        public decimal Rendimiento { get; set; }
    }
}