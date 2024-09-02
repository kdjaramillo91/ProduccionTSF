
function ComboItem_Init(s, e) {
    var data = {
        id_item: s.GetValue()
    };
    
    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionItemData",
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
}

function ComboItem_SelectedIndexChanged(s, e) {
    
    var id_productionLotSelected = id_originLot.GetValue();
    var id_itemSelected = s.GetValue();
    var id_warehouseSelected = id_warehouse.GetValue();
    var id_warehouseLocationSelected = id_warehouseLocation.GetValue();
    var data = {
        id_originLot: id_originLotCurrent,
        id_item: id_itemCurrent,
        id_warehouse: id_warehouseCurrent,
        id_warehouseLocation: id_warehouseLocationCurrent,
        id_productionLotSelected: id_productionLotSelected,
        id_itemSelected: id_itemSelected,
        id_warehouseSelected: id_warehouseSelected,
        id_warehouseLocationSelected: id_warehouseLocationSelected
    };

    $.ajax({
        url: "ProductionLotProcess/ItemDetailData",
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
            if (result !== null && result !== undefined) {
                metricUnit.SetText(result.metricUnit);
                currentStock.SetValue(0);
                currentStockAux = 0;
                id_warehouse.ClearItems();
                id_warehouseLocation.ClearItems();
                UpdateProductionLotDetailWarehouses(result.warehouses);
            }
            else {
                metricUnit.SetText("");
                currentStock.SetValue(0);
                currentStockAux = 0;
                id_warehouse.ClearItems();
                id_warehouseLocation.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function UpdateProductionLotDetailWarehouses(warehouses) {

    for (var i = 0; i < id_warehouse.GetItemCount() ; i++) {
        var warehouse = id_warehouse.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouses.length; j++) {
            if (warehouse.value == warehouses[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouse.RemoveItem(i);
            i -= 1;
        }
    }

    
    for (var i = 0; i < warehouses.length; i++) {
        var warehouse = id_warehouse.FindItemByValue(warehouses[i].id);
        if (warehouse == null) id_warehouse.AddItem(warehouses[i].name, warehouses[i].id);
    }

}

//Validations

function OnOriginLotDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnItemDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


//var currentStockAux = 0;

function OnAdjustMoreValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Ajustar(+) debe ser mayor o igual a 0";
    }
}

function OnAdjustMoreValueChanged(s, e) {
    adjustLess.SetValue(0);
    var valueAux = quantity.GetValue() + s.GetValue();
    UpdateTotalsDetailClose(gvProductionLotReceptionEditFormClosesDetail.cpId, valueAux.toFixed(2));

}

function OnAdjustLessValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) > 0) {
        e.isValid = false;
        e.errorText = "Ajustar(-) debe ser menor o igual a 0";
    }
}

function OnAdjustLessValueChanged(s, e) {
    adjustMore.SetValue(0);
    var valueAux = quantity.GetValue() + s.GetValue();
    UpdateTotalsDetailClose(gvProductionLotReceptionEditFormClosesDetail.cpId, valueAux.toFixed(2));

}

function UpdateTotalsDetailClose(id, paramTotalUM) {
    var strTotalUM = paramTotalUM == null ? "0" : paramTotalUM.toString();
    var resTotalUM = strTotalUM.replace(".", ",");

    totalMU.SetValue(paramTotalUM);

    $.ajax({
        url: "ProductionLotReception/UpdateTotalsDetailClose",
        type: "post",
        data: { id: id, totalUM: resTotalUM },
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
                totalPounds.SetValue(result.totalPounds);
                percentPerformancePounds.SetValue(result.percentPerformancePounds);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function OnQuantityAdjustValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Ajustada debe ser mayor o igual a 0";
    }
}

// EVENTS
function OnGridViewCloseDetailsInit(s, e) {
    UpdateTitlePanelCloseDetails();
}

function UpdateTitlePanelCloseDetails() {

    var selectedFilteredRowCount = GetSelectedFilteredRowCountCloseDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormClosesDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptionEditFormClosesDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountCloseDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    
    $("#lblInfoCloses").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsCloses", gvProductionLotReceptionEditFormClosesDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormClosesDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionCloses", gvProductionLotReceptionEditFormClosesDetail.GetSelectedRowCount() > 0);
    }

}

