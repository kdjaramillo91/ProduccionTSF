using System;
using System.Collections.Generic;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Models
{
    public class ReportProdModel
    {
        public string codeReport{ get; set; }
        
        public List<ParamCR> paramCRList { get; set; }

        public Conexion conex { get; set; }

        public string nameFile { get; set; }

        public string nameReport { get; set; }

        public string nameDepartment { get; set; }

        public string nameUser { get; set; }
        
    }
}