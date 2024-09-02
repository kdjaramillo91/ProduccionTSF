using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ModelExtension
{
    public class ProductionLotQualityControlExt
    {


        public int idProductionLot { get; set; }
        public int idProductionLotDetail { get; set; }
        public int id_qualityControl
        {
            get; set;

        }
        public Boolean isConform { get; set; }
        public string statusDocument { get; set; }

        public string statusQualityControl { get; set; }
        public bool isNotCompleteQuality { get; set; }

    }

    public class ProductionLotQualityControlResumeExt
    {


        public int idProductionLot { get; set; }
        public int countDetail { get; set; }
        public int countDetailOk { get; set; }
         

    }


    

}