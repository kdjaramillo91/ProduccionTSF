namespace DXPANACEASOFT.WORKERS.Models
{
    public class MonthlyBalanceProcessMessageDto
    {
        public int? anio { get; set; }
        public int? mes { get; set; }
        public string? idProcess { get; set; }
        public int idUser { get; set; }
        public bool isMassive { get; set; }
        public int? idWarehouse { get; set; }
        public int? idItem { get; set; }
    }
}
