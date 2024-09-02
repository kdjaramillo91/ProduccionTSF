using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.QualityControls
{
    public class QualityControlConfigDataTypeValidateDto
    {
        public int id_qualityControlDetail { get; set; }
        public int id_qualityControlConfiguration { get; set; }
        public int id_qualityAnalysis { get; set; }
        public int id_qualityDataType { get; set; }
        public int id_qualityValidate { get; set; }
        public string valueValidate { get; set; }
        public string valueValidateMin { get; set; }
        public string valueValidateMax { get; set; }
    }
}