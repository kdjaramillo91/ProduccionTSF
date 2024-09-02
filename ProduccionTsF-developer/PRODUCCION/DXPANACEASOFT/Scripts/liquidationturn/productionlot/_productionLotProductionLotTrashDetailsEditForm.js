var id_itemIniAux = 0
var id_warehouseIniAux = 0
var id_warehouseLocationIniAux = 0
//Validations

function OnItemProductionLotTrashDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_item: s.GetValue(),
            id_metricUnit: id_metricUnitTrash.GetValue()
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
                            id_warehouseNew: id_warehouseTrash.GetValue(),
                            //id_warehouseOld: id_warehouseIniAux,
                            id_warehouseLocationNew: id_warehouseLocationTrash.GetValue()
                            //id_warehouseLocationOld: id_warehouseLocationIniAux
                        };
                        if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
                            data.id_warehouseLocationNew != id_warehouseLocationIniAux) {
                            $.ajax({
                                url: "ProductionLot/ItsRepeatedTrash",
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

function OnWarehouseProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

function OnMetricUnitProductionLotTrashDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// EDITOR'S EVENTS

function UpdateProductionLotTrashDetailInfo(data, s, e) {

    if (data.id_item === null) {
        id_metricUnitTrash.ClearItems();
        id_warehouseTrash.SetValue(null);
        id_warehouseLocationTrash.ClearItems();
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    id_metricUnitTrash.SetValue(null);
    //metricUnit.SetText("");
    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    id_warehouseTrash.SetValue(null);
    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");
    id_warehouseLocationTrash.SetValue(null);

    //if (id_item != null) {

        $.ajax({
            url: "ProductionLot/TrashItemDetailData",
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
                    UpdateDetailObjects(id_metricUnitTrash, result.metricUnits, arrayFieldStr);

                    //UpdateProductionLotTrashDetailMetricUnits(result.metricUnits);
                    id_metricUnitTrash.SetValue(result.id_metricUnit);

                    id_warehouseTrash.SetValue(result.id_warehouse);

                    arrayFieldStr = [];
                    arrayFieldStr.push("name");
                    UpdateDetailObjects(id_warehouseLocationTrash, result.warehouseLocations, arrayFieldStr);
                    id_warehouseLocationTrash.SetValue(result.id_warehouseLocation);
                    //gvProductionLotEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    //}
}

function ItemProductionLotTrashDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotTrashDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

//function ItemProductionLotTrashDetailCombo_DropDown(s, e) {

//    $.ajax({
//        url: "ProductionLot/ProductionLotReceptionTrashDetails",
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
//            for (var i = 0; i < id_itemTrash.GetItemCount() ; i++) {
//                var item = id_itemTrash.GetItem(i);
//                if (result.indexOf(item.value) >= 0) {
//                    id_itemTrash.RemoveItem(i);
//                    i = -1;
//                }
//            }
//        },
//        complete: function () {
//            //hideLoading();
//        }
//    });
//}


function ItemProductionLotTrashDetailCombo_Init(s, e) {


    id_itemIniAux = s.GetValue();
    id_warehouseIniAux = id_warehouseTrash.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocationTrash.GetValue();
    var data = {
        id_item: s.GetValue(),
        id_metricUnit: id_metricUnitTrash.GetValue()
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
                //UpdateProductionLotTrashDetailMetricUnits(result.metricUnits);

                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                UpdateDetailObjects(id_metricUnitTrash, result.metricUnits, arrayFieldStr);
            }
            else {
                //metricUnit.SetText("");
                id_metricUnitTrash.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    //ItemProductionLotTrashDetailCombo_OnInit(s, e);
}

//function UpdateProductionLotTrashDetailMetricUnits(metricUnits) {

//    for (var i = 0; i < id_metricUnitTrash.GetItemCount() ; i++) {
//        var metricUnit = id_metricUnitTrash.GetItem(i);
//        var into = false;
//        for (var j = 0; j < metricUnits.length; j++) {
//            if (metricUnit.value == metricUnits[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_metricUnitTrash.RemoveItem(i);
//            i -= 1;
//        }
//    }


//    for (var i = 0; i < metricUnits.length; i++) {
//        var metricUnit = id_metricUnitTrash.FindItemByValue(metricUnits[i].id);
//        if (metricUnit == null) id_metricUnitTrash.AddItem(metricUnits[i].code, metricUnits[i].id);
//    }

//}

//function ItemProductionLotTrashDetailCombo_OnInit(s, e) {
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


//function UpdateProductionLotTrashDetailWarehouseLocations(warehouseLocations) {
//    for (var i = 0; i < id_warehouseLocationTrash.GetItemCount() ; i++) {
//        var warehouseLocation = id_warehouseLocationTrash.GetItem(i);
//        var into = false;
//        for (var j = 0; j < warehouseLocations.length; j++) {
//            if (warehouseLocation.value == warehouseLocations[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_warehouseLocationTrash.RemoveItem(i);
//            i -= 1;
//        }
//    }
//    for (var i = 0; i < warehouseLocations.length; i++) {
//        var warehouseLocation = id_warehouseLocationTrash.FindItemByValue(warehouseLocations[i].id);
//        if (warehouseLocation == null) id_warehouseLocationTrash.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
//    }
//}

function ComboWarehouseProductionLotTrashDetail_SelectedIndexChanged(s, e) {

    id_warehouseLocationTrash.SetValue(null);
    id_warehouseLocationTrash.ClearItems();
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
                UpdateDetailObjects(id_warehouseLocationTrash, result.warehouseLocations, arrayFieldStr);
                //UpdateProductionLotTrashDetailWarehouseLocations(result.warehouseLocations);
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

function ComboWarehouseLocationProductionLotTrashDetail_Init(s, e) {
    var data = {
        id_warehouse: id_warehouseTrash.GetValue(),
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
            UpdateDetailObjects(id_warehouseLocationTrash, [], arrayFieldStr);
            //UpdateProductionLotTrashDetailWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_warehouseLocationTrash, result.warehouseLocations, arrayFieldStr);
            //UpdateProductionLotTrashDetailWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS

function OnGridViewTrashDetailsInit(s, e) {
    UpdateTitlePanelTrashDetails();
}

function UpdateTitlePanelTrashDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountTrashDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountTrashDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoTrashs").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsTrashs", gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0 && gvProductionLotEditFormProductionLotTrashsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionTrashs", gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveTrash.SetEnabled(gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountTrashDetails() {
    return gvProductionLotEditFormProductionLotTrashsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotEditFormProductionLotTrashsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewTrashsDetailSelectionChanged(s, e) {
    UpdateTitlePanelTrashDetails();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackTrashsDetail);

}

function GetSelectedFieldValuesCallbackTrashsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewTrashDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewTrashDetailsEndCallback(s, e) {
    UpdateTitlePanelTrashDetails();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    //if (gv !== gvProductionLotEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotEditFormProductionLotTrashsDetail) {
    //        if (s.GetEditor("id_Trash") !== null && s.GetEditor("id_Trash") !== undefined) {
    //            s.GetEditor("id_Trash").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    } else {
    //        if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
    //            s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    //        }
    //    }

    //} else {
    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
    //    }
    //}

    UpdateProductionLotTotals();

}

function gvEditTrashDetailsClearSelection() {
    gvProductionLotEditFormProductionLotTrashsDetail.UnselectRows();
}

function gvEditTrashDetailsSelectAllRows() {
    gvProductionLotEditFormProductionLotTrashsDetail.SelectRows();
}


