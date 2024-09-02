using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.Filter
{
    public class ProductionLotReceptionFilterForm : BaseFilterForm
    {
        public ProductionLotReceptionFilterForm()
        {
            //Fechas
            ListFilterType.Add(new FilterType { id = 1, type = "Date", name = "Lot.Document.emissionDate", alias = "Fecha Emisión Doc." });
            ListFilterType.Add(new FilterType { id = 2, type = "Date", name = "receptionDate", alias = "Fecha Recepción" });
            ListFilterType.Add(new FilterType { id = 3, type = "Date", name = "expirationDate", alias = "Fecha Expiración" });
            ListFilterType.Add(new FilterType { id = 4, type = "Date", name = "liquidationDate", alias = "Fecha Liquidación" });
            ListFilterType.Add(new FilterType { id = 5, type = "Date", name = "closeDate", alias = "Fecha Cierre" });
            ListFilterType.Add(new FilterType { id = 6, type = "Date", name = "liquidationPaymentDate", alias = "Fecha Pago" });

            //Textos
            ListFilterType.Add(new FilterType { id = 7, type = "Text", name = "Lot.Document.number", alias = "Doc. Número" });
            ListFilterType.Add(new FilterType { id = 8, type = "Text", name = "number", alias = "Número Lote" });
            ListFilterType.Add(new FilterType { id = 9, type = "Text", name = "internalNumber", alias = "Número Lote Interno" });
            ListFilterType.Add(new FilterType { id = 10, type = "Text", name = "description", alias = "Descripción" });
            ListFilterType.Add(new FilterType { id = 11, type = "Text", name = "reference", alias = "Reference" });

            //Número
            ListFilterType.Add(new FilterType { id = 12, type = "Number", name = "totalQuantityOrdered", alias = "Cant. Total Ordenada"});
            ListFilterType.Add(new FilterType { id = 13, type = "Number", name = "totalQuantityRemitted", alias = "Cant. Total Remitida" });
            ListFilterType.Add(new FilterType { id = 14, type = "Number", name = "totalQuantityRecived", alias = "Cant. Total Recibida" });
            ListFilterType.Add(new FilterType { id = 15, type = "Number", name = "totalQuantityLiquidation", alias = "Cant. Total Liquidada" });
            ListFilterType.Add(new FilterType { id = 16, type = "Number", name = "totalQuantityTrash", alias = "Cant. Total Basura" });
            ListFilterType.Add(new FilterType { id = 17, type = "Number", name = "totalQuantityLiquidationAdjust", alias = "Cant. Total Liquidada Ajustada" });
            ListFilterType.Add(new FilterType { id = 18, type = "Number", name = "quantityOrdered", alias = "Cant. Ordenada Materia Prima", nameDetail = "ProductionLotDetail" });
            ListFilterType.Add(new FilterType { id = 19, type = "Number", name = "quantityRemitted", alias = "Cant. Remitida Materia Prima", nameDetail = "ProductionLotDetail" });
            ListFilterType.Add(new FilterType { id = 20, type = "Number", name = "quantityRecived", alias = "Cant. Recibida Materia Prima", nameDetail = "ProductionLotDetail" });
            ListFilterType.Add(new FilterType { id = 21, type = "Number", name = "quantityProcessed", alias = "Cant. Procesada Materia Prima", nameDetail = "ProductionLotDetail" });
            ListFilterType.Add(new FilterType { id = 22, type = "Number", name = "sourceExitQuantity", alias = "Cant. Salida del Origen Material de Despacho", nameDetail = "ProductionLotDispatchMaterial" });
            ListFilterType.Add(new FilterType { id = 23, type = "Number", name = "sendedDestinationQuantity", alias = "Cant. Enviada al Destino Material de Despacho", nameDetail = "ProductionLotDispatchMaterial" });
            ListFilterType.Add(new FilterType { id = 24, type = "Number", name = "arrivalDestinationQuantity", alias = "Cant. Llegada al Destino Material de Despacho", nameDetail = "ProductionLotDispatchMaterial" });
            ListFilterType.Add(new FilterType { id = 25, type = "Number", name = "arrivalGoodCondition", alias = "Cant. Llegada Bueno al Destino Material de Despacho", nameDetail = "ProductionLotDispatchMaterial" });
            ListFilterType.Add(new FilterType { id = 26, type = "Number", name = "arrivalBadCondition", alias = "Cant. Llegada Malo al Destino Material de Despacho", nameDetail = "ProductionLotDispatchMaterial" });
            ListFilterType.Add(new FilterType { id = 27, type = "Number", name = "quantity", alias = "Cant. Unidad en el Empaque", nameDetail = "ProductionLotLiquidation" });
            ListFilterType.Add(new FilterType { id = 28, type = "Number", name = "quantityTotal", alias = "Cant. Total Unidad de Presentación", nameDetail = "ProductionLotLiquidation" });
            ListFilterType.Add(new FilterType { id = 29, type = "Number", name = "quantity", alias = "Cant. Material Empaque", nameDetail = "ProductionLotPackingMaterial" });
            ListFilterType.Add(new FilterType { id = 30, type = "Number", name = "quantity", alias = "Cant. Desperdicio", nameDetail = "ProductionLotTrash" });

            //Selección
            ListFilterType.Add(new FilterType { id = 31, type = "Select", name = "Lot.Document.id_documentState", alias = "Estado Documento", dataSource = "DataProviderDocumentState.AllDocumentStatesByCompany" });
            ListFilterType.Add(new FilterType { id = 32, type = "Select", name = "id_ProductionLotState", alias = "Estado Lote", dataSource = "DataProviderProductionLotState.AllProductionLotStatesByCompany" });
            ListFilterType.Add(new FilterType { id = 33, type = "Select", name = "id_productionUnit", alias = "Unidad de Producción", dataSource = "DataProviderProductionUnit.AllProductionUnitRECsByCompany" });
            ListFilterType.Add(new FilterType { id = 34, type = "Select", name = "id_provider", alias = "Proveedor", dataSource = "DataProviderPerson.AllProvidersByCompany" });
            ListFilterType.Add(new FilterType { id = 35, type = "Select", name = "id_priceList", alias = "Lista de Precio/Cotización", dataSource = "DataProviderPriceList.AllPriceListsForPurchaseByCompany" });
            ListFilterType.Add(new FilterType { id = 36, type = "Select", name = "id_buyer", alias = "Comprador", dataSource = "DataProviderPerson.AllBuyersByCompany" });
            ListFilterType.Add(new FilterType { id = 37, type = "Select", name = "id_personRequesting", alias = "Solicitante", dataSource = "DataProviderPerson.AllRequestingPersonsByCompany" });
            ListFilterType.Add(new FilterType { id = 38, type = "Select", name = "id_personReceiving", alias = "Recibidor", dataSource = "DataProviderPerson.AllReceivingPersonsByCompany" });
            ListFilterType.Add(new FilterType { id = 39, type = "Select", name = "id_productionUnitProviderPool", alias = "Piscina", dataSource = "DataProviderProductionUnitProvider.AllProductionUnitProviderPoolsByCompany" });
            ListFilterType.Add(new FilterType { id = 40, type = "Select", name = "id_item", alias = "Producto Materia Prima", nameDetail = "ProductionLotDetail", dataSource = "DataProviderItem.AllPurchaseItemsMPByCompany" });
            ListFilterType.Add(new FilterType { id = 41, type = "Select", name = "id_warehouse", alias = "Bodega Materia Prima", nameDetail = "ProductionLotDetail", dataSource = "DataProviderWarehouse.AllWarehousesMPByCompany" });
            ListFilterType.Add(new FilterType { id = 42, type = "Select", name = "id_warehouseLocation", alias = "Ubicación Materia Prima", nameDetail = "ProductionLotDetail", dataSource = "DataProviderWarehouseLocation.AllWarehouseLocationsMPByCompany" });
            ListFilterType.Add(new FilterType { id = 43, type = "Select", name = "id_item", alias = "Producto Material de Despacho", nameDetail = "ProductionLotDispatchMaterial", dataSource = "DataProviderItem.AllPurchaseItemsMDDByCompany" });
            ListFilterType.Add(new FilterType { id = 44, type = "Select", name = "id_warehouse", alias = "Bodega Material de Despacho", nameDetail = "ProductionLotDispatchMaterial", dataSource = "DataProviderWarehouse.AllWarehousesMIByCompany" });
            ListFilterType.Add(new FilterType { id = 45, type = "Select", name = "id_warehouseLocation", alias = "Ubicación Material de Despacho", nameDetail = "ProductionLotDispatchMaterial", dataSource = "DataProviderWarehouseLocation.AllWarehouseLocationsMIByCompany" });
            ListFilterType.Add(new FilterType { id = 46, type = "Select", name = "id_item", alias = "Producto Liquidación", nameDetail = "ProductionLotLiquidation", dataSource = "DataProviderItem.AllPurchaseItemsPPPTByCompany" });
            ListFilterType.Add(new FilterType { id = 47, type = "Select", name = "id_warehouse", alias = "Bodega Liquidación", nameDetail = "ProductionLotLiquidation", dataSource = "DataProviderWarehouse.AllWarehousesMPByCompany" });
            ListFilterType.Add(new FilterType { id = 48, type = "Select", name = "id_warehouseLocation", alias = "Ubicación Liquidación", nameDetail = "ProductionLotLiquidation", dataSource = "DataProviderWarehouseLocation.AllWarehouseLocationsMPByCompany" });
            ListFilterType.Add(new FilterType { id = 49, type = "Select", name = "id_salesOrder", alias = "Orden Pedido", nameDetail = "ProductionLotLiquidation", dataSource = "DataProviderSalesOrder.AllSalesOrderByCompany" });
            ListFilterType.Add(new FilterType { id = 50, type = "Select", name = "id_item", alias = "Producto Material Empaque", nameDetail = "ProductionLotPackingMaterial", dataSource = "DataProviderItem.AllPurchaseItemsMDEByCompany" });
            ListFilterType.Add(new FilterType { id = 51, type = "Select", name = "id_item", alias = "Producto Desperdicio", nameDetail = "ProductionLotTrash", dataSource = "DataProviderItem.AllPurchaseItemsDEByCompany" });
            ListFilterType.Add(new FilterType { id = 52, type = "Select", name = "id_warehouse", alias = "Bodega Desperdicio", nameDetail = "ProductionLotTrash", dataSource = "DataProviderWarehouse.AllWarehousesDEByCompany" });
            ListFilterType.Add(new FilterType { id = 53, type = "Select", name = "id_warehouseLocation", alias = "Ubicación Desperdicio", nameDetail = "ProductionLotTrash", dataSource = "DataProviderWarehouseLocation.AllWarehouseLocationsDEByCompany" });

            //Chequeo
            ListFilterType.Add(new FilterType { id = 54, type = "Check", name = "withPrice", alias = "Sin Lista de Precio desde la Compra" });
        }
    }
}