using System;
namespace BibliotecaReporte.Model.Recepcion
{
   internal class CierreLiquidacionModel
    {
        public int IdProduccion { get; set; }
        public DateTime FechaHoraRecepcion { get; set; }
        public string NumeroLote { get; set; }
        public string CodeProcessType { get; set; }
        public string NombreProducto { get; set; }
        public string TallaProducto { get; set; }
        public decimal CantidadCajas { get; set; }
        public string GuiaRemision { get; set; }
        public decimal CantidadmaximaXcajas { get; set; }
        public string NombreProveedor { get; set; }
        public string NombrePiscina { get; set; }
        public int SecuenciaLiquidacion { get; set; }
        public decimal LibrasProcesadas { get; set; }
        public decimal LibrasBasuras { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public decimal LibrasNetas { get; set; }
        public decimal Rendsr1 { get; set; }
        public decimal Rendsr2 { get; set; }
        public decimal LibrasDespachadas { get; set; }
        public decimal LibrasRechazo { get; set; }
        public string Camaronera { get; set; }
        public DateTime FechaProceso { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string UsuarioModifica { get; set; }
        public string Codetype { get; set; }
        public string Estado { get; set; }
        public decimal CantidadMinimaXcajas{ get; set; }

    }
}
