using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class HorasNoProductivasModel
    {
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string EstadoDocumento { get; set; }
        public string Maquina { get; set; }
        public string Proceso { get; set; }
        public string Turno { get; set; }
        public string TotalParada { get; set; }
        public string TotalProduccion { get; set; }
        public string Total { get; set; }
        public string DProveedor { get; set; }
        public string DPiscina { get; set; }
        public int DGrammage { get; set; }
        public string DLote { get; set; }
        public int Dcc { get; set; }
        public int Dsc { get; set; }
        public int DLibrasIngresadas { get; set; }
        public string DHoraInicio { get; set; }
        public string DHoraFin { get; set; }
        public int DLibrasProcesadas { get; set; }
        public int DNumeroPersonas { get; set; }
        public string DMotivo { get; set; }
        public string DObservacion { get; set; }
        public int Cabeza { get; set; }
        public string HoraCabeza { get; set; }
        public int Cola { get; set; }
        public int Totald2 { get; set; }
        public string HoraCola { get; set; }
        public string TotalHoras { get; set; }
        public int Personas { get; set; }
        public int MinutosNoProductivo { get; set; }
        public int MinutosProductivo { get; set; }
        public int CabezaMinuto { get; set; }
        public int Colaminuto { get; set; }
        public int Totalminuto { get; set; }
        public byte[] Logo { get; set; }

    }
}
