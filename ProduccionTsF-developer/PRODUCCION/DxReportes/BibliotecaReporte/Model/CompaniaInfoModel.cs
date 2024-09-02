using System;

namespace BibliotecaReporte.Model
{
    internal class CompaniaInfoModel
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string NombreComercial { get; set; }
        public Byte[] Logo { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string NumeroTelefono { get; set; }
    }
}
