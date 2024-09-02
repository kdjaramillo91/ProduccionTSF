using System;

namespace BibliotecaReporte.Model.Recepcion
{
    internal class VitacoraProducionModel
    {
        public string NombreProveedor { get; set; }
        public string NombrePiscina { get; set; }
        public int NumeroGavetas { get; set; }
        public string NombreSitio { get; set; }
        public string NombreConductor { get; set; }
        public string NumeroInterno { get; set; }
        public string NumeroGuia { get; set; }
        public string SelloRetorno { get; set; }
        public decimal QuantityRecived { get; set; }
        public string FechaRecepcionInicio { get; set; }
        public string FechaRecepcionFin { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string ProcesoPlanta { get; set; }
        public string DireccionCia { get; set; }
    
    }
}