// DIALOG BUTTONS ACTIONS

function Update(approve) {
    var valid = true;
    var validTableTabDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
    var validTableTabRequest = ASPxClientEdit.ValidateEditorsInContainerById("tableTabRequest", null, true);

    if (validTableTabDocumentCut) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validTableTabRequest) {
        UpdateTabImage({ isValid: true }, "tabRequest");
    } else {
        UpdateTabImage({ isValid: false }, "tabRequest");
        valid = false;
    }

    if (gvSalesRequestDetails.cpRowsCount === 0 || gvSalesRequestDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabDetails");
    }

    if (valid) {
        var id = $("#id_salesRequest").val();
        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditSalesRequest").serialize();
        var url = (id === "0") ? "SalesRequest/SalesRequestPartialAddNew"
                                : "SalesRequest/SalesRequestPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {

    Update(false);
    
}

function ButtonCancel_Click(s, e) {
    showPage("SalesRequest/Index", null);
}

// ACTIONS BUTTONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("SalesRequest/FormEditSalesRequest", data);
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
    }, "¿Desea aprobar el Requerimiento de Pedido?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_salesRequest").val()
    //    };
    //    showForm("SalesRequest/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_salesRequest").val()
        };
        showForm("SalesRequest/Autorize", data);
    }, "¿Desea autorizar el Requerimiento de Pedido?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_salesRequest").val()
        };
        showForm("SalesRequest/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_salesRequest").val()
        };
        showForm("SalesRequest/Cancel", data);
    }, "¿Desea anular el Requerimiento de Pedido?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_salesRequest").val()
        };
        showForm("SalesRequest/Revert", data);
    }, "¿Desea reversar el Requerimiento de Pedido?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {

}

//DETALLE DE SOLICITUD

function PurchaseRequestDetail_OnBeginCallback(s, e) {

}

// CABECERA DE SOLICITUD

function OnPriceListBeginCallback(s, e) {
    e.customArgs['id_customer'] = id_customer.GetValue();
}

function SalesRequestCustomer_SelectedIndexChanged(s, e) {
    id_priceList.PerformCallback();
}

function OnPriceList_SelectedIndexChanged(s, e) {
    var data = {
        id_priceList: s.GetValue()
    };

    $.ajax({
        url: "SalesRequest/OnPriceList_SelectedIndexChanged",
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
            gvSalesRequestDetails.PerformCallback();
            //UpdateQuatationTotals();
        },
        complete: function () {
            //hideLoading();
        }
    });
}


// DETALLE DE SOLICITUD BUTTONS ACTIONS

function AddNewDetail(s, e) {
    gvSalesRequestDetails.AddNewRow();
}

function RemoveDetail(s, e) {
    gvSalesRequestDetails.GetSelectedFieldValues("id_item", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "SalesRequest/SalesRequestDetailsDeleteSelectedItems",
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
            success: function (result) {
                //$("#maincontent").html(result);
                //hideLoading();
            },
            complete: function () {
                //gvSalesRequestDetails.PerformCallback();
                gvSalesRequestDetails.UnselectRows();
                //hideLoading();
            }
        });
    });
}

function RefreshDetail(s, e) {
    showLoading();
    gvSalesRequestDetails.PerformCallback();
    gvSalesRequestDetails.UnselectRows();
    hideLoading();
}

//DETALLE DE SOLICITUD COMBOS

var id_itemIniAux = null;
var id_itemCurrent = null;
var id_metricUnitTypeUMPresentationIniAux = null;
var errorMessage = "";
var msgErrorConversion = "";
var msgErrorDiscount = "";

