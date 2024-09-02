using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class IntegrationProcess
    {

        private DBContext db = new DBContext();
        
        public Boolean isRequeridDate { get; set; }
        public List<IntegrationProcessLogView> IntegrationProcessLogViewList { get; set; }

       
    }

    public class IntegrationProcessLogView
    {
        public DateTime FechaHora { get; set; }
        public String Accion{ get; set; }
        public String Description { get; set; }
    }
}