using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ModelExtension
{
    public class BasicAux
    {

        public int id { get; set; }
        public string description { get; set; }
    }


    public class SecurityControlInfo
    {
        public Boolean  isReadOnly { get; set; }
        public Boolean isRequired { get; set; }
    }

    public class BasicAuxValida
    {

        public int id { get; set; }
        public bool  valida { get; set; }
    }
}