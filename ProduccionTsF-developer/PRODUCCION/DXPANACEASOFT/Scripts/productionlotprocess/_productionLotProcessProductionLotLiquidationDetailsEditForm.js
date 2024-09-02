var id_itemIniAux = 0
var id_warehouseIniAux = 0
var id_warehouseLocationIniAux = 0
//Validations

function OnItemProductionLotLiquidationDetailValidation(s, e) {
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
                                url: "ProductionLotProcess/ItsRepeatedLiquidation",
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


// EDITOR'S EVENTS

function UpdateProductionLotLiquidationDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    metricUnit.SetText("");
    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText("");
    id_warehouse.SetValue(null);
    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    id_warehouseLocation.SetValue(null);
    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");

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
                    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    id_warehouse.SetValue(result.id_warehouse);
                    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
                    id_warehouseLocation.SetValue(result.id_warehouseLocation);
                    //gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionLotLiquidationDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemProductionLotLiquidationDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotProcess/ProductionLotProcessLiquidationDetails",
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

function ItemProductionLotLiquidationDetailCombo_Init(s, e) {


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

    ItemProductionLotLiquidationDetailCombo_OnInit(s, e);
}

function ItemProductionLotLiquidationDetailCombo_OnInit(s, e) {
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


function UpdateProductionLotLiquidationDetailWarehouseLocations(warehouseLocations) {

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

function ComboWarehouseProductionLotLiquidationDetail_SelectedIndexChanged(s, e) {

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
                UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
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

function ComboWarehouseLocationProductionLotLiquidationDetail_Init(s, e) {
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
            UpdateProductionLotLiquidationDetailWarehouseLocations([]);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS
function OnGridViewLiquidationDetailsInit(s, e) {
    UpdateTitlePanelLiquidationDetails();
}

function UpdateTitlePanelLiquidationDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountLiquidationDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountLiquidationDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoLiquidations").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsLiquidations", gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0 && gvProductionLotProcessEditFormProductionLotLiquidationsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionLiquidations", gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveLiquidation.SetEnabled(gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLiquidationDetails() {
    return gvProductionLotProcessEditFormProductionLotLiquidationsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotProcessEditFormProductionLotLiquidationsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewLiquidationsDetailSelectionChanged(s, e) {
    UpdateTitlePanelLiquidationDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackLiquidationsDetail);

}

function GetSelectedFieldValuesCallbackLiquidationsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewLiquidationDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewLiquidationDetailsEndCallback(s, e) {
    UpdateTitlePanelLiquidationDetails();
    if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
        s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
    }
    //if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
    //    if (gv !== gvProductionLotReceptionEditFormItemsDetail) {
    //        if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //            s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
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

function gvEditLiquidationDetailsClearSelection() {
    gvProductionLotProcessEditFormProductionLotLiquidationsDetail.UnselectRows();
}

function gvEditLiquidationDetailsSelectAllRows() {
    gvProductionLotProcessEditFormProductionLotLiquidationsDetail.SelectRows();
}


function Quantity_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}


