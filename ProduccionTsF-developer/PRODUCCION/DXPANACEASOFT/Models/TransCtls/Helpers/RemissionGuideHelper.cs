using DXPANACEASOFT.Models.Dto;
using System;
using System.Linq;

namespace DXPANACEASOFT.Models.Helpers
{
    public static class RemissionGuideHelper
    {
        public static RemissionGuideDto ToDto(this RemissionGuide input)
        {
            RemissionGuideDto model = null;
            if (input == null) return model;
            model = new RemissionGuideDto
            {
                id = input.id,
                id_reciver = input.id_reciver,
                id_reason = input.id_reason,
                route = input.route,
                startAdress = input.startAdress,
                despachureDate = input.despachureDate,
                arrivalDate = input.arrivalDate,
                returnDate = input.returnDate,
                uniqueCustomDocument = input.uniqueCustomDocument,
                isExport = input.isExport,
                id_providerRemisionGuide = input.id_providerRemisionGuide,
                id_priceList = input.id_priceList,
                id_buyer = input.id_buyer,
                id_protectiveProvider = input.id_protectiveProvider,
                id_productionUnitProvider = input.id_productionUnitProvider,
                id_productionUnitProviderPool = input.id_productionUnitProviderPool,
                descriptionpurchaseorder = input.descriptionpurchaseorder,
                isInternal = input.isInternal,
                id_RemisionGuideReassignment = input.id_RemisionGuideReassignment,
                id_shippingType = input.id_shippingType,
                hasEntrancePlanctProduction = input.hasEntrancePlanctProduction,
                hasExitPlanctProduction = input.hasExitPlanctProduction,
                despachurehour = input.despachurehour,
                id_TransportTariffType = input.id_TransportTariffType,
                id_tbsysCatalogState = input.id_tbsysCatalogState,
                despachureHourDt = input.despachureHourDt,
                id_RemissionGuideType = input.id_RemissionGuideType,
                id_personProcessPlant = input.id_personProcessPlant,
                isCopackingRG = input.isCopackingRG,
                id_certification = input.id_certification,
                Guia_Externa = input.Guia_Externa,
                isManual = input.isManual,
                liquidationDocument = input.liquidationDocument,
                invoiceNumber = input.invoiceNumber,
                totalLiquidation = input.totalLiquidation,
                RemissionGuideDispatchMaterial = input.RemissionGuideDispatchMaterial.Select(r=> r.ToDto()).ToArray()
            };
            return model;
        }

        public static RemissionGuideDispatchMaterialDto ToDto(this RemissionGuideDispatchMaterial input)
        {
            RemissionGuideDispatchMaterialDto model = null;
            if (input == null) return model;
            model = new RemissionGuideDispatchMaterialDto
            {
                id = input.id,
                id_item = input.id_item,
                id_remisionGuide = input.id_remisionGuide,
                quantityRequiredForPurchase = input.quantityRequiredForPurchase,
                sourceExitQuantity = input.sourceExitQuantity,
                sendedDestinationQuantity = input.sendedDestinationQuantity,
                arrivalDestinationQuantity = input.arrivalDestinationQuantity,
                arrivalGoodCondition = input.arrivalGoodCondition,
                arrivalBadCondition = input.arrivalBadCondition,
                manual = input.manual,
                isActive = input.isActive,
                id_userCreate = input.id_userCreate,
                dateCreate = input.dateCreate,
                id_userUpdate = input.id_userUpdate,
                dateUpdate = input.dateUpdate,
                id_warehouse = input.id_warehouse,
                id_warehouselocation = input.id_warehouselocation,
                amountConsumed = input.amountConsumed,
                sendedAdjustmentQuantity = input.sendedAdjustmentQuantity,
                stealQuantity = input.stealQuantity,
                transferQuantity = input.transferQuantity
            };

            return model;
        }


        public static RemissionGuide ToModel(this RemissionGuideDto input)
        {
            RemissionGuide model = null;
            if (input == null) return model;
            model = new RemissionGuide
            {
                id = input.id,
                id_reciver = input.id_reciver,
                id_reason = input.id_reason,
                route = input.route,
                startAdress = input.startAdress,
                despachureDate = input.despachureDate,
                arrivalDate = input.arrivalDate,
                returnDate = input.returnDate,
                uniqueCustomDocument = input.uniqueCustomDocument,
                isExport = input.isExport,
                id_providerRemisionGuide = input.id_providerRemisionGuide,
                id_priceList = input.id_priceList,
                id_buyer = input.id_buyer,
                id_protectiveProvider = input.id_protectiveProvider,
                id_productionUnitProvider = input.id_productionUnitProvider,
                id_productionUnitProviderPool = input.id_productionUnitProviderPool,
                descriptionpurchaseorder = input.descriptionpurchaseorder,
                isInternal = input.isInternal,
                id_RemisionGuideReassignment = input.id_RemisionGuideReassignment,
                id_shippingType = input.id_shippingType,
                hasEntrancePlanctProduction = input.hasEntrancePlanctProduction,
                hasExitPlanctProduction = input.hasExitPlanctProduction,
                despachurehour = input.despachurehour,
                id_TransportTariffType = input.id_TransportTariffType,
                id_tbsysCatalogState = input.id_tbsysCatalogState,
                despachureHourDt = input.despachureHourDt,
                id_RemissionGuideType = input.id_RemissionGuideType,
                id_personProcessPlant = input.id_personProcessPlant,
                isCopackingRG = input.isCopackingRG,
                id_certification = input.id_certification,
                Guia_Externa = input.Guia_Externa,
                isManual = input.isManual,
                liquidationDocument = input.liquidationDocument,
                invoiceNumber = input.invoiceNumber,
                totalLiquidation = input.totalLiquidation,
                RemissionGuideDispatchMaterial = input.RemissionGuideDispatchMaterial.Select(r => r.ToModel()).ToArray()
            };
            return model;
        }

        public static RemissionGuideDispatchMaterial ToModel(this RemissionGuideDispatchMaterialDto input)
        {
            RemissionGuideDispatchMaterial model = null;
            if (input == null) return model;
            model = new RemissionGuideDispatchMaterial
            {
                id = input.id,
                id_item = input.id_item,
                id_remisionGuide = input.id_remisionGuide,
                quantityRequiredForPurchase = input.quantityRequiredForPurchase,
                sourceExitQuantity = input.sourceExitQuantity,
                sendedDestinationQuantity = input.sendedDestinationQuantity,
                arrivalDestinationQuantity = input.arrivalDestinationQuantity,
                arrivalGoodCondition = input.arrivalGoodCondition,
                arrivalBadCondition = input.arrivalBadCondition,
                manual = input.manual,
                isActive = input.isActive,
                id_userCreate = input.id_userCreate,
                dateCreate = input.dateCreate,
                id_userUpdate = input.id_userUpdate,
                dateUpdate = input.dateUpdate,
                id_warehouse = input.id_warehouse,
                id_warehouselocation = input.id_warehouselocation,
                amountConsumed = input.amountConsumed,
                sendedAdjustmentQuantity = input.sendedAdjustmentQuantity,
                stealQuantity = input.stealQuantity,
                transferQuantity = input.transferQuantity
            };

            return model;
        }
    }
}

