
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;

namespace DXPANACEASOFT.Models.RequestInventoryMoveModel
{
    public class RequestInventoryMoveModelP
    {
        public int idRIMModelP { get; set; }
        public int idPersonRModelP { get; set; }
        public int? idWarehouseModelP { get; set; }
        public int? idProviderModelP { get; set; }
        public int? idCustomerModelP { get; set; }
        public int idNatureMoveModelP { get;set;}

        public DocumentModelP documentModelP { get; set; }
        public List<RequestInventoryMoveDetailModelP> lstRequestInvDetail { get; set; }
    }
    public class RequestInventoryMoveDetailModelP
    {
        public int id { get; set; }
        public int id_item { get; set; }
        
        public decimal quantityRequest { get; set; }

        public decimal? quantityUpdate { get; set; }

        public int? id_warehouseLocation { get; set; }
        
        public bool isActive { get; set; }
    }
}