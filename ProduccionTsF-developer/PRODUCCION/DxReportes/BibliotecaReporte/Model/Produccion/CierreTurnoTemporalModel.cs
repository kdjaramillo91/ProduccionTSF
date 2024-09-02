namespace BibliotecaReporte.Model.Produccion
{
    internal class CierreTurnoTemporalModel
    {
        public string Turno { get; set; }
        public string Proceso { get; set; }
        public string NombreProducto { get; set; }
        public string NombreTallaProductoPrimario { get; set; }
        public decimal Cajas { get; set; }
        public decimal Libras { get; set; }
        public string NombreClientePrimario { get; set; }
        public string FechaLiquidacion { get; set; }
        public string Estado { get; set; }
    }
}