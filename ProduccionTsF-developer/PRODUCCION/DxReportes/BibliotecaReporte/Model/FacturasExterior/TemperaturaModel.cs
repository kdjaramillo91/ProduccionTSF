using System;
namespace BibliotecaReporte.Model.FacturasExterior
{
    internal class TemperaturaModel
    {
        public string Buque { get; set; }
        public string Viaje { get; set; }
        public string Nombrecia { get; set; }
        public string CiudadPais { get; set; }
        public string ClienteExterior { get; set; }
        public string PuertoDescarga3 { get; set; }
        public int NFactura { get; set; }
        public string Contenedores { get; set; }
        public int TemperaturaInstruccion { get; set; }
        public string TipoTemperatura { get; set; }
        public string NumeroBooking { get; set; }
        public int Po { get; set; }
        public byte[] Logo { get; set; }
    }
}
