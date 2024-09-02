using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class HorasNoProductivasSubModel
    {
        public string Motivo { get; set; }
        public int Paradas { get; set; }
        public int Minutos { get; set; }
        public int TotalMinutos { get; set; }
        public int IdMotivo { get; set; }
        public string CodigoMotivo { get; set; }
        public string HoraMinutos { get; set; }
        public string HoraTotal { get; set; }
    }
}
