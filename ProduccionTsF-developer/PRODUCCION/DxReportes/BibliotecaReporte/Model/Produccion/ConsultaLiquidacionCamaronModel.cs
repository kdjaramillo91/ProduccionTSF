using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class ConsultaLiquidacionCamaronModel
    {
        public string NombreCompania { get; set; }
        public string RucCompania { get; set; }
        public string NumeroCompania { get; set; }
        public string NombreProveedor { get; set; }
        public DateTime FechaLiquidacion { get; set; }
        public string NumeroLoteInterno { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public string NombreProceso { get; set; }
        public decimal TotalPagar { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string SecuenciaTransaccional { get; set; }

    }
}
