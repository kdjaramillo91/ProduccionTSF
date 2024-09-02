using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.ModelExtension
{
    internal class EntityParameters
    {

        internal string code { get; set; }
        internal string valueString { get; set; }
        internal int? valueInteger { get; set; }
        internal decimal? valueDecimal { get; set; }
        internal string valueVarchar { get; set; }
        internal DateTime? valueDate { get; set; }
    }
}