using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class InventoryMove
    {
        public int? idWarehousePackagingMaterials { get; set; }
        public int? idWarehouseLocationPackagingMaterials { get; set; }
        public int? idLotePackagingMaterials { get; set; }
        public int? idProviderPackagingMaterials { get; set; }

        public string nameProviderPackagingMaterials { get; set; }

        public string nameProductionUnitProviderPackagingMaterials { get; set; }
        public int? idCostCenterPackagingMaterials { get; set; }
        public int? idSubCostCenterPackagingMaterials { get; set; }

        public int? idItemPackagingMaterials { get; set; }

        public string nameItemDescriptionPackagingMaterials { get; set; }

        public decimal? quantityDrainedPackagingMaterials { get; set; }


    }
}