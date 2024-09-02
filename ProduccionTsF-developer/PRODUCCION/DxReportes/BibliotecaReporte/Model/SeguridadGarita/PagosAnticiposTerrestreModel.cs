using System;

namespace BibliotecaReporte.Model.SeguridadGarita
{
    internal class PagosAnticiposTerrestreModel
    {
        public byte[] Logo { get; set; }
        public string Documento { get; set; }
        public DateTime EmissionDate { get; set; }
        public decimal Viatico { get; set; }
        public string Chofer { get; set; }
        public string Transportista { get; set; }
        public string CipersonaTransportista { get; set; }
        public string Placa { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }
        public string Description { get; set; }
        public int NumeroAnticipo { get; set; }
        public string UsuarioPago { get; set; }
    }
}