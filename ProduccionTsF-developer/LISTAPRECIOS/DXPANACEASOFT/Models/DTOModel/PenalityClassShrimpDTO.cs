using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class PenalityClassShrimpDTO
    {
        public int id { get; set; }
        public bool byProvider { get; set; }
        public bool byGroup { get; set; }
        public string nameDestination { get; set; }
        public int? id_provider { get; set; }
        public int? id_groupPersonByRol { get; set; }
        public List<PriceListPenaltyDTO> ListPriceListPenaltyDTO { get; set; }
    }
}