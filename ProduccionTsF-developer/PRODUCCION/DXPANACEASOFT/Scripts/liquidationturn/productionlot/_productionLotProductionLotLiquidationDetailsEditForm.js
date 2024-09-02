var id_salesOrderIniAux = 0;
var id_salesOrderDetailIniAux = 0;
var id_itemIniAux = 0;
var id_warehouseIniAux = 0;
var id_warehouseLocationIniAux = 0;
var id_metricUnitIniAux = null;
//Validations

function OnItemProductionLotLiquidationDetailValidation(s, e) {
    
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_item: s.GetValue(),
            id_metricUnit: id_metricUnitPresentation.GetValue()
        };
        $.ajax({
            url: "ProductionLot/ExistsConversionWithLbsProductionLotLiquidation",
            type: "post",
            data: data,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                
                if (result !== null) {
                    if (result.metricUnitConversionValue == 0) {
                        e.isValid = false;
                        e.errorText = "Unidad medida del Item no tiene factor de conversión configurado con " + result.metricUnitName + " cuyo código se espera que sea (" + result.metricUnitCode + "). Configúrelo e intente de nuevo";
                    } else {
                        var data = {
                            id_salesOrderNew: 0,//id_salesOrder.GetValue(),
                            id_itemNew: s.GetValue(),
                            //id_itemOld: id_itemIniAux,
                            id_warehouseNew: id_warehouse.GetValue(),
                            //id_warehouseOld: id_warehouseIniAux,
                            id_warehouseLocationNew: id_warehouseLocation.GetValue()
                            //id_warehouseLocationOld: id_warehouseLocationIniAux
                        };
                        if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
                            data.id_warehouseLocationNew != id_warehouseLocationIniAux) {
                            $.ajax({
                                url: "ProductionLot/ItsRepeatedLiquidation",
                                type: "post",
                                data: data,
                                async: false,
                                cache: false,
                                error: function (error) {
                                    console.log(error);
                                },
                                beforeSend: function () {
                                    //showLoading();
                                },
                                success: function (result) {
                                    
                                    if (result !== null) {
                                        //console.log("result.itsRepeated: ");
                                        //console.log(result.itsRepeated);
                                        //console.log("result.itsRepeated == 1: ");
                                        //console.log(result.itsRepeated == 1);
                                        if (result.itsRepeated == 1) {
                                            e.isValid = false;
                                            e.errorText = result.Error;
                                        }
                                        //else {
                                        //    id_itemIniAux = 0
                                        //    id_warehouseIniAux = 0
                                        //    id_warehouseLocationIniAux = 0
                                        //}
                                    }
                                },
                                complete: function () {
                                    //hideLoading();
                                }
                            });
                        }
                        
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function OnWarehouseProductionLotLiquidationDetailValidation(s, e) {
    
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationProductionLotLiquidationDetailValidation(s, e) {
    
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityProductionLotLiquidationDetailValidation(s, e) {
    
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

function OnMetricUnitProductionLotLiquidationDetailValidation(s, e) {
    
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


// EDITOR'S EVENTS

function ProductionLotLiquidationDetailItem_BeginCallback(s, e) {
    
    //e.customArgs["txtItemsFilter"] = txtItemsFilter.GetText();
    e.customArgs["id_itemIni"] = id_itemIniAux;

    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var id_itemTmp = window["ItemDetail" + key].GetValue();

    e.customArgs["id_item"] = id_itemTmp;
    e.customArgs["id_salesOrder"] = 0;//id_salesOrder.GetValue();
    e.customArgs["id_salesOrderDetailIni"] = id_salesOrderDetailIniAux;
}

function OnTxtChanged(s, e) {
    
    
    if (gvProductionLotEditFormProductionLotLiquidationsDetail.IsEditing()) {
        var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
        var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
        var objItemTmp = window["ItemDetail" + key];

        objItemTmp.PerformCallback();
    }
}


function ProductionLotLiquidationDetailItem_EndCallback(s, e) {

    //id_metricUnit.PerformCallback();
}

function ItemProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    id_metricUnitIniAux = null;
    id_warehouse.SetValue(null);
    id_warehouseLocation.ClearItems();

    var quantityAux = quantity.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    $.ajax({
        url: "ProductionLot/ItemDetailData",
        type: "post",
        data: { id_item: s.GetValue(), quantity: resQuantity },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                UpdateDetailObjects(id_metricUnit, result.metricUnits, arrayFieldStr);
                //UpdateProductionLotLiquidationDetailMetricUnits(result.metricUnits);
                id_metricUnit.SetValue(result.id_metricUnit);
                //id_metricUnit.PerformCallback();

                quantityTotal.SetValue(result.quantityTotal);
                quantityPoundsLiquidation.SetValue(result.quantityLiquidationPounds);
                id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);
                id_metricUnitIniAux = result.id_metricUnit;
                id_warehouse.SetValue(result.id_warehouse);

                arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
                //UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
                id_warehouseLocation.SetValue(result.id_warehouseLocation);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}


function SalesOrderProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
   


    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var objItemTmp = window["ItemDetail" + key];

    if (objItemTmp != undefined) {
        objItemTmp.ClearItems();
    }

    
    //id_metricUnit.ClearItems();
    id_metricUnitIniAux = null;
    quantityTotal.SetValue(0);
    id_metricUnitPresentation.SetValue(null);

    id_warehouse.SetValue(null);
    id_warehouseLocation.ClearItems();

    if (objItemTmp != undefined) {
        objItemTmp.PerformCallback();
    }
}

function SalesOrderProductionLotLiquidationDetailCombo_Init(s, e) {


    id_salesOrderIniAux = s.GetValue();
    id_salesOrderDetailIniAux = gvProductionLotEditFormProductionLotLiquidationsDetail.cpEditingRowSalesOrderDetail;

    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var objItemTmp = window["ItemDetail" + key];

    id_itemIniAux = objItemTmp.GetValue();
    id_warehouseIniAux = id_warehouse.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocation.GetValue();

    var idumint = ("#settUMTP").val();
    id_metricUnitIniAux = idumint;
        //id_metricUnit.GetValue();
    var data = {
        id_salesOrder: s.GetValue(),
        id_item: objItemTmp.GetValue(),
        id_metricUnit: idumint
            //id_metricUnit.GetValue()
    };

    $.ajax({
        url: "ProductionLot/InitSalesOrderItemAndMetricUnit",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
        },
        success: function (result) {
            
            var salesOrderAux = s.FindItemByValue(result.salesOrder.id);
            if (salesOrderAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
            s.SetValue(result.salesOrder.id);

            var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
            var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
            var objItemTmp = window["ItemDetail" + key];
            objItemTmp.PerformCallback();
        },
        complete: function () {
        }
    });
}

function ComboWarehouseProductionLotLiquidationDetail_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();
    var data = {
        id_warehouse: s.GetValue()
    };

    $.ajax({
        url: "ProductionLot/UpdateProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
            }
        },
        complete: function () {
        }
    });
}

function ComboWarehouseLocationProductionLotLiquidationDetail_Init(s, e) {
    var data = {
        id_warehouse: id_warehouse.GetValue(),
        id_warehouseLocation: s.GetValue()//,
    };
    $.ajax({
        url: "ProductionLot/GetProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocation, [], arrayFieldStr);
        },
        beforeSend: function () {
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
        },
        complete: function () {
        }
    });
}

function Quantity_NumberChange(s, e) {
    
    var quantityAux = s.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var objItemTmp = window["ItemDetail" + key];

    var itmunitTmp = $("#settUMTP").val();

    UpdateQuantityTotal({
        id_item: objItemTmp.GetValue(),
        quantity: resQuantity,
        id_metricUnit: itmunitTmp
//            id_metricUnit.GetValue()
    });
}

function OnKeyPressQuantity(s, e) {
    if (e.htmlEvent.keyCode === 13) {
        //// 
        var itmunitTmp = $("#settUMTP").val();
        var quantityAux = s.GetValue();
        var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
        var resQuantity = strQuantity.replace(".", ",");

        var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
        var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
        var objItemTmp = window["ItemDetail" + key];

        UpdateQuantityTotal({
            id_item: objItemTmp.GetValue(),
            quantity: resQuantity,
            id_metricUnit: itmunitTmp
                //id_metricUnit.GetValue()
        });
        gvProductionLotEditFormProductionLotLiquidationsDetail.UpdateEdit();
    }
}

function UpdateQuantityTotal(data) {
    
    if (data.id_metricUnit === null) {
        quantityTotal.SetValue(0);
        id_metricUnitPresentation.SetValue(null);
        return;
    }

    $.ajax({
        url: "ProductionLot/UpdateQuantityTotal",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null) {
                quantityPoundsLiquidation.SetValue(result.quantityPoundsLiquidation);
                quantityTotal.SetValue(result.quantityTotal);
                id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);

            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function MetricUnitProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    var quantityAux = quantity.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var objItemTmp = window["ItemDetail" + key];

    var itmunitTmp = $("#settUMTP").val();

    UpdateQuantityTotal({
        id_item: objItemTmp.GetValue(),
        quantity: resQuantity,
        id_metricUnit: itmunitTmp
    });
}

function ProductionLotLiquidationMetricUnit_BeginCallback(s, e) {
    var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
    var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
    var objItemTmp = window["ItemDetail" + key];

    var itmunitTmp = $("#settUMTP").val();

    e.customArgs["id_metricUnit"] = itmunitTmp;
        //id_metricUnit.GetValue();
    e.customArgs["id_item"] = objItemTmp.GetValue();
}

function ProductionLotLiquidationMetricUnit_EndCallback(s, e) {
    
    id_metricUnit.SetValue(id_metricUnitIniAux);
}


// EDITOR'S EVENTS
function OnGridViewLiquidationDetailsInit(s, e) {
    UpdateTitlePanelLiquidationDetails();
}

function UpdateTitlePanelLiquidationDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCountLiquidationDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountLiquidationDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoLiquidations").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsLiquidations", gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0 && gvProductionLotEditFormProductionLotLiquidationsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionLiquidations", gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveLiquidation.SetEnabled(gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLiquidationDetails() {
    return gvProductionLotEditFormProductionLotLiquidationsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotEditFormProductionLotLiquidationsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewLiquidationsDetailSelectionChanged(s, e) {
    UpdateTitlePanelLiquidationDetails();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackLiquidationsDetail);

}

function GetSelectedFieldValuesCallbackLiquidationsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewLiquidationDetailsBeginCallback(s, e) {

    //e.customArgs["txtItemsFilter"] = txtItemsFilter.GetText();
    customCommand = e.command;
    var indexDetail = 0;
    if (e.command == "UPDATEROW" || e.command == "UPDATEEDIT") {
        if (indexDetail > 0) {
            if (window["ItemDetail" + indexDetail] != undefined) {
                e.customArgs["id_item2"] = window["ItemDetail" + indexDetail].GetValue();
            }
        } else if (indexDetail == 0) {
            var _index = gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowIndex;
            var key = _index >= 0 ? gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey : 0;
            e.customArgs["id_item2"] = window["ItemDetail" + key].GetValue();
        }
    }
}

function OnGridViewLiquidationDetailsEndCallback(s, e) {
    UpdateTitlePanelLiquidationDetails();

    UpdateProductionLotTotals();
    gvProductionLotEditFormProductionLotPackingMaterialsDetail.PerformCallback();
}

function gvEditLiquidationDetailsClearSelection() {
    gvProductionLotEditFormProductionLotLiquidationsDetail.UnselectRows();
}

function gvEditLiquidationDetailsSelectAllRows() {
    gvProductionLotEditFormProductionLotLiquidationsDetail.SelectRows();
}

//new version
var id_itemInit = null;
function ItemCombo_OnInit(s, e) {
    id_itemInit = s.GetValue();
    s.PerformCallback();
}

function OnItemDetailBeginCallback(s, e) {
    //e.customArgs["txtItemsFilter"] = txtItemsFilter.GetText();
    if (s !== undefined) {
        if (s.name.startsWith('ItemDetail')) {
            var index = parseInt(s.name.substr("ItemDetail".length));
            e.customArgs["indice"] = index;
            indexDetail = index;
        }
        e.customArgs["id_item"] = $("#ItemDetail" + gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowKey).val();
    }
}

function DetailsItemsCombo_SelectedIndexChanged(s, e) {
    id_metricUnitIniAux = null;
    //id_warehouse.SetValue(null);
    //id_warehouseLocation.ClearItems();

    var quantityAux = quantity.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    $.ajax({
        url: "ProductionLot/ItemDetailData",
        type: "post",
        data: { id_item: s.GetValue(), quantity: resQuantity },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null) {
                var arrayFieldStr = [];
                id_metricUnit.SetValue(result.id_metricUnit);
                quantityTotal.SetValue(result.quantityTotal);
                quantityPoundsLiquidation.SetValue(result.quantityLiquidationPounds);
                id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);
                id_metricUnitIniAux = result.id_metricUnit;
                //id_warehouse.SetValue(result.id_warehouse);
                arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
                //id_warehouseLocation.SetValue(result.id_warehouseLocation);
            }
        },
        complete: function () {
        }
    });
}

function OnItemDetailEndCallback(s, e) {
    s.SetValue(id_itemInit);
    //DetailsItemsCombo_SelectedIndexChanged(s, e);
    //s.PerformCallback();
}