function GetSelectedFilteredRowCountCloseDetails() {
    return gvProductionLotReceptionEditFormClosesDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotReceptionEditFormClosesDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewClosesDetailSelectionChanged(s, e) {
    UpdateTitlePanelCloseDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackClosesDetail);

}

function GetSelectedFieldValuesCallbackClosesDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewCloseDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewCloseDetailsEndCallback(s, e) {
    UpdateTitlePanelCloseDetails();

    UpdateProductionLotPerformances();
    
}

function gvEditCloseDetailsClearSelection() {
    gvProductionLotReceptionEditFormClosesDetail.UnselectRows();
}

function gvEditCloseDetailsSelectAllRows() {
    gvProductionLotReceptionEditFormClosesDetail.SelectRows();
}

// UPDATE PRODUCTION LOT Performances

function UpdateProductionLotPerformances() {
    $.ajax({
        url: "ProductionLotReception/ProductionLotPerformances",
        type: "post",
        data: null,
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
                //Datos del Entero
                totalAdjustmentWholePoundsSumary.SetValue(result.totalAdjustmentWholePounds);
                wholeSubtotalAdjustSumary.SetValue(result.wholeSubtotalAdjust);
                wholeTotalQuantityRecivedSumary.SetValue(result.wholeTotalQuantityRecivedSumary);
                wholeNetSumary.SetValue(result.wholeNetSumary);
                percentWholePerformanceSumary.SetValue(result.percentWholePerformanceSumary);

                //Datos de la Cola
                totalAdjustmentTailPoundsSumary.SetValue(result.totalAdjustmentTailPounds);
                subtotalTailAdjustSumary.SetValue(result.subtotalTailAdjust);

                tailNetSumary.SetValue(result.tailNetSumary);
                percentTailPerformanceSumary.SetValue(result.percentTailPerformanceSumary);

                //Datos Totales
                totalAdjustmentPoundsSumary.SetValue(result.totalAdjustmentPounds);
                totalQuantityLiquidationAdjustSumary.SetValue(result.totalQuantityLiquidationAdjust);
                poundsGarbageTotal.SetValue(result.poundsGarbageTotal);
                totalTotalQuantityRecivedSumary.SetValue(result.totalTotalQuantityRecivedSumary);
                totalNetSumary.SetValue(result.totalNetSumary);

                //Datos SubTotales Entero
                //totalAdjustmentWholePounds.SetValue(result.totalAdjustmentWholePounds);
                //wholeSubtotalAdjust.SetValue(result.wholeSubtotalAdjust);
                percentPerformanceWholeSubtotalAdjust.SetValue(result.percentPerformanceWholeSubtotalAdjust);

                //Datos SubTotales Cola
                //totalAdjustmentTailPounds.SetValue(result.totalAdjustmentTailPounds);
                //subtotalTailAdjust.SetValue(result.subtotalTailAdjust);
                percentPerformanceSubtotalTailAdjust.SetValue(result.percentPerformanceSubtotalTailAdjust);

                //Datos SubTotales Total
                //totalAdjustmentPounds.SetValue(result.totalAdjustmentPounds);
                //totalQuantityLiquidationAdjust.SetValue(result.totalQuantityLiquidationAdjust);
                //console.log(wholeLeftover.GetValue());

                var wholeLeftoverAux = (wholeLeftover.GetValue()).toFixed(2);
                if (wholeLeftoverAux > 0) {
                    tailTotalQuantityRecivedSumary.SetValue(wholeLeftoverAux);
                } else {
                    tailTotalQuantityRecivedSumary.SetValue(result.tailTotalQuantityRecivedSumary);
                }

                UpdateTotalWholeLeftoverSumaryAux();
                OnWholeGarbagePoundsValueChanged();

            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}



