using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class LiquidacionCarroXCarroModel
    {
        public string NumeroLote { get; set; }
        public string NombreCarro { get; set; }
        public string NombreProductoPrimario { get; set; }
        public string NombreTallaProductoPrimario { get; set; }
        public decimal CajasCantidad { get; set; }
        public string Piscina { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public decimal LibrasDespachadas { get; set; }
        public decimal LibrasProcesadas { get; set; }
        public string FechaProceso { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Camaronera { get; set; }
        public string Turno { get; set; }
        public string Observacion { get; set; }
        public string SecTransaccional { get; set; }
        public string NumeroLiq { get; set; }
        public string Maquina { get; set; }
        public string Estado { get; set; }
        public string NombreLiquidador { get; set; }
        public string Proceso_Producto { get; set; }

    }
}
