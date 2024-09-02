// DIALOG BUTTONS ACTIONS

function Update(approve) {
    var valid = true;
    var validTableTabDocument = ASPxClientEdit.ValidateEditorsInContainerById("tableTabDocument", null, true);
    var validTableTabOrder = ASPxClientEdit.ValidateEditorsInContainerById("tableTabOrder", null, true);

    if (validTableTabDocument) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validTableTabOrder) {
        UpdateTabImage({ isValid: true }, "tabOrder");
    } else {
        UpdateTabImage({ isValid: false }, "tabOrder");
        valid = false;
    }

    if (gvSalesOrderEditFormDetails.cpRowsCount === 0 || gvSalesOrderEditFormDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabDetails");
    }

    if (valid) {
        var id = $("#id_order").val();
        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditSalesOrder").serialize();
        var url = (id === "0") ? "SalesOrder/SalesOrdersPartialAddNew"
                                : "SalesOrder/SalesOrdersPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {


    Update(false);

}

function ButtonUpdateClose_Click(s, e) {

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_order").val();

        var data = "id=" + id + "&" + $("#formEditSalesOrder").serialize();

        var url = (id === "0") ? "SalesOrder/SalesOrderPartialAddNew"
                               : "SalesOrder/SalesOrderPartialUpdate";

        if (data != null) {
            $.ajax({
                url: url,
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
                    console.log(result);
                },
                complete: function () {
                    hideLoading();
                    showPage("SalesOrder/Index", null);
                }
            });
        }
    }
}

function ButtonCancel_Click(s, e) {
    showPage("SalesOrder/Index", null);
}

