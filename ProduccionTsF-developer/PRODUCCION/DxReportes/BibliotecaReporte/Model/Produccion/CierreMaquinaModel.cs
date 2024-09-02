using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class CierreMaquinaModel
    {
        public DateTime FechaEmission { get; set; }
        public string Maquina { get; set; }
        public string Responsable { get; set; }
        public string Turno { get; set; }
        public string Estado { get; set; }
        public int NumeroPersona { get; set; }
        public string Proceso { get; set; }
        public string NumeroLiquidacion { get; set; }
        public string Provider { get; set; }
        public string NameProviderShrimp { get; set; }
        public string ProductionUnitProviderPool { get; set; }
        public int Detailweight { get; set; }
        public string ProccesType { get; set; }
        public string NumberLote { get; set; }
        public string NumberLot { get; set; }
        public string PlantProcess { get; set; }
        public decimal PoundsRemitted { get; set; }
        public decimal PoundsProcessed { get; set; }
        public decimal PoundsCooling { get; set; }
        public int noOfBox { get; set; }
        public string Detailstate { get; set; }
        public int Cabeza { get; set; }
        public int Cola { get; set; }
        public int CabezaRefri { get; set; }
        public int ColaRefri { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }
    }
}