

namespace DXPANACEASOFT.WORKERS
{
    public partial class BackgroundProcess
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string State { get; set; } = null!;
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
