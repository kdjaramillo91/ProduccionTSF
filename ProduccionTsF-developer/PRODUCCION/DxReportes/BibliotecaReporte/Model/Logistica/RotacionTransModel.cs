using System;
namespace BibliotecaReporte.Model.Logistica
{
    internal class RotacionTransModel
    {
        public string NombreCiaFactura { get; set; }
        public string NombreDueno { get; set; }
        public string Matricula { get; set; }
        public int ContadorLiqidaciones { get; set; }
        public decimal TotalValorGuia { get; set; }
        public decimal TotalValor { get; set; }
        public decimal TotalPendiente { get; set; }
        public int TieneHunter { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
    }
}