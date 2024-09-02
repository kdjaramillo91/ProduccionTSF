
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.RemGuide
{
    public class AnswerRGService
    {
        public int id { get; set; }

        public DBContext dbTmp { get; set; }
    }
    public class ParamRGRequestWarehouse
    {
        public int id_RemissionGuide { get; set; }

        public DBContext dbTmp { get; set; }

        public RemissionGuide rgTmp { get; set; }
    }
    public class ObjItemDispMaterial
    {
        public int? id_ReqHeader { get; set; }
        public int? id_ReqDetail { get; set; }
        public int? id_warehouse { get; set; }
        public int? id_warehouseLocation { get; set; }
        public int id_item { get; set; }
        public decimal quantityDispatch { get; set; }
        public bool isActive { get; set; }
    }
    public class ObjReqInventory
    {
        public int id_requestInventoryMove { get; set; }
    }
}