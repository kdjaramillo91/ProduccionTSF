using System;
namespace BibliotecaReporte.Model.Recepcion
{
   internal class CierreLiquidacionRendimientoModel
    {
        public int IdProduccion { get; set; }
        public DateTime FechaHoraRecepcion { get; set; }
        public string NumeroLote { get; set; }
        public string CodeProcessType { get; set; }
        public string NameProcessType { get; set; }
        public string NombreProducto { get; set; }
        public string TallaProducto { get; set; }
        public decimal CantidadCajas { get; set; }
        public string GuiaRemision { get; set; }
        public decimal CantidadMinimaXcajas { get; set; }
        public decimal Rendimiento { get; set; }
        public decimal Rendimiento2 { get; set; }
        public string NombreProveedor { get; set; }
        public string NombrePiscina { get; set; }
        public int SecuenciaLiquidacion { get; set; }
        public decimal LibrasProcesadas { get; set; }
        public decimal LibrasBasuras { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public decimal LibrasNetas { get; set; }
        public decimal PorcentajeRendimiento { get; set; }
        public string PorcentajeRendimiento2 { get; set; }
        public string LibrasDespachadas { get; set; }
        public decimal LibrasRechazo { get; set; }
        public int IdCompany { get; set; }
        public string Camaronera { get; set; }
        public string Estado { get; set; }
        public DateTime FechaProceso { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public int Metricunit { get; set; }
        public string TipoProces { get; set; }
    }
}
