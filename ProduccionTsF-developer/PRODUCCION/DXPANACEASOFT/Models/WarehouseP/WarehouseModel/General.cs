
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.WarehouseP.WarehouseModel
{
    public class WarehouseModelP
    {
        public int idWarehouseModelP { get; set; }
        public string nameWarehouseModelP { get; set; }
        public string descWarehouseModelP { get; set; }

        public int idWarehouseTypeModelP { get; set; }

        public int idInventoryLineModelP { get; set; }
        public bool isActive { get; set; }
    }
    public class WarehouseLocationModelP
    {
        public int idWarehouseLocationModelP { get; set; }

        public int idWarehouseModelP { get; set; }

        public string codeWarehouseLocationModelP { get; set; }

        public string nameWarehouseLocationModelP { get; set; }
    }
}