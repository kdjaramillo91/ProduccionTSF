using DevExpress.XtraCharts;
using System;

namespace DXPANACEASOFT.Models.Mastereds
{
    public class ResultMastered
    {
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string noLote { get; set; }
        public string saldoStr { get; set; }
        public int id_item { get; set; }
        public int? id_size { get; set; }
        public int id_itemType { get; set; }
        public int? id_trademark { get; set; }
        public int? id_presentationMP { get; set; }
        public int id_lote { get; set; }
        public decimal saldo { get; set; }
        public bool exitsInDetail { get; set; }
    }

}

