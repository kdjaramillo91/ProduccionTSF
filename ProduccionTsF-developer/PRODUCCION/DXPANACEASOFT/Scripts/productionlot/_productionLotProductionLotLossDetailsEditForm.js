var id_itemIniAux = 0
var id_warehouseIniAux = 0
var id_warehouseLocationIniAux = 0
//Validations

function OnItemProductionLotLossDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_item: s.GetValue(),
            id_metricUnit: id_metricUnitLoss.GetValue()
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
                            id_itemNew: s.GetValue(),
                            //id_itemOld: id_itemIniAux,
                            id_warehouseNew: id_warehouseLoss.GetValue(),
                            //id_warehouseOld: id_warehouseIniAux,
                            id_warehouseLocationNew: id_warehouseLocationLoss.GetValue()
                            //id_warehouseLocationOld: id_warehouseLocationIniAux
                        };
                        if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
                            data.id_warehouseLocationNew != id_warehouseLocationIniAux) {
                            $.ajax({
                                url: "ProductionLot/ItsRepeatedLoss",
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
                                        };
                                    }
                                },
                                complete: function () {
                                    //hideLoading();
                                }
                            });
                        }
                        //id_itemIniAux = 0
                        //id_warehouseIniAux = 0
                        //id_warehouseLocationIniAux = 0
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function OnWarehouseProductionLotLossDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationProductionLotLossDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityProductionLotLossDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

function OnMetricUnitProductionLotLossDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// EDITOR'S EVENTS