function OnItemValidation(s, e) {
    //gridMessageErrorSalesRequest.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorSalesRequest").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            id_itemNew: s.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux &&
            (gvSalesRequestDetails.cpEditingRowSalesQuotation == null || gvSalesRequestDetails.cpEditingRowSalesQuotation == 0) &&
            (gvSalesRequestDetails.cpEditingRowBusinessOportunity == null && gvSalesRequestDetails.cpEditingRowBusinessOportunity == 0)) {
            $.ajax({
                url: "SalesRequest/ItsRepeatedItemDetail",
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

function SalesRequestItem_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemIniAux;
}

function SalesRequestItem_EndCallback(s, e) {
    id_item.SetValue(id_itemIniAux);
    if ((gvSalesRequestDetails.cpEditingRowSalesQuotation != null && gvSalesRequestDetails.cpEditingRowSalesQuotation != 0) ||
        (gvSalesRequestDetails.cpEditingRowBusinessOportunity != null && gvSalesRequestDetails.cpEditingRowBusinessOportunity != 0)) {
        id_item.SetEnabled(false);
        quantityRequested.SetEnabled(false);
    } 
}

function SalesRequestMetricUnitTypeUMPresentation_BeginCallback(s, e) {
    //if ((gvSalesRequestDetails.cpEditingRowSalesQuotation != null && gvSalesRequestDetails.cpEditingRowSalesQuotation != 0) ||
    //    (gvSalesRequestDetails.cpEditingRowBusinessOportunity != null && gvSalesRequestDetails.cpEditingRowBusinessOportunity != 0)) {
    //    e.customArgs["id_item"] = id_itemIniAux;
    //    console.log("id_itemIniAux:" + id_itemIniAux);
    //} else {
    //    e.customArgs["id_item"] = id_item.GetValue();
    //    console.log("id_item.GetValue():" + id_item.GetValue());
    //}
    e.customArgs["id_item"] = id_itemCurrent;
    e.customArgs["id_metricUnitTypeUMPresentation"] = id_metricUnitTypeUMPresentationIniAux;//s.GetValue();
}

function SalesRequestMetricUnitTypeUMPresentation_EndCallback(s, e) {
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
        id: gvSalesRequestDetails.cpEditingRowKey,
        id_item: id_itemCurrent,
        quantityApproved: resquantityApproved,
        discount: resdiscount,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });
    id_metricUnitTypeUMPresentationIniAux = null;
    id_metricUnitTypeUMPresentation.PerformCallback();
}

function UpdateItemInfo(data) {

    quantityTypeUMSale.SetValue(0);
    metricUnitSale.SetText("");
    price.SetValue(0);
    id_metricUnitTypeUMPresentation.SetValue(null);
    currentStock.SetValue(0);

    if (data.id_item === null) {
        ValidateSalesRequestDetail();
        return;
    }

    ItemDetailData(data);
}

function ItemDetailData(data) {

    $.ajax({
        url: "SalesRequest/ItemDetailData", //ItemRequestDetailData
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
                quantityOutstandingOrder.SetValue(result.quantityApproved);
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
                currentStock.SetValue(result.ItemDetailData.currentStock);

                price.SetValue(result.ItemDetailData.price);
                iva.SetValue(result.ItemDetailData.iva);
                subtotal.SetValue(result.ItemDetailData.subtotal);
                total.SetValue(result.ItemDetailData.total);

                // UPDATE ORDER TOTAL
                requestSubtotal.SetValue(result.RequestTotal.subtotal);
                requestSubtotalIVA12Percent.SetValue(result.RequestTotal.subtotalIVA12Percent);
                requestTotalIVA12.SetValue(result.RequestTotal.totalIVA12);
                requestDiscount.SetValue(result.RequestTotal.discount);
                requestSubtotalIVA14Percent.SetValue(result.RequestTotal.subtotalIVA14Percent);
                requestTotalIVA14.SetValue(result.RequestTotal.totalIVA14);
                requestTotal.SetValue(result.RequestTotal.total);
            }
        },
        complete: function () {
            //hideLoading();
            ValidateSalesRequestDetail();
        }
    });

}

function ValidateSalesRequestDetail() {

    OnItemValidation(id_item, id_item);
    OnQuantityRequestedValidation(quantityRequested, quantityRequested);
    OnQuantityApprovedValidation(quantityApproved, quantityApproved);
    OnMetricUnitTypeUMPresentationValidation(id_metricUnitTypeUMPresentation, id_metricUnitTypeUMPresentation);
    PriceValidation(price, price);
    OnDiscountValidation(discount, discount);
}

function OnQuantityRequestedValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Requerida: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cant. Requerida: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Requerida: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cant. Requerida: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cant. Requerida: Debe ser mayor que cero.";
        } else {
            errorMessage += "</br>- Cant. Requerida: Debe ser mayor que cero.";
        }
    }
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
        url: "SalesRequest/UpdateSalesRequested2",//UpdateSalesQuotation2
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
        id: gvSalesRequestDetails.cpEditingRowKey,
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
        id: gvSalesRequestDetails.cpEditingRowKey,
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

function Discount_ValueChanged(s, e) {
    var quantityApprovedAux = quantityApproved.GetValue();
    var strquantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();
    var resquantityApproved = strquantityApproved.replace(".", ",");

    var discountAux = s.GetValue();
    var strdiscount = discountAux == null ? "0" : discountAux.toString();
    var resdiscount = strdiscount.replace(".", ",");

    ItemDetailData({
        id: gvSalesRequestDetails.cpEditingRowKey,
        id_item: id_item.GetValue(),
        quantityApproved: resquantityApproved,
        discount: resdiscount,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });
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
        gridMessageErrorSalesRequest.SetText(msgErrorAux);
        $("#GridMessageErrorSalesRequest").show();

    }
}

