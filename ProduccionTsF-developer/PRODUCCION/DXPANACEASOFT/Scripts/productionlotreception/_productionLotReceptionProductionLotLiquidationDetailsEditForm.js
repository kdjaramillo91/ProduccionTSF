var id_salesOrderIniAux = 0;
var id_salesOrderDetailIniAux = 0;
var id_itemIniAux = 0;
var id_warehouseIniAux = 0;
var id_warehouseLocationIniAux = 0;
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
            url: "ProductionLotReception/ExistsConversionWithLbsProductionLotLiquidation",
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
                            id_salesOrderNew: id_salesOrder.GetValue(),
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
                                url: "ProductionLotReception/ItsRepeatedLiquidation",
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
                                        } else {
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

function UpdateProductionLotLiquidationDetailInfo(data, s, e) {

    //if (data.id_item === null) {
    //    return;
    //}

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText("");
    //metricUnit.SetText("");
    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
    //id_warehouse.SetValue(null);
    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");
    //id_warehouseLocation.SetValue(null);
    id_item.SetValue(data.id_item);
    id_metricUnit.SetValue(null);
    id_warehouse.SetValue(null);
    id_warehouseLocation.SetValue(null);
    //if (id_item != null) {

        $.ajax({
            url: url,//"ProductionLotReception/ItemDetailData",
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
                    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    id_warehouse.SetValue(result.id_warehouse);
                    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
                    id_warehouseLocation.SetValue(result.id_warehouseLocation);
                    //gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    //}
}

function ItemProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    
    //id_item.SetValue(data.id_item);
    id_metricUnit.ClearItems();
    id_warehouse.SetValue(null);
    id_warehouseLocation.ClearItems();

    var quantityAux = quantity.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    $.ajax({
        url: "ProductionLotReception/ItemDetailData",
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
                UpdateProductionLotLiquidationDetailMetricUnits(result.metricUnits);
                id_metricUnit.SetValue(result.id_metricUnit);

                quantityTotal.SetValue(result.quantityTotal);
                id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);

                id_warehouse.SetValue(result.id_warehouse);

                UpdateProductionLotLiquidationDetailWarehouseLocations(result.warehouseLocations);
                id_warehouseLocation.SetValue(result.id_warehouseLocation);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}


function SalesOrderProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
   
    id_item.ClearItems();

    id_metricUnit.ClearItems();
    //quantity.SetValue(0);
    quantityTotal.SetValue(0);
    id_metricUnitPresentation.SetValue(null);

    id_warehouse.SetValue(null);
    //id_warehouse.ClearItems();
    id_warehouseLocation.ClearItems();

    //id_item.PerformCallback();
    $.ajax({
        url: "ProductionLotReception/SalesOrderDetailData",
        type: "post",
        data: { id_salesOrder: s.GetValue(), id_salesOrderDetailIni: id_salesOrderDetailIniAux, id_itemIni: id_itemIniAux },
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
                //id_item.DataBind();//(result.items);
                UpdateProductionLotLiquidationDetailItems(result.items);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function SalesOrderProductionLotLiquidationDetailCombo_Init(s, e) {


    id_salesOrderIniAux = s.GetValue();
    id_salesOrderDetailIniAux = gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.cpEditingRowSalesOrderDetail;
    id_itemIniAux = id_item.GetValue();
    id_warehouseIniAux = id_warehouse.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocation.GetValue();
    var data = {
        id_salesOrder: s.GetValue(),
        //id_salesOrderDetail: gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.cpEditingRowSalesOrderDetail,
        id_item: id_item.GetValue(),
        id_metricUnit: id_metricUnit.GetValue()
    };

    $.ajax({
        url: "ProductionLotReception/InitSalesOrderItemAndMetricUnit",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_salesOrder
            var salesOrderAux = s.FindItemByValue(result.salesOrder.id);
            if (salesOrderAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
            s.SetValue(result.salesOrder.id);

            //id_item
            UpdateProductionLotLiquidationDetailItems(result.items);

            //id_metricUnit
            UpdateProductionLotLiquidationDetailMetricUnits(result.metricUnits);

            //if (result !== null && result !== undefined) {
            //    metricUnit.SetText(result.metricUnit);
            //}
            //else {
            //    metricUnit.SetText("");
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });

    //ItemProductionLotLiquidationDetailCombo_OnInit(s, e);
}

function UpdateProductionLotLiquidationDetailItems(items) {

    for (var i = 0; i < id_item.GetItemCount() ; i++) {
        var item = id_item.GetItem(i);
        var into = false;
        for (var j = 0; j < items.length; j++) {
            if (item.value == items[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_item.RemoveItem(i);
            i -= 1;
        }   
    }


    for (var i = 0; i < items.length; i++) {
        var item = id_item.FindItemByValue(items[i].id);
        var arrayStr = [];
        arrayStr.push(items[i].name);
        arrayStr.push(items[i].clase);
        arrayStr.push(items[i].size);
        if (item == null) id_item.AddItem(arrayStr, items[i].id);
    }

}

function UpdateProductionLotLiquidationDetailMetricUnits(metricUnits) {

    for (var i = 0; i < id_metricUnit.GetItemCount() ; i++) {
        var metricUnit = id_metricUnit.GetItem(i);
        var into = false;
        for (var j = 0; j < metricUnits.length; j++) {
            if (metricUnit.value == metricUnits[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_metricUnit.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < metricUnits.length; i++) {
        var metricUnit = id_metricUnit.FindItemByValue(metricUnits[i].id);
        if (metricUnit == null) id_metricUnit.AddItem(metricUnits[i].code, metricUnits[i].id);
    }

}


function ItemProductionLotLiquidationDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionLiquidationDetails",
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

function ItemLiquidationComboBox_BeginCallback(s, e) {
    e.customArgs["id_salesOrder"] = id_salesOrder.GetValue();
    e.customArgs["me"] = id_item;
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
        url: "ProductionLotReception/UpdateProductionLotReceptionWarehouseLocation",
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
        url: "ProductionLotReception/GetProductionLotReceptionWarehouseLocation",
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

function Quantity_NumberChange(s, e) {
    //if (!changed) {
    //    inicialQuantityOrdered = s.GetValue();
    //    inicialPrice = price.GetValue();
    //    changed = true;
    //}
    //console.log("changed: ");
    //console.log(changed);

    var quantityAux = s.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    UpdateQuantityTotal({
        id_item: id_item.GetValue(),
        quantity: resQuantity,
        id_metricUnit: id_metricUnit.GetValue()
    });
}

function UpdateQuantityTotal(data) {

    if (data.id_metricUnit === null) {
        quantityTotal.SetValue(0);
        id_metricUnitPresentation.SetValue(null);
        return;
    }

    $.ajax({
        url: "ProductionLotReception/UpdateQuantityTotal",
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
                quantityTotal.SetValue(result.quantityTotal);
                id_metricUnitPresentation.SetValue(result.id_metricUnitPresentation);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function MetricUnitProductionLotLiquidationDetailCombo_SelectedIndexChanged(s, e) {
    var quantityAux = quantity.GetValue();
    var strQuantity = quantityAux == null ? "0" : quantityAux.toString();
    var resQuantity = strQuantity.replace(".", ",");

    UpdateQuantityTotal({
        id_item: id_item.GetValue(),
        quantity: resQuantity,
        id_metricUnit: s.GetValue()
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

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountLiquidationDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";


    $("#lblInfoLiquidations").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsLiquidations", gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionLiquidations", gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
    }

    btnRemoveLiquidation.SetEnabled(gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountLiquidationDetails() {
    return gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.GetSelectedKeysOnPage().length;
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
    customCommand = e.command;
}

function OnGridViewLiquidationDetailsEndCallback(s, e) {
    UpdateTitlePanelLiquidationDetails();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
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
	UpdateProductionLotPerformances();
    gvProductionLotReceptionEditFormProductionLotPackingMaterialsDetail.PerformCallback();
}

function gvEditLiquidationDetailsClearSelection() {
    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.UnselectRows();
}

function gvEditLiquidationDetailsSelectAllRows() {
    gvProductionLotReceptionEditFormProductionLotLiquidationsDetail.SelectRows();
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
