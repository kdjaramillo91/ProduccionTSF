using System;
namespace BibliotecaReporte.Model.Logistica
{
    internal class RotacionTransFluvialModel
    {
        public string NombreCiaFactura { get; set; }
        public string NombreDueno { get; set; }
        public string Matricula { get; set; }
        public int ContadorLiqidaciones { get; set; }
        public decimal TotalValor { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
    }
}