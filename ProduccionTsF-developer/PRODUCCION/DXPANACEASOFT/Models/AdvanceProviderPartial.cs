using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DXPANACEASOFT.Models
{
    public partial class AdvanceProvider
    {
        public string namePriList { get; set; }
        
        public List<AdvanceProviderDTO.AdvanceProviderPLRG> aplRg { get; set; }

    }
}