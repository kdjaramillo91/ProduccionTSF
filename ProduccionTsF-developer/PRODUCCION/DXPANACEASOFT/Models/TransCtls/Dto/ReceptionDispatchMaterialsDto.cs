using DXPANACEASOFT.Models.DTOModel;
using System.Collections.Generic;

namespace DXPANACEASOFT.Models.Dto
{
    public class ReceptionDispatchMaterialsDto
    {
        public int id { get; set; }
        public int id_remissionGuide { get; set; }

        public DocumentDTO Document { get; set; }
        
        public RemissionGuideDto RemissionGuide { get; set; }
        public ReceptionDispatchMaterialsDetailDto[] ReceptionDispatchMaterialsDetail { get; set; }
    }

    public class ReceptionDispatchMaterialsDetailDto
    {
        public int id { get; set; }
        public int id_item { get; set; }
        public int id_receptionDispatchMaterials { get; set; }
        public decimal arrivalDestinationQuantity { get; set; }
        public decimal arrivalGoodCondition { get; set; }
        public decimal arrivalBadCondition { get; set; }
        public int id_warehouse { get; set; }
        public int id_warehouseLocation { get; set; }
        public decimal sendedAdjustmentQuantity { get; set; }
        public decimal stealQuantity { get; set; }
        public decimal transferQuantity { get; set; }
    }
}