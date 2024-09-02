using System;

namespace BibliotecaReporte.Model.Logistica
{
   internal class GuiaRideModel
    {
        public byte[] Logo { get; set; }
        public string Compania { get; set; }
        public string Direccion { get; set; }
        public string Sucursal { get; set; }
        public string ContribuyenteEspecial { get; set; }
        public string Ruc { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ProveedorId { get; set; }
        public string ProveedorNombre { get; set; }
        public string NombreConductor { get; set; }
        public string IdConductor { get; set; }
        public string PlacaVehiculo { get; set; }
        public string ClaveAcceso { get; set; }
        public string Razon { get; set; }
        public string PuntoPartida { get; set; }
        public string Destino { get; set; }
        public string TipoDocumento { get; set; }
        public byte[] Logo2 { get; set; }
       
    }
}
