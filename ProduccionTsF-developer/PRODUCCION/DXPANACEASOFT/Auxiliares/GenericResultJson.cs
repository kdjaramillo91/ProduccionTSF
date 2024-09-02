using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
    public class GenericResultJson
    {

        public string message { get; set; }
        public int codeReturn{ get; set; }

        public List<ActionAccess> ActionAccessList { get; set; }
        public List<ValueData> ValueDataList { get; set; }

    }


    public class ActionAccess
    {
        public string CodeObject { get; set; }
        public Boolean Enabled { get; set; }


    }

    public class ValueData
    {

        public string CodeObject { get; set; }
        public object valueObject { get; set; }
    }
}