

namespace DXPANACEASOFT.WORKERS
{
    public partial class MonthlyBalance
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public int Periodo { get; set; }
        public int IdItem { get; set; }
        public string? NameItem { get; set; }
        public int IdWarehouse { get; set; }
        public string? NameWarehouse { get; set; }
        public int IdPresentation { get; set; }
        public string? NamePresentation { get; set; }
        public int? IdMetricUnit { get; set; }
        public string? CodeMetricUnit { get; set; }
        public string? NameMetricUnit { get; set; }
        public decimal? SaldoAnterior { get; set; }
        public decimal? Entrada { get; set; }
        public decimal? Salida { get; set; }
        public decimal? SaldoActual { get; set; }
        public decimal? Minimum { get; set; }
        public decimal? Maximum { get; set; }
        public decimal? LbSaldoAnterior { get; set; }
        public decimal? LbEntrada { get; set; }
        public decimal? LbSalida { get; set; }
        public decimal? LbSaldoActual { get; set; }
    }
}
