var id_itemIniAux = 0;
var id_saleRequestDetailIniAux = 0;
var errorMessage = "";
var msgErrorConversion = "";

//Validations

function OnItemRequestValidation(s, e) {
    //gridMessageErrorProductionScheduleRequestDetail.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorProductionScheduleRequestDetail").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_itemNew: s.GetValue(),
            id_saleRequestDetail: id_saleRequestDetailIniAux,
        };
        if (data.id_itemNew != id_itemIniAux) {
            $.ajax({
                url: "ProductionSchedule/ItsRepeatedItemRequestDetail",
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
                            errorMessage = "- Nombre del Producto: " + result.Error;
                        }
                        //else {
                        //    id_itemIniAux = 0
                        //    id_purchaseRequestDetailIniAux = 0
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

function OnQuantityScheduleValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad: Debe ser mayor a cero.";
        } else {
            errorMessage += "</br>- Cantidad: Debe ser mayor a cero.";
        }
    }
}

function OnMetricUnitRequestDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- UM: Es obligatoria.";
        } else {
            errorMessage += "</br>- UM: Es obligatoria.";
        }
    } else if (msgErrorConversion != "") {
        e.isValid = false;
        e.errorText = msgErrorConversion;
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- UM: " + msgErrorConversion;
        } else {
            errorMessage += "</br>- UM: " + msgErrorConversion;
        }
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorProductionScheduleRequestDetail.SetText(msgErrorAux);
        $("#GridMessageErrorProductionScheduleRequestDetail").show();

    }
}

function ValidateProductionScheduleRequestDetail() {
    OnItemRequestValidation(id_itemRequest, id_itemRequest);
    OnQuantityScheduleValidation(quantitySchedule, quantitySchedule);
    OnMetricUnitRequestDetailValidation(id_metricUnitRequest, id_metricUnitRequest);
}

// EDITOR'S EVENTS
function ItemRequestCombo_OnInit(s, e) {

    id_itemIniAux = s.GetValue();
    id_saleRequestDetailIniAux = gvProductionScheduleRequestDetail.cpEditingRowSaleRequestDetail;

    

    var data = {
        id_itemCurrent: s.GetValue(),
        //quantityScheduleCurrent: resQuantitySchedule,
        id_metricUnitRequestCurrent: id_metricUnitRequest.GetValue()
    };

    if (data.id_itemCurrent != null && id_saleRequestDetailIniAux != null && id_saleRequestDetailIniAux != 0) {
        s.SetEnabled(false)
        reservedInInventory.SetEnabled(false)
    };

    $.ajax({
        url: "ProductionSchedule/ProductionScheduleItemRequestDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_itemRequest
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            arrayFieldStr.push("itemSaleInformationMetricUnitCode");
            UpdateDetailObjects(id_itemRequest, result.items, arrayFieldStr);
            ////id_salesOrder
            //var salesOrderAux = s.FindItemByValue(result.salesOrder.id);
            //if (salesOrderAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
            //s.SetValue(result.salesOrder.id);

            ////id_item
            //UpdateProductionLotLiquidationDetailItems(result.items);

            //id_metricUnitRequest
            arrayFieldStr = [];
            arrayFieldStr.push("code");
            UpdateDetailObjects(id_metricUnitRequest, result.metricUnits, arrayFieldStr);

            //if (data.id_metricUnitRequestCurrent == null) {
            //    id_metricUnitRequest.SetValue(result.id_metricUnitRequest);
            //    //quantitySchedule.SetValue(result.quantitySchedule);
            //    //quantitySale.SetValue(result.quantitySale);
            //}
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
  
}

function ItemRequestCombo_SelectedIndexChanged(s, e) {
    var quantityScheduleAux = quantitySchedule.GetValue();
    var strQuantitySchedule = quantityScheduleAux == null ? "0" : quantityScheduleAux.toString();
    var resQuantitySchedule = strQuantitySchedule.replace(".", ",");

    UpdateItemRequestInfo({
        id_itemRequest: s.GetValue(),
        quantitySchedule: resQuantitySchedule,
        id_metricUnitRequest: id_metricUnitRequest.GetValue()
    });
}

function UpdateItemRequestInfo(data) {

    if (data.id_itemRequest === null)
    {
        quantitySale.SetValue(0);
        metricUnitSale.SetText("");
        ValidateProductionScheduleRequestDetail();
        return;
    }

    quantitySale.SetValue(0);
    metricUnitSale.SetText("");
    
    ItemRequestDetailData(data);
    
}

function ItemRequestDetailData(data) {

    $.ajax({
        url: "ProductionSchedule/ItemRequestDetailData",
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
                //console.log(result);
                quantitySale.SetValue(result.quantitySale);
                metricUnitSale.SetText(result.metricUnitSale);

                quantitySchedule.SetValue(result.quantitySchedule);
                msgErrorConversion = result.msgErrorConversion;
                //metricUnit.SetText(result.ItemDetailData.um);
                //price.SetValue(result.ItemDetailData.price);//.replace(",", ".")
                //iva.SetValue(result.ItemDetailData.iva);
                //total.SetValue(result.ItemDetailData.total);

                //// UPDATE ORDER TOTAL
                //orderSubtotal.SetValue(result.OrderTotal.subtotal);
                //orderSubtotalIVA12Percent.SetValue(result.OrderTotal.subtotalIVA12Percent);
                //orderTotalIVA12.SetValue(result.OrderTotal.totalIVA12);
                //orderDiscount.SetValue(result.OrderTotal.discount);
                //orderSubtotalIVA14Percent.SetValue(result.OrderTotal.subtotalIVA14Percent);
                //orderTotalIVA14.SetValue(result.OrderTotal.totalIVA14);
                //orderTotal.SetValue(result.OrderTotal.total);
            }
        },
        complete: function () {
            //hideLoading();
            ValidateProductionScheduleRequestDetail();
        }
    });

}

