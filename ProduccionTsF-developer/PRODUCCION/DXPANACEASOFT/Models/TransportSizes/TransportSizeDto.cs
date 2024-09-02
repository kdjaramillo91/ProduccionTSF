using DXPANACEASOFT.Models;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models
{
    public class TransportSizeDto
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int id_poundsRange { get; set; }
        public int id_iceBagRange { get; set; }
        public bool isActive { get; set; }
        public int id_company { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }
        public int id_transportTariffType { get; set; }
        public int? binRangeMinimum { get; set; }
        public int? binRangeMaximun { get; set; }

        public IceBagRange IceBagRange { get; set; }
        public PoundsRange PoundsRange { get; set; }

        public TransportTariffType TransportTariffType { get; set; }


    }
}

    