//SALES ORDER BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("SalesOrder/FormEditSalesOrder", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar la Orden de Producción?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("SalesOrder/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("SalesOrder/Autorize", data);
    }, "¿Desea autorizar la Orden de Producción?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("SalesOrder/Protect", data);
    }, "¿Desea cerrar la Orden de Producción?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("SalesOrder/Cancel", data);
    }, "¿Desea anular la Orden de Producción?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("SalesOrder/Revert", data);
    }, "¿Desea reversar la Orden de Producción?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    var _url = "SalesOrder/SalesOrdersReport";
    var id = $("#id_salesOrder").val();

    if (!(id == 0) && !(id == null)) {
        var ids = [id];
        $.ajax({
            url: _url,
            type: "post",
            data: { ids: ids },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#maincontent").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }

}

//SHOW/HIDE SALES ORDER IMPORTATION DETAILS

//function ShowDetailImportation(s, e) {
//    if (s.GetChecked()) {
//        $("#detailImportation").show();
//    } else {
//        $("#detailImportation").hide();
//    }
//}

// CABECERA DE SOLICITUD

function OnPriceListBeginCallback(s, e) {
    e.customArgs['id_customer'] = id_customer.GetValue();
}

function SalesOrderCustomer_SelectedIndexChanged(s, e) {
    id_priceList.PerformCallback();
}

function OnPriceList_SelectedIndexChanged(s, e) {
    var data = {
        id_priceList: s.GetValue()
    };

    $.ajax({
        url: "SalesOrder/OnPriceList_SelectedIndexChanged",
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
            gvSalesOrderEditFormDetails.PerformCallback();
            //UpdateQuatationTotals();
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// SALES ORDERS DETAILS

function AddNewDetail(s, e) {
    gvSalesOrderEditFormDetails.AddNewRow();
}

function RemoveDetail(s, e) {

    gvSalesOrderEditFormDetails.GetSelectedFieldValues("id_item", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "SalesOrder/SalesOrderDetailsDeleteSeleted",
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function () {
                // TODO: 
            },
            complete: function () {
                gvSalesOrderEditFormDetails.PerformCallback();
                UpdateOrderTotals();
            }
        });
    });

}

function RefreshDetail(s, e) {

}

//COMBOBOX SALES ORDER DETAILS ACTIONS

var id_itemIniAux = null;
var id_itemCurrent = null;
var id_metricUnitTypeUMPresentationIniAux = null;
var errorMessage = "";
var msgErrorConversion = "";

function OnItemValidation(s, e) {
    //gridMessageErrorSalesOrder.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorSalesOrder").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            id_itemNew: s.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux &&
            (gvSalesOrderEditFormDetails.cpEditingRowSalesRequest == null || gvSalesOrderEditFormDetails.cpEditingRowSalesRequest == 0) &&
            (gvSalesOrderEditFormDetails.cpEditingRowProductionSchedule == null || gvSalesOrderEditFormDetails.cpEditingRowProductionSchedule == 0)) {
            $.ajax({
                url: "SalesOrder/ItsRepeatedItemDetail",
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
                        } else {
                            //id_itemIniAux = 0
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

function ItemCombo_OnInit(s, e) {
    ItemCombo_InitFilter(s, e);

    id_itemIniAux = s.GetValue();
    id_itemCurrent = id_itemIniAux;
    id_metricUnitTypeUMPresentationIniAux = id_metricUnitTypeUMPresentation.GetValue();

    s.PerformCallback();
    id_metricUnitTypeUMPresentation.PerformCallback();
}

function ItemCombo_InitFilter(s, e) {
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

function SalesOrderItem_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemIniAux;
}

function SalesOrderItem_EndCallback(s, e) {
    id_item.SetValue(id_itemIniAux);
    if ((gvSalesOrderEditFormDetails.cpEditingRowSalesRequest != null && gvSalesOrderEditFormDetails.cpEditingRowSalesRequest != 0) ||
        (gvSalesOrderEditFormDetails.cpEditingRowProductionSchedule != null && gvSalesOrderEditFormDetails.cpEditingRowProductionSchedule != 0)) {
        id_item.SetEnabled(false);
        quantityOrdered.SetEnabled(false);
    }
}

function SalesOrderMetricUnitTypeUMPresentation_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemCurrent;
    e.customArgs["id_metricUnitTypeUMPresentation"] = id_metricUnitTypeUMPresentationIniAux;//s.GetValue();
}

function SalesOrderMetricUnitTypeUMPresentation_EndCallback(s, e) {
    id_metricUnitTypeUMPresentation.SetValue(id_metricUnitTypeUMPresentationIniAux);
    //if (id_competitorIniAux != null) s.SetEnabled(false);
}

function ItemCombo_SelectedIndexChanged(s, e) {
    var quantityApprovedAux = quantityApproved.GetValue();
    var strquantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();
    var resquantityApproved = strquantityApproved.replace(".", ",");

    var discountAux = discount.GetValue();
    var strdiscount = discountAux == null ? "0" : discountAux.toString();
    var resdiscount = strdiscount.replace(".", ",");

    id_itemCurrent = s.GetValue();

    UpdateItemInfo({
        id: gvSalesOrderEditFormDetails.cpEditingRowKey,
        id_item: id_itemCurrent,
        quantityApproved: resquantityApproved,
        discount: resdiscount,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });
    id_metricUnitTypeUMPresentationIniAux = null;
    id_metricUnitTypeUMPresentation.PerformCallback();
}

function OnQuantityApprovedValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Debe ser mayor que cero.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Debe ser mayor que cero.";
        }
    }
}

function QuantityApproved_Init() {

    $.ajax({
        url: "SalesOrder/UpdateSalesOrder2",//UpdateSalesRequested2",//UpdateSalesQuotation2
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
                console.log(result);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function QuantityApproved_ValueChanged(s, e) {
    var quantityApprovedAux = s.GetValue();
    var strquantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();
    var resquantityApproved = strquantityApproved.replace(".", ",");

    var discountAux = discount.GetValue();
    var strdiscount = discountAux == null ? "0" : discountAux.toString();
    var resdiscount = strdiscount.replace(".", ",");

    ItemDetailData({
        id: gvSalesOrderEditFormDetails.cpEditingRowKey,
        id_item: id_item.GetValue(),
        quantityApproved: resquantityApproved,
        discount: resdiscount,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });
}

function PriceValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Es obligatoria.";
        } else {
            errorMessage += "</br>- Precio: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Precio: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Debe ser mayor que cero.";
        } else {
            errorMessage += "</br>- Precio: Debe ser mayor que cero.";
        }
    }



}

function MetricUnitTypeUMPresentationCombo_SelectedIndexChanged(s, e) {
    var quantityApprovedAux = quantityApproved.GetValue();
    var strquantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();
    var resquantityApproved = strquantityApproved.replace(".", ",");

    var discountAux = discount.GetValue();
    var strdiscount = discountAux == null ? "0" : discountAux.toString();
    var resdiscount = strdiscount.replace(".", ",");


    ItemDetailData({
        id: gvSalesOrderEditFormDetails.cpEditingRowKey,
        id_item: id_item.GetValue(),
        quantityApproved: resquantityApproved,
        discount: resdiscount,
        id_metricUnitTypeUMPresentation: s.GetValue()
    });
}

function OnMetricUnitTypeUMPresentationValidation(s, e) {
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

    //if (errorMessage != null && errorMessage != "") {
    //    var msgErrorAux = ErrorMessage(errorMessage);
    //    gridMessageErrorProductionScheduleRequestDetail.SetText(msgErrorAux);
    //    $("#GridMessageErrorProductionScheduleRequestDetail").show();

    //}
}

function ItemCombo_DropDown(s, e) {

    $.ajax({
        url: "SalesOrder/SalesOrderDetails",
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

function ItemCombo_SelectedIndexChanged(s, e) {
    UpdateItemInfo({
        id_item: s.GetValue(),
        quantityOrdered: quantityOrdered.GetValue(),
        price: 0.0
    });
}

function QuantityOrdered_ValueChanged(s, e) {
    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityOrdered: s.GetValue(),
        price: price.GetValue()
    });
}

function Price_ValueChanged(s, e) {
    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityOrdered: quantityOrdered.GetValue(),
        price: s.GetValue()
    });
}

