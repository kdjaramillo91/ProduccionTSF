using System;
namespace BibliotecaReporte.Model.Procesos
{
    internal class DetalleDeReprocesoModel
    {
        public int Agr1Proceso { get; set; }
        public string PRProceso { get; set; }
        public int Agr2Prproc { get; set; }
        public string LineaInv { get; set; }
        public string ITEMEGRESO { get; set; }
        public string ITEMINGRESO { get; set; }
        public decimal LibrasIngreso { get; set; }
        public decimal LbrxBajas { get; set; }
        public decimal Rend { get; set; }
        public int Agr3Tipo { get; set; }
        public string Tipo { get; set; }
        public decimal RESULT { get; set; }
        public decimal Lbsrecibidas { get; set; }
        public DateTime Fi  { get; set; }
        public DateTime FF { get; set; }
        public string NombreCia { get; set; }
        public string RucCia { get; set; }
        public string DireccionCia { get; set; }
    }
}