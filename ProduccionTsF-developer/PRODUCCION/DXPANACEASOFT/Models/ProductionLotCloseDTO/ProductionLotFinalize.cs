using System;

namespace DXPANACEASOFT.Models.ProductionLotCloseDTO
{
    public partial class ProductionLotFinalize
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroLote { get; set; }
        public string Referencia { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public string Solicitante { get; set; }

        #region Entidad para consulta detallada
        [System.Reflection.Obfuscation(Exclude = true)]
        [System.Serializable()]
        public class Detallado : ProductionLotFinalize
        {
            public bool PuedeEditarse { get; set; }
            public int[] IdsLote { get; set; }
            public DetalleLote[] DetallesLotes { get; set; }
            public ResultKardex[] DetallesKardex { get; set; }

            public Detallado()
            {
                this.IdsLote = new int[] { };
                this.DetallesLotes = new DetalleLote[] { };
                this.DetallesKardex = new ResultKardex[] { };
            }
        }
        #endregion
    }

}