function QuantitySchedule_ValueChanged(s, e) {

    var quantityScheduleAux = s.GetValue();
    var strQuantitySchedule = quantityScheduleAux == null ? "0" : quantityScheduleAux.toString();
    var resQuantitySchedule = strQuantitySchedule.replace(".", ",");

    ItemRequestDetailData({
        id_itemRequest: id_itemRequest.GetValue(),
        quantitySchedule: resQuantitySchedule,
        id_metricUnitRequest: id_metricUnitRequest.GetValue()
    });
}

function MetricUnitRequestDetailCombo_SelectedIndexChanged(s, e) {
    var quantityScheduleAux = quantitySchedule.GetValue();
    var strQuantitySchedule = quantityScheduleAux == null ? "0" : quantityScheduleAux.toString();
    var resQuantitySchedule = strQuantitySchedule.replace(".", ",");

    ItemRequestDetailData({
        id_itemRequest: id_itemRequest.GetValue(),
        quantitySchedule: resQuantitySchedule,
        id_metricUnitRequest: s.GetValue()
    });
}

// EDITOR'S EVENTS
var customCommand = "";

function OnGridViewInitRequestDetail(s, e) {
    RequestDetailsUpdateTitlePanel();
}

function OnGridViewBeginCallbackRequestDetail(s, e) {
    customCommand = e.command;
    RequestDetailsUpdateTitlePanel();
}

function OnGridViewEndCallbackRequestDetail(s, e) {
    RequestDetailsUpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    //UpdateOrderTotals();
    if (gvProductionScheduleRequestDetail.cpEditingRowRefreshRequest) {
        gvProductionScheduleInventoryReservationDetail.PerformCallback();
        gvProductionScheduleProductionOrderDetail.PerformCallback();
        gvProductionSchedulePurchaseRequestDetail.PerformCallback();
        gvProductionScheduleScheduleDetail.PerformCallback();
    }
    
}

function OnGridViewSelectionChangedRequestDetail(s, e) {
    RequestDetailsUpdateTitlePanel();

}

function RequestDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCountRequestDetail();

    var text = "Total de elementos seleccionados: <b>" + gvProductionScheduleRequestDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionScheduleRequestDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountRequestDetail();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfoRequestDetail").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRowsRequestDetail", gvProductionScheduleRequestDetail.GetSelectedRowCount() > 0 && gvProductionScheduleRequestDetail.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelectionRequestDetail", gvProductionScheduleRequestDetail.GetSelectedRowCount() > 0);
    //}

    btnRemoveProductionScheduleRequestDetail.SetEnabled(false);
    //btnRemoveDetail.SetEnabled(gvProductionScheduleRequestDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountRequestDetail() {
    return gvProductionScheduleRequestDetail.cpFilteredRowCountWithoutPage + gvProductionScheduleRequestDetail.GetSelectedKeysOnPage().length;
}

function EditSelectAllRowsRequestDetail() {
    gvProductionScheduleRequestDetail.SelectRows();
}

function EditClearSelectionRequestDetail() {
    gvProductionScheduleRequestDetail.UnselectRows();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}