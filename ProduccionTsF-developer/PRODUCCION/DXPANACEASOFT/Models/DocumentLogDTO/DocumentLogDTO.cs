
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DXPANACEASOFT.Models.DocumentLogDTO
{
    public class DocumentLogDTO
    {
        public int id { get; set; }
        
        public string description { get; set; }

        public string code_Action { get; set; }

        public int id_User { get; set; }

    }
}