function UpdateItemInfo(data) {

    quantityTypeUMSale.SetValue(0);
    metricUnitSale.SetText("");
    price.SetValue(0);
    id_metricUnitTypeUMPresentation.SetValue(null);
    //currentStock.SetValue(0);

    if (data.id_item === null) {
        ValidateSalesOrderDetail();
        return;
    }

    ItemDetailData(data);
    }

function ItemDetailData(data) {

        $.ajax({
            url: "SalesOrder/ItemDetailData", //ItemRequestDetailData
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
                    masterCode.SetText(result.ItemDetailData.masterCode);
                    quantityTypeUMSale.SetValue(result.quantityTypeUMSale);
                    metricUnitSale.SetText(result.metricUnitSale);

                    quantityApproved.SetValue(result.quantityApproved);
                    //quantityOutstandingOrder.SetValue(result.quantityApproved);
                    msgErrorConversion = result.msgErrorConversion;
                    msgErrorDiscount = result.msgErrorDiscount;

                    //if (id_metricUnitTypeUMPresentationIniAux != id_metricUnitTypeUMPresentation.GetValue()) {
                    //    id_metricUnitTypeUMPresentationIniAux = id_metricUnitTypeUMPresentation.GetValue();
                    //} else {
                    //    id_metricUnitTypeUMPresentation.PerformCallback();
                    //}


                    //id_metricUnitTypeUMPresentation
                    //arrayFieldStr = [];
                    //arrayFieldStr.push("code");
                    //UpdateDetailObjects(id_metricUnitTypeUMPresentation, result.metricUnits, arrayFieldStr);
                    //currentStock.SetValue(result.ItemDetailData.currentStock);

                    price.SetValue(result.ItemDetailData.price);
                    iva.SetValue(result.ItemDetailData.iva);
                    subtotal.SetValue(result.ItemDetailData.subtotal);
                    total.SetValue(result.ItemDetailData.total);

                    // UPDATE ORDER TOTAL
                    orderSubtotal.SetValue(result.OrderTotal.subtotal);
                    orderSubtotalIVA12Percent.SetValue(result.OrderTotal.subtotalIVA12Percent);
                    orderTotalIVA12.SetValue(result.OrderTotal.totalIVA12);
                    orderDiscount.SetValue(result.OrderTotal.discount);
                    orderSubtotalIVA14Percent.SetValue(result.OrderTotal.subtotalIVA14Percent);
                    orderTotalIVA14.SetValue(result.OrderTotal.totalIVA14);
                    orderTotal.SetValue(result.OrderTotal.total);
                }
            },
            complete: function () {
                //hideLoading();
                ValidateSalesOrderDetail();
            }
        });
    
}