function UpdateProductionLotLossDetailInfo(data, s, e) {

    if (data.id_item === null) {
        id_metricUnitLoss.ClearItems();
        id_warehouseLoss.SetValue(null);
        id_warehouseLocationLoss.ClearItems();
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    id_metricUnitLoss.SetValue(null);
    //metricUnit.SetText("");
    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    id_warehouseLoss.SetValue(null);
    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");
    id_warehouseLocationLoss.SetValue(null);

    //if (id_item != null) {

        $.ajax({
            url: "ProductionLot/LossItemDetailData",
            type: "post",
            data: data,
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
                    UpdateDetailObjects(id_metricUnitLoss, result.metricUnits, arrayFieldStr);

                    //UpdateProductionLotLossDetailMetricUnits(result.metricUnits);
                    id_metricUnitLoss.SetValue(result.id_metricUnit);

                    id_warehouseLoss.SetValue(result.id_warehouse);

                    arrayFieldStr = [];
                    arrayFieldStr.push("name");
                    UpdateDetailObjects(id_warehouseLocationLoss, result.warehouseLocations, arrayFieldStr);
                    id_warehouseLocationLoss.SetValue(result.id_warehouseLocation);
                    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    //}
}

function ItemProductionLotLossDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotLossDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

//function ItemProductionLotLossDetailCombo_DropDown(s, e) {

//    $.ajax({
//        url: "ProductionLot/ProductionLotReceptionLossDetails",
//        type: "post",
//        data: null,
//        async: false,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//        },
//        beforeSend: function () {
//            //showLoading();
//        },
//        success: function (result) {
//            for (var i = 0; i < id_itemLoss.GetItemCount() ; i++) {
//                var item = id_itemLoss.GetItem(i);
//                if (result.indexOf(item.value) >= 0) {
//                    id_itemLoss.RemoveItem(i);
//                    i = -1;
//                }
//            }
//        },
//        complete: function () {
//            //hideLoading();
//        }
//    });
//}


function ItemProductionLotLossDetailCombo_Init(s, e) {


    id_itemIniAux = s.GetValue();
    id_warehouseIniAux = id_warehouseLoss.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocationLoss.GetValue();
    var data = {
        id_item: s.GetValue(),
        id_metricUnit: id_metricUnitLoss.GetValue()
    };

    $.ajax({
        url: "ProductionLot/InitMetricUnitsItem",//ProductionLotReceptionItemData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //metricUnit.SetText("");
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var itemAux = s.FindItemByValue(result.item.id);
                if (itemAux == null && result.item.id != null) s.AddItem(result.item.name, result.item.id);
                s.SetValue(result.item.id);
                s.DataSource = null;
                //console.log("s.DataSource: ");
                //console.log(s.DataSource);
                //s.PerformCallback();
                //metricUnit.SetText(result.metricUnit);
                //UpdateProductionLotLossDetailMetricUnits(result.metricUnits);

                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                UpdateDetailObjects(id_metricUnitLoss, result.metricUnits, arrayFieldStr);
            }
            else {
                //metricUnit.SetText("");
                id_metricUnitLoss.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    //ItemProductionLotLossDetailCombo_OnInit(s, e);
}

//function UpdateProductionLotLossDetailMetricUnits(metricUnits) {

//    for (var i = 0; i < id_metricUnitLoss.GetItemCount() ; i++) {
//        var metricUnit = id_metricUnitLoss.GetItem(i);
//        var into = false;
//        for (var j = 0; j < metricUnits.length; j++) {
//            if (metricUnit.value == metricUnits[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_metricUnitLoss.RemoveItem(i);
//            i -= 1;
//        }
//    }


//    for (var i = 0; i < metricUnits.length; i++) {
//        var metricUnit = id_metricUnitLoss.FindItemByValue(metricUnits[i].id);
//        if (metricUnit == null) id_metricUnitLoss.AddItem(metricUnits[i].code, metricUnits[i].id);
//    }

//}

//function ItemProductionLotLossDetailCombo_OnInit(s, e) {
//    //store actual filtering method and override
//    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
//    s.filterStrategy.FilteringOnClient = function () {
//        //create a new format string for all list box columns. you could skip this bit and just set
//        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
//        //columns 1 and 3
//        var lb = this.GetListBoxControl();
//        var filterTextFormatStringItems = [];
//        for (var i = 0; i < lb.columnFieldNames.length; i++) {
//            filterTextFormatStringItems.push('{' + i + '}');
//        }
//        var filterTextFormatString = filterTextFormatStringItems.join(' ');

//        //store actual format string and override with one for all columns
//        var actualTextFormatString = lb.textFormatString;
//        lb.textFormatString = filterTextFormatString;

//        //call actual filtering method which will now work on our temporary format string
//        actualFilteringOnClient.apply(this);

//        //restore original format string
//        lb.textFormatString = actualTextFormatString;
//    };
//}


//function UpdateProductionLotLossDetailWarehouseLocations(warehouseLocations) {
//    for (var i = 0; i < id_warehouseLocationLoss.GetItemCount() ; i++) {
//        var warehouseLocation = id_warehouseLocationLoss.GetItem(i);
//        var into = false;
//        for (var j = 0; j < warehouseLocations.length; j++) {
//            if (warehouseLocation.value == warehouseLocations[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_warehouseLocationLoss.RemoveItem(i);
//            i -= 1;
//        }
//    }
//    for (var i = 0; i < warehouseLocations.length; i++) {
//        var warehouseLocation = id_warehouseLocationLoss.FindItemByValue(warehouseLocations[i].id);
//        if (warehouseLocation == null) id_warehouseLocationLoss.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
//    }
//}

function ComboWarehouseProductionLotLossDetail_SelectedIndexChanged(s, e) {

    id_warehouseLocationLoss.SetValue(null);
    id_warehouseLocationLoss.ClearItems();
    var data = {
        id_warehouse: s.GetValue()//,
    };

    $.ajax({
        url: "ProductionLot/UpdateProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_warehouseLocationLoss, result.warehouseLocations, arrayFieldStr);
                //UpdateProductionLotLossDetailWarehouseLocations(result.warehouseLocations);
            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ComboWarehouseLocationProductionLotLossDetail_Init(s, e) {
    var data = {
        id_warehouse: id_warehouseLoss.GetValue(),
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
            UpdateDetailObjects(id_warehouseLocationLoss, [], arrayFieldStr);
            //UpdateProductionLotLossDetailWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocationLoss, result.warehouseLocations, arrayFieldStr);
            //UpdateProductionLotLossDetailWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS

function OnGridViewLossDetailsInit(s, e) {
    UpdateTitlePanelLossDetails();
}

function UpdateTitlePanelLossDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountLossDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotEditFormProductionLotLossDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotEditFormProductionLotLossDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountLossDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoLoss").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsLoss", gvProductionLotEditFormProductionLotLossDetail.GetSelectedRowCount() > 0 && gvProductionLotEditFormProductionLotLossDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionLoss", gvProductionLotEditFormProductionLotLossDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveLoss.SetEnabled(gvProductionLotEditFormProductionLotLossDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLossDetails() {
    return gvProductionLotEditFormProductionLotLossDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotEditFormProductionLotLossDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewLossDetailSelectionChanged(s, e) {
    UpdateTitlePanelLossDetails();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackLossDetail);

}

function GetSelectedFieldValuesCallbackLossDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewLossDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewLossDetailsEndCallback(s, e) {
    UpdateTitlePanelLossDetails();

    UpdateProductionLotTotals();

}

function gvEditLossDetailsClearSelection() {
    gvProductionLotEditFormProductionLotLossDetail.UnselectRows();
}

function gvEditLossDetailsSelectAllRows() {
    gvProductionLotEditFormProductionLotLossDetail.SelectRows();
}


