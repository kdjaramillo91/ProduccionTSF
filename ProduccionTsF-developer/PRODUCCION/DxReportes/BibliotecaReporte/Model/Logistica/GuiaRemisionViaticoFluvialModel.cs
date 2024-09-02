using System;


namespace BibliotecaReporte.Model.Logistica
{
   internal class GuiaRemisionViaticoFluvialModel
    {
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
        public string Documento { get; set; }
        public DateTime EmissionDate { get; set; }
        public decimal Viatico { get; set; }
        public string Persona { get; set; }
        public string CiPersona { get; set; }
        public string Rol { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }

        public string Description { get; set; }

        public int NumeroViaticoFluvial { get; set; }
        public string UsuarioPago { get; set; }


    }
}
