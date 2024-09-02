var id_itemIniAux = 0
var id_warehouseIniAux = 0
var id_warehouseLocationIniAux = 0
//Validations
function OnItemProductionLotLossDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_item: s.GetValue()
        };
        $.ajax({
            url: "ProductionLotProcess/ExistsConversionWithLbs",
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
                        e.errorText = "Unidad medida del Item no tiene factor de conversión configurado con " + result.metricUnitName + " cuyo codigo se espera que sea (" + result.metricUnitCode + "). Configúrelo e intente de nuevo";
                    } else {
                        var data = {
                            id_itemNew: s.GetValue(),
                            id_warehouseNew: id_warehouse.GetValue(),
                            id_warehouseLocationNew: id_warehouseLocation.GetValue()
                        };
                        if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
                            data.id_warehouseLocationNew != id_warehouseLocationIniAux) {
                            $.ajax({
                                url: "ProductionLotProcess/ItsRepeatedLoss",
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
                        id_itemIniAux = 0
                        id_warehouseIniAux = 0
                        id_warehouseLocationIniAux = 0
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}


function OnItemProductionLotTrashDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var data = {
            id_item: s.GetValue()
        };
        $.ajax({
            url: "ProductionLotProcess/ExistsConversionWithLbs",
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
                        e.errorText = "Unidad medida del Item no tiene factor de conversión configurado con " + result.metricUnitName + " cuyo codigo se espera que sea (" + result.metricUnitCode + "). Configúrelo e intente de nuevo";
                    } else {
                        var data = {
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
                                url: "ProductionLotProcess/ItsRepeatedTrash",
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
                        id_itemIniAux = 0
                        id_warehouseIniAux = 0
                        id_warehouseLocationIniAux = 0
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


// EDITOR'S EVENTS

function UpdateProductionLotTrashDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    metricUnit.SetText("");
    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("metricUnit").SetText("");
    id_warehouse.SetValue(null);
    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    id_warehouseLocation.SetValue(null);
    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");

    if (id_item != null) {

        $.ajax({
            url: "ProductionLotProcess/ItemDetailDataLiquidationAndTrash",
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
                    metricUnit.SetText(result.metricUnit);
                    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    id_warehouse.SetValue(result.id_warehouse);
                    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    UpdateProductionLotTrashDetailWarehouseLocations(result.warehouseLocations);
                    id_warehouseLocation.SetValue(result.id_warehouseLocation);
                    //gvProductionLotProcessEditFormProductionLotTrashsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemProductionLotTrashDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotTrashDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemProductionLotTrashDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotProcess/ProductionLotProcessTrashDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_item.GetItemCount() ; i++) {
                var item = id_item.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_item.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}


function ItemProductionLotTrashDetailCombo_Init(s, e) {


    id_itemIniAux = s.GetValue();
    id_warehouseIniAux = id_warehouse.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocation.GetValue();
    var data = {
        id_item: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotProcess/ProductionLotProcessItemData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            metricUnit.SetText("");
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                metricUnit.SetText(result.metricUnit);
            }
            else {
                metricUnit.SetText("");
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    ItemProductionLotTrashDetailCombo_OnInit(s, e);
}

function ItemProductionLotTrashDetailCombo_OnInit(s, e) {
    //store actual filtering method and override
    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    s.filterStrategy.FilteringOnClient = function () {
        //create a new format string for all list box columns. you could skip this bit and just set
        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
        //columns 1 and 3
        var lb = this.GetListBoxControl();
        var filterTextFormatStringItems = [];
        for (var i = 0; i < lb.columnFieldNames.length; i++) {
            filterTextFormatStringItems.push('{' + i + '}');
        }
        var filterTextFormatString = filterTextFormatStringItems.join(' ');

        //store actual format string and override with one for all columns
        var actualTextFormatString = lb.textFormatString;
        lb.textFormatString = filterTextFormatString;

        //call actual filtering method which will now work on our temporary format string
        actualFilteringOnClient.apply(this);

        //restore original format string
        lb.textFormatString = actualTextFormatString;
    };
}


function UpdateProductionLotTrashDetailWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_warehouseLocation.GetItemCount() ; i++) {
        var warehouseLocation = id_warehouseLocation.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouseLocation.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_warehouseLocation.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_warehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}

function ComboWarehouseProductionLotTrashDetail_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();
    var data = {
        id_warehouse: s.GetValue()//,
    };

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotProcessWarehouseLocation",
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
                UpdateProductionLotTrashDetailWarehouseLocations(result.warehouseLocations);
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
        id_warehouse: id_warehouse.GetValue(),
        id_warehouseLocation: s.GetValue()//,
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotProcessWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateProductionLotTrashDetailWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateProductionLotTrashDetailWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}
function ItemProductionLotLossDetailCombo_Init(s, e) {


    id_itemIniAux = s.GetValue();
    id_warehouseIniAux = id_warehouse.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocation.GetValue();
    var data = {
        id_item: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotProcess/ProductionLotProcessItemData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            metricUnit.SetText("");
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                metricUnit.SetText(result.metricUnit);
            }
            else {
                metricUnit.SetText("");
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    ItemProductionLotLossDetailCombo_OnInit(s, e);
}
function ItemProductionLotLossDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotLossDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function UpdateProductionLotLossDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    metricUnit.SetText("");
    id_warehouse.SetValue(null);
    id_warehouseLocation.SetValue(null);

    if (id_item != null) {

        $.ajax({
            url: "ProductionLotProcess/ItemDetailDataLiquidationAndLoss",
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
                    metricUnit.SetText(result.metricUnit);
                    id_warehouse.SetValue(result.id_warehouse);
                    UpdateProductionLotLossDetailWarehouseLocations(result.warehouseLocations);
                    id_warehouseLocation.SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ComboWarehouseProductionLotLossDetail_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();
    var data = {
        id_warehouse: s.GetValue()//,
    };

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotProcessWarehouseLocation",
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
                UpdateProductionLotLossDetailWarehouseLocations(result.warehouseLocations);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ComboWarehouseLocationProductionLotLossDetail_Init(s, e) {
    var data = {
        id_warehouse: id_warehouse.GetValue(),
        id_warehouseLocation: s.GetValue()//,
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotProcessWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateProductionLotTrashDetailWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateProductionLotLossDetailWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}
function UpdateProductionLotLossDetailWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_warehouseLocation.GetItemCount(); i++) {
        var warehouseLocation = id_warehouseLocation.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouseLocation.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_warehouseLocation.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_warehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}

// EDITOR'S EVENTS

function OnGridViewTrashDetailsInit(s, e) {
    UpdateTitlePanelTrashDetails();
}

function UpdateTitlePanelTrashDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountTrashDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountTrashDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoTrashs").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsTrashs", gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0 && gvProductionLotProcessEditFormProductionLotTrashsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionTrashs", gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveTrash.SetEnabled(gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountTrashDetails() {
    return gvProductionLotProcessEditFormProductionLotTrashsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotProcessEditFormProductionLotTrashsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewTrashsDetailSelectionChanged(s, e) {
    UpdateTitlePanelTrashDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackTrashsDetail);

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
    if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
        s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    }
    //if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotProcessEditFormProductionLotTrashsDetail) {
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
    gvProductionLotProcessEditFormProductionLotTrashsDetail.UnselectRows();
}

function gvEditTrashDetailsSelectAllRows() {
    gvProductionLotProcessEditFormProductionLotTrashsDetail.SelectRows();
}

function OnGridViewLossDetailsInit(s, e) {
    UpdateTitlePanelLossDetails();
}

function UpdateTitlePanelLossDetails() {

    var selectedFilteredRowCount = GetSelectedFilteredRowCountLossDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountLossDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoLoss").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsLoss", gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedRowCount() > 0 && gvProductionLotProcessEditFormProductionLotLossDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionLoss", gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveLoss.SetEnabled(gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLossDetails() {
    return gvProductionLotProcessEditFormProductionLotLossDetail.cpFilteredRowCountWithoutPage +
        gvProductionLotProcessEditFormProductionLotLossDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewLossDetailSelectionChanged(s, e) {
    UpdateTitlePanelLossDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackLossDetail);

}

function GetSelectedFieldValuesCallbackLossDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewLossDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewLossDetailsEndCallback(s, e) {
    UpdateTitlePanelLossDetails();
    if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
        s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    }

    UpdateProductionLotTotals();

}

function gvEditLossDetailsClearSelection() {
    gvProductionLotProcessEditFormProductionLotLossDetail.UnselectRows();
}

function gvEditLossDetailsSelectAllRows() {
    gvProductionLotProcessEditFormProductionLotLossDetail.SelectRows();
}

function Quantity_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}


