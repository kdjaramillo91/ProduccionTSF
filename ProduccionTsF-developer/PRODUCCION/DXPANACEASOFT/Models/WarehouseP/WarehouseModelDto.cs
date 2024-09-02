using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Dto
{
    public class WarehouseModelDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Id_InventoryPeriodDetail { get; set; }
    }
}