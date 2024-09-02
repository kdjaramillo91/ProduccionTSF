using DevExpress.DataProcessing;
using DXPANACEASOFT.Models.Dto;
using System.Linq;
using System.Windows.Input;

namespace DXPANACEASOFT.Models.Helpers
{
    public static class ReceptionDispatchMaterialsHelper
    {
        public static ReceptionDispatchMaterialsDto ToDto(this ReceptionDispatchMaterials input)
        {
            ReceptionDispatchMaterialsDto model = null;
            if (input == null) return model;
            model = new ReceptionDispatchMaterialsDto
            {
              id = input.id,
              id_remissionGuide = input.id_remissionGuide,
              Document =input.Document.ToDto(),
              ReceptionDispatchMaterialsDetail = input.ReceptionDispatchMaterialsDetail.Select(r=> r.ToDto()).ToArray(),
              RemissionGuide = input.RemissionGuide.ToDto(),

            };

            return model;
        }

        public static ReceptionDispatchMaterialsDetailDto ToDto(this ReceptionDispatchMaterialsDetail input)
        {
            ReceptionDispatchMaterialsDetailDto model = null;
            if (input == null) return model;
            model = new ReceptionDispatchMaterialsDetailDto
            {
                id= input.id,
                id_item = input.id_item,
                id_receptionDispatchMaterials = input.id_receptionDispatchMaterials,
                arrivalDestinationQuantity = input.arrivalDestinationQuantity,
                arrivalGoodCondition = input.arrivalGoodCondition,
                arrivalBadCondition = input.arrivalBadCondition,
                id_warehouse = input.id_warehouse,
                id_warehouseLocation = input.id_warehouseLocation,
                sendedAdjustmentQuantity = input.sendedAdjustmentQuantity,
                stealQuantity = input.stealQuantity,
                transferQuantity = input.transferQuantity
            };

            return model;

        }


        public static ReceptionDispatchMaterials ToModel(this ReceptionDispatchMaterialsDto input)
        {
            ReceptionDispatchMaterials model = null;
            if (input == null) return model;
            model = new ReceptionDispatchMaterials
            {
                id = input.id,
                id_remissionGuide = input.id_remissionGuide,
                Document = input.Document.ToModel(),
                RemissionGuide = input.RemissionGuide.ToModel(),
                ReceptionDispatchMaterialsDetail = input.ReceptionDispatchMaterialsDetail.Select(r=> r.ToModel()).ToArray()
                
            };

            return model;
        }

        public static ReceptionDispatchMaterialsDetail ToModel(this ReceptionDispatchMaterialsDetailDto input)
        {
            ReceptionDispatchMaterialsDetail model = null;
            if (input == null) return model;
            model = new ReceptionDispatchMaterialsDetail
            {
                id = input.id,
                id_item = input.id_item,
                id_receptionDispatchMaterials = input.id_receptionDispatchMaterials,
                arrivalDestinationQuantity = input.arrivalDestinationQuantity,
                arrivalGoodCondition = input.arrivalGoodCondition,
                arrivalBadCondition = input.arrivalBadCondition,
                id_warehouse = input.id_warehouse,
                id_warehouseLocation = input.id_warehouseLocation,
                sendedAdjustmentQuantity = input.sendedAdjustmentQuantity,
                stealQuantity = input.stealQuantity,
                transferQuantity = input.transferQuantity
            };

            return model;

        }
    }
}