function ValidateSalesOrderDetail() {

    OnItemValidation(id_item, id_item);
    OnQuantityOrderedValidation(quantityOrdered, quantityOrdered);
    OnQuantityApprovedValidation(quantityApproved, quantityApproved);
    OnMetricUnitTypeUMPresentationValidation(id_metricUnitTypeUMPresentation, id_metricUnitTypeUMPresentation);
    PriceValidation(price, price);
    OnDiscountValidation(discount, discount);
}

function OnQuantityOrderedValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Ordenada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cant. Ordenada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Ordenada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cant. Ordenada: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Ordenada: Debe ser mayor que cero.";
        } else {
            errorMessage += "</br>- Cant. Ordenada: Debe ser mayor que cero.";
        }
    }


}

function OnDiscountValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Descuento: Es obligatoria.";
        } else {
            errorMessage += "</br>- Descuento: Es obligatoria.";
        }
        } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
            if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Descuento: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Descuento: Máximo 20 caracteres.";
            }
            } else if (s.GetValue() < 0) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
            if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Descuento: Debe ser mayor e igual cero.";
            } else {
            errorMessage += "</br>- Descuento: Debe ser mayor e igual que cero.";
}
} else if (msgErrorDiscount != "") {
        e.isValid = false;
        e.errorText = msgErrorConversion;
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Descuento: " + msgErrorDiscount;
} else {
            errorMessage += "</br>- Descuento: " + msgErrorDiscount;
}
}

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorSalesOrder.SetText(msgErrorAux);
        $("#GridMessageErrorSalesOrder").show();

}
}

function UpdateOrderTotals() {
    $.ajax({
        url: "SalesOrder/OrderTotals",
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
                orderSubtotal.SetValue(result.orderSubtotal);
                orderSubtotalIVA12Percent.SetValue(result.orderSubtotalIVA12Percent);
                orderTotalIVA12.SetValue(result.orderTotalIVA12);
                orderDiscount.SetValue(result.orderDiscount);
                orderSubtotalIVA14Percent.SetValue(result.orderSubtotalIVA14Percent);
                orderTotalIVA14.SetValue(result.orderTotalIVA14);
                orderTotal.SetValue(result.orderTotal);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// SALES ORDER DETAILS SELECTION

var customCommand = "";

function SalesOrderDetailsOnGridViewInit(s, e) {
    SalesOrderDetailsUpdateTitlePanel();
}

function SalesOrderDetailsOnGridViewBeginCallback(s, e) {
    customCommand = e.command;
    SalesOrderDetailsUpdateTitlePanel();
}

function SalesOrderDetailsOnGridViewEndCallback(s, e) {
    SalesOrderDetailsUpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    UpdateOrderTotals();
}

function SalesOrderDetailsOnGridViewSelectionChanged(s, e) {
    SalesOrderDetailsUpdateTitlePanel();

}

function SalesOrderDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = SalesOrderDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesOrderEditFormDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesOrderEditFormDetails.GetSelectedRowCount() - SalesOrderDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesOrderEditFormDetails.GetSelectedRowCount() > 0 && gvSalesOrderEditFormDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesOrderEditFormDetails.GetSelectedRowCount() > 0);
    //}

    try {
        btnRemoveDetail.SetEnabled(gvSalesOrderEditFormDetails.GetSelectedRowCount() > 0);
    } catch (e) {
        //No se hace nada
    }

}

function SalesOrderDetailsGetSelectedFilteredRowCount() {
    return gvSalesOrderEditFormDetails.cpFilteredRowCountWithoutPage + gvSalesOrderEditFormDetails.GetSelectedKeysOnPage().length;
}

function SalesOrderDetailsSelectAllRows() {
    gvSalesOrderEditFormDetails.SelectRows();
}

function SalesOrderDetailsClearSelection() {
    gvSalesOrderEditFormDetails.UnselectRows();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = parseInt($("#id_order").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "SalesOrder/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "SalesOrder/InitializePagination",
        type: "post",
        data: { id_salesOrder: $("#id_order").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

// MAIN FUNCTIONS

function init() {
    UpdatePagination();

    AutoCloseAlert();
}

$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});