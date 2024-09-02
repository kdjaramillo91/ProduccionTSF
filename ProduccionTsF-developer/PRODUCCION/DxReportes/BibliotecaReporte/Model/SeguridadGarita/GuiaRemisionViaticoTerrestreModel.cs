using System;
namespace BibliotecaReporte.Model.SeguridadGarita
{
    internal class GuiaRemisionViaticoTerrestreModel
    {
        public byte[] Logo { get; set; }
        public string Documento { get; set; }
        public DateTime EmissionDate { get; set; }
        public decimal Viatico { get; set; }
        public string Persona { get; set; }
        public string Cipersona { get; set; }
        public string Rol { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }
        public string Description { get; set; }
        public int NumeroViaticoPersonal { get; set; }
        public string UsuarioPago { get; set; }
    }
}
