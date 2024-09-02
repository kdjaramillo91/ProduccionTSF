using System;
namespace BibliotecaReporte.Model.Produccion
{
    internal class PruebaDeEscurridoModel
    {
        public DateTime DateDraining { get; set; }
        public string NameProviderShrimp { get; set; }
        public string PoolNumber { get; set; }
        public string LotNumber { get; set; }
        public string LotSystem { get; set; }
        public decimal PoundsRemitted { get; set; }
        public int DrawersNumbers { get; set; }
        public string GuideRemission { get; set; }
        public decimal PoundsProyected { get; set; }
        public string FullNameBusisnessName { get; set; }
        public int Sampling { get; set; }
        public decimal ColumnWeight1 { get; set; }
        public decimal PoundsAverage1 { get; set; }
        public decimal ColumnWeight2 { get; set; }
        public decimal PoundsAverage2 { get; set; }
        public decimal ColumnWeight3 { get; set; }
        public decimal PoundsAverage3 { get; set; }
        public decimal ColumnWeight4 { get; set; }
        public decimal PoundsAverage4 { get; set; }
        public decimal ColumnWeight5 { get; set; }
        public decimal PoundsAverage5 { get; set; }
        public string ProcessPlant { get; set; }
    }
}