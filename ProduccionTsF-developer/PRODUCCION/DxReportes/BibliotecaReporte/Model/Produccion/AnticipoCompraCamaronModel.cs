using System;

namespace BibliotecaReporte.Model.Produccion
{
   internal class AnticipoCompraCamaronModel
    {
        public string NombreCompania { get; set;}
        public string RucCompania { get; set;}
        public byte[] LogoCompania { get; set;}
        public string NumeroCompania { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaEmision { get; set; }
        public string LoteInterno { get; set; }
        public decimal LibrasRecibidas { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal ValorAnticipo { get; set; }
        public string EstadoAnticipo { get; set; }
        public string NombreProveedor { get; set; }
        public int Secuencia { get; set; }
        public string ProcesoPlanta { get; set; }
    }
}