//function ItemCombo_SelectedIndexChanged(s, e) {
//    masterCode.SetText("");
//    metricUnit.SetText("");
//    //  id_proposedProvider.SetSelectedItem(null);
//    currentStock.SetValue(0);
//    // minimumStock.SetText("");
//    var id_item = s.GetValue();
//    if (id_item !== null && id_item !== undefined) {
//        $.ajax({
//            url: "SalesRequest/ItemDetailData",
//            type: "post",
//            data: {
//                id_item: id_item,
//                quantityRequested: quantityRequested.GetValue(),
//                price: 0
//            },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                if (result !== null && result != undefined) {
//                    masterCode.SetText(result.ItemDetailData.masterCode);
//                    metricUnit.SetValue(result.ItemDetailData.um);
//                    currentStock.SetValue(result.ItemDetailData.currentStock);
//                    price.SetValue(result.ItemDetailData.price);
//                    iva.SetValue(result.ItemDetailData.iva);
//                    subtotal.SetValue(result.ItemDetailData.subtotal);
//                    total.SetValue(result.ItemDetailData.total);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });
//    }
//}

// SELECTION

function OnGridViewDetailsInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewDetailsSelectionChanged(s, e) {
    UpdateTitlePanel();
}

var customCommand = "";

function OnGridViewDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewDetailsEndCallback(s, e) {
    UpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");

    //    masterCode.SetText("");
    //    metricUnit.SetText("");
    //    //  id_proposedProvider.SetSelectedItem(null);
    //    currentStock.SetText("");
    //    // minimumStock.SetText("");

    //    var id_item = s.GetEditor("id_item").GetValue();

    //    if (id_item !== null && id_item !== undefined) {

    //        $.ajax({
    //            url: "PurchaseRequest/ItemDetailData",
    //            type: "post",
    //            data: { id_item: id_item },
    //            async: true,
    //            cache: false,
    //            error: function (error) {
    //                console.log(error);
    //            },
    //            beforeSend: function () {
    //                //showLoading();
    //            },
    //            success: function (result) {
    //                if (result !== null && result !== undefined) {
    //                    masterCode.SetText(result.masterCode);
    //                    metricUnit.SetText(result.metricUnit);
    //                    id_proposedProvider.SetValue(result.id_proposedProvider);
    //                    currentStock.SetValue(result.currentStock);
    //                    //minimumStock.SetValue(result.minimumStock);
    //                }
    //            },
    //            complete: function () {
    //                //hideLoading();
    //            }
    //        });
    //    }
    //}
    UpdateRequestTotals();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesRequestDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesRequestDetails.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesRequestDetails.GetSelectedRowCount() > 0 && gvSalesRequestDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesRequestDetails.GetSelectedRowCount() > 0);
    //}
    try {
        btnRemoveDetail.SetEnabled(gvSalesRequestDetails.GetSelectedRowCount() > 0);
    } catch (e) {
        //No se hace nada
    }
    
}

function GetSelectedFilteredRowCount() {
    return gvSalesRequestDetails.cpFilteredRowCountWithoutPage + gvSalesRequestDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SalesRequestDetailsClearSelection() {
    gvSalesRequestDetails.UnselectRows();
}

function SalesRequestDetailsSelectAllRows() {
    gvSalesRequestDetails.SelectRows();
}

function UpdateRequestTotals() {
    $.ajax({
        url: "SalesRequest/RequestTotals",
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
                requestSubtotal.SetValue(result.requestSubtotal);
                requestSubtotalIVA12Percent.SetValue(result.requestSubtotalIVA12Percent);
                requestTotalIVA12.SetValue(result.requestTotalIVA12);
                requestDiscount.SetValue(result.requestDiscount);
                requestSubtotalIVA14Percent.SetValue(result.requestSubtotalIVA14Percent);
                requestTotalIVA14.SetValue(result.requestTotalIVA14);
                requestTotal.SetValue(result.requestTotal);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "SalesRequest/InitializePagination",
        type: "post",
        data: { id_salesRequest: $("#id_salesRequest").val() },
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

function UpdateView() {
    var id = parseInt($("#id_salesRequest").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS
    $.ajax({
        url: "SalesRequest/Actions",
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