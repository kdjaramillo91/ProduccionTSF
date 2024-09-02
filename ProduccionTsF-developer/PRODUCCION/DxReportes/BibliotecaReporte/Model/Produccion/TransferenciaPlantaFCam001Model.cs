using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class TransferenciaPlantaFCam001Model
    {
        public DateTime FechaEmision { get; set; }
        public string PlacaTurno { get; set; }
        public string Turno { get; set; }
        public DateTime HoraInicio { get; set; }
        public string HoraCierre { get; set; }
        public string Lote { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string EstadoDocumento { get; set; }
        public string Descripcion { get; set; }
        public string CodigoItem { get; set; }
        public string NombreItem { get; set; }
        public string Marc { get; set; }
        public string Peso { get; set; }
        public string Cliente { get; set; }
        public string Talla { get; set; }
        public string NumeroCarro { get; set; }
        public int NumeroCajas { get; set; }
        public byte [] Logo { get; set; }

    }
}
