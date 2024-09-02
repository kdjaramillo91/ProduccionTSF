// DIALOG BUTTONS ACTIONS

function Update(approve) {
    var valid = true;
    var validTableTabDocument = ASPxClientEdit.ValidateEditorsInContainerById("tableTabDocument", null, true);
    var validTableTabQuotation = ASPxClientEdit.ValidateEditorsInContainerById("tableTabQuotation", null, true);

    if (validTableTabDocument) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validTableTabQuotation) {
        UpdateTabImage({ isValid: true }, "tabQuotation");
    } else {
        UpdateTabImage({ isValid: false }, "tabQuotation");
        valid = false;
    }

    if (gvSalesQuotationDetails.cpRowsCount === 0 || gvSalesQuotationDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabDetails");
    }

    if (valid) {
        var id = $("#id_quotation").val();
        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditSalesQuotation").serialize();
        var url = (id === "0") ? "SalesQuotation/SalesQuotationPartialAddNew"
                                : "SalesQuotation/SalesQuotationPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {

    Update(false);
}

function ButtonCancel_Click(s, e) {
    showPage("SalesQuotation/Index", null);
}

// ACTION BUTTONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("SalesQuotation/SalesQuotationEditForm", data);
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
    }, "¿Desea aprobar la cotización?");

    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_quotation").val()
    //    };
    //    showForm("SalesQuotation/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_quotation").val()
        };
        showForm("SalesQuotation/Autorize", data);
    }, "¿Desea autorizar la cotización?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_quotation").val()
        };
        showForm("SalesQuotation/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_quotation").val()
        };
        showForm("SalesQuotation/Cancel", data);
    }, "¿Desea anular la cotización?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_quotation").val()
        };
        showForm("SalesQuotation/Revert", data);
    }, "¿Desea reversar la cotización?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    
}

// CABECERA DE COTIZACIÓN

function OnPriceListBeginCallback(s, e) {
    e.customArgs['id_customer'] = id_customer.GetValue();
}


function SalesQuotationCustomer_SelectedIndexChanged(s, e) {
    id_priceList.PerformCallback();
}

function OnPriceList_SelectedIndexChanged(s, e) {
    var data = {
        id_priceList: s.GetValue()
    };

    $.ajax({
        url: "SalesQuotation/OnPriceList_SelectedIndexChanged",
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
            gvSalesQuotationDetails.PerformCallback();
            //UpdateQuatationTotals();
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// DETALLE DE SOLICITUD BUTTONS ACTIONS

function AddNewDetail(s, e) {
    gvSalesQuotationDetails.AddNewRow();
}

function RemoveDetail(s, e) {

    //gvSalesQuotationDetails.GetSelectedFieldValues("id_item", function (values) {
    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "SalesQuotation/SalesQuotationDetailsDeleteSelectedItems",
    //        type: "post",
    //        data: { itemIds: selectedRows },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            //$("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            gvSalesQuotationDetails.PerformCallback();
    //            gvSalesQuotationDetails.UnselectRows();
    //            hideLoading();
    //        }
    //    });
    //});
}

function RefreshDetail(s, e) {
    showLoading();
    gvSalesQuotationDetails.PerformCallback();
    gvSalesQuotationDetails.UnselectRows();
    hideLoading();
}


// SALE QUOTATION DETAILS ACTIONS

var id_itemIniAux = null;
var id_itemCurrent = null;
var id_metricUnitTypeUMPresentationIniAux = null;
var errorMessage = "";
var msgErrorConversion = "";

function OnItemValidation(s, e) {
    //gridMessageErrorSalesQuotation.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorSalesQuotation").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            id_itemNew: s.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux) {
            $.ajax({
                url: "SalesQuotation/ItsRepeatedItemDetail",
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
                            id_itemIniAux = 0
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

function SalesQuotationItem_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemIniAux;
}

function SalesQuotationItem_EndCallback(s, e) {
    id_item.SetValue(id_itemIniAux);
    //if (id_competitorIniAux != null) s.SetEnabled(false);
}

function SalesQuotationMetricUnitTypeUMPresentation_BeginCallback(s, e) {
    e.customArgs["id_item"] = id_itemCurrent;
    e.customArgs["id_metricUnitTypeUMPresentation"] = id_metricUnitTypeUMPresentationIniAux;
}

function SalesQuotationMetricUnitTypeUMPresentation_EndCallback(s, e) {
    id_metricUnitTypeUMPresentation.SetValue(id_metricUnitTypeUMPresentationIniAux);
    //if (id_competitorIniAux != null) s.SetEnabled(false);
}

function ItemCombo_OnInit(s, e) {

    ItemCombo_InitFilter(s, e);

    id_itemIniAux = s.GetValue();
    id_itemCurrent = id_itemIniAux;
    id_metricUnitTypeUMPresentationIniAux = id_metricUnitTypeUMPresentation.GetValue();

    s.PerformCallback();
    id_metricUnitTypeUMPresentation.PerformCallback();
    //var data = {
    //    id_itemCurrent: s.GetValue(),
    //    id_metricUnitTypeUMPresentationCurrent: id_metricUnitTypeUMPresentation.GetValue()
    //};

    //if (data.id_itemCurrent != null && id_purchaseRequestDetailIniAux != null && id_purchaseRequestDetailIniAux != 0) s.SetEnabled(false);

    //$.ajax({
    //    url: "SalesQuotation/SalesQuotationDetails",//PurchaseOrderDetails
    //    type: "post",
    //    data: data,
    //    async: false,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //        //id_metricUnit.SetValue(null);
    //    },
    //    beforeSend: function () {
    //        //showLoading();
    //    },
    //    success: function (result) {
    //        //id_item
    //        var arrayFieldStr = [];
    //        arrayFieldStr.push("masterCode");
    //        arrayFieldStr.push("name");
    //        arrayFieldStr.push("itemSaleInformationMetricUnitCode");
    //        UpdateDetailObjects(id_item, result.items, arrayFieldStr);
    //        //id_metricUnitTypeUMPresentation
    //        arrayFieldStr = [];
    //        arrayFieldStr.push("code");
    //        UpdateDetailObjects(id_metricUnitTypeUMPresentation, result.metricUnits, arrayFieldStr);
    //    },
    //    complete: function () {
    //        //hideLoading();
    //    }
    //});
}

function ItemDetailData(data) {

    $.ajax({
        url: "SalesQuotation/ItemDetailData", //ItemRequestDetailData
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
                masterCode.SetText(result.masterCode);
                quantity.SetValue(result.quantity);
                metricUnitSale.SetText(result.metricUnit);

                quantityTypeUMPresentation.SetValue(result.quantityTypeUMPresentation);
                msgErrorConversion = result.msgErrorConversion;

                //id_metricUnitTypeUMPresentation.PerformCallback();
                //id_metricUnitTypeUMPresentation
                //arrayFieldStr = [];
                //arrayFieldStr.push("code");
                //UpdateDetailObjects(id_metricUnitTypeUMPresentation, result.metricUnits, arrayFieldStr);

                price.SetValue(result.ItemDetailData.price);
                iva.SetValue(result.ItemDetailData.iva);
                subtotal.SetValue(result.ItemDetailData.subtotal);
                total.SetValue(result.ItemDetailData.total);

                // UPDATE ORDER TOTAL
                quotationSubtotal.SetValue(result.OrderTotal.subtotal);
                quotationSubtotalIVA12Percent.SetValue(result.OrderTotal.subtotalIVA12Percent);
                quotationTotalIVA12.SetValue(result.OrderTotal.totalIVA12);
                quotationDiscount.SetValue(result.OrderTotal.discount);
                quotationSubtotalIVA14Percent.SetValue(result.OrderTotal.subtotalIVA14Percent);
                quotationTotalIVA14.SetValue(result.OrderTotal.totalIVA14);
                quotationTotal.SetValue(result.OrderTotal.total);
            }
        },
        complete: function () {
            //hideLoading();
            ValidateSalesQuotationDetail();
        }
    });

}

function UpdateItemInfo(data) {

    quantity.SetValue(0);
    metricUnitSale.SetText("");
    price.SetValue(0);
    id_metricUnitTypeUMPresentation.SetValue(null);

    if (data.id_item === null) {
        ValidateSalesQuotationDetail();
        return;
    }

    ItemDetailData(data);
}

function ItemCombo_SelectedIndexChanged(s, e) {
    var quantityTypeUMPresentationAux = quantityTypeUMPresentation.GetValue();
    var strquantityTypeUMPresentation = quantityTypeUMPresentationAux == null ? "0" : quantityTypeUMPresentationAux.toString();
    var resquantityTypeUMPresentation = strquantityTypeUMPresentation.replace(".", ",");

    id_itemCurrent = s.GetValue();

    UpdateItemInfo({
        id: gvSalesQuotationDetails.cpEditingRowKey,
        id_item: id_itemCurrent,
        quantityTypeUMPresentation: resquantityTypeUMPresentation,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });

    id_metricUnitTypeUMPresentationIniAux = null;
    id_metricUnitTypeUMPresentation.PerformCallback();
}

function ValidateSalesQuotationDetail() {

    OnItemValidation(id_item, id_item);
    OnQuantityTypeUMPresentationValidation(quantityTypeUMPresentation, quantityTypeUMPresentation);
    OnMetricUnitTypeUMPresentationValidation(id_metricUnitTypeUMPresentation, id_metricUnitTypeUMPresentation);
    PriceValidation(price, price);
}

function OnQuantityTypeUMPresentationValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Total: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Total: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Total: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad Total: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Total: Debe ser mayor que cero.";
        } else {
            errorMessage += "</br>- Cantidad Total: Debe ser mayor que cero.";
        }
    }
}

function QuantityTypeUMPresentation_Init() {

    $.ajax({
        url: "SalesQuotation/UpdateSalesQuotation2",//UpdatePurchaseOrder2
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

function QuantityTypeUMPresentation_ValueChanged(s, e) {
    var quantityTypeUMPresentationAux = s.GetValue();
    var strquantityTypeUMPresentation = quantityTypeUMPresentationAux == null ? "0" : quantityTypeUMPresentationAux.toString();
    var resquantityTypeUMPresentation = strquantityTypeUMPresentation.replace(".", ",");

    ItemDetailData({
        id: gvSalesQuotationDetails.cpEditingRowKey,
        id_item: id_item.GetValue(),
        quantityTypeUMPresentation: resquantityTypeUMPresentation,
        id_metricUnitTypeUMPresentation: id_metricUnitTypeUMPresentation.GetValue()
    });

    //var quantityApprovedAux = s.GetValue();
    //var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    //var priceAux = price.GetValue();
    //var strPrice = priceAux == null ? "0" : priceAux.toString();

    //var resQuantityApproved = strQuantityApproved.replace(".", ",");
    //var resPrice = strPrice.replace(".", ",");
    //UpdateItemInfo({
    //    id_item: id_item.GetValue(),
    //    quantityApproved: resQuantityApproved,
    //    price: resPrice
    //});
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

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorSalesQuotation.SetText(msgErrorAux);
        $("#GridMessageErrorSalesQuotation").show();

    }
}

function MetricUnitTypeUMPresentationCombo_SelectedIndexChanged(s, e) {
    var quantityTypeUMPresentationAux = quantityTypeUMPresentation.GetValue();
    var strquantityTypeUMPresentation = quantityTypeUMPresentationAux == null ? "0" : quantityTypeUMPresentationAux.toString();
    var resquantityTypeUMPresentation = strquantityTypeUMPresentation.replace(".", ",");

    ItemDetailData({
        id: gvSalesQuotationDetails.cpEditingRowKey,
        id_item: id_item.GetValue(),
        quantityTypeUMPresentation: resquantityTypeUMPresentation,
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

function UpdateQuatationTotals() {
    $.ajax({
        url: "SalesQuotation/QuotationTotals",
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
                quotationSubtotal.SetValue(result.quotationSubtotal);
                quotationSubtotalIVA12Percent.SetValue(result.quotationSubtotalIVA12Percent);
                quotationTotalIVA12.SetValue(result.quotationTotalIVA12);
                quotationDiscount.SetValue(result.quotationDiscount);
                quotationSubtotalIVA14Percent.SetValue(result.quotationSubtotalIVA14Percent);
                quotationTotalIVA14.SetValue(result.quotationTotalIVA14);
                quotationTotal.SetValue(result.quotationTotal);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

//SELECTION

var customCommand = "";

function SaleQuotationDetailsOnGridViewInit(s, e) {
    SaleQuotationDetailsUpdateTitlePanel();
}

function SaleQuotationDetailsOnGridViewBeginCallback(s, e) {
    customCommand = e.command;
    SaleQuotationDetailsUpdateTitlePanel();
}

function SaleQuotationDetailsOnGridViewEndCallback(s, e) {
    SaleQuotationDetailsUpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    UpdateQuatationTotals();
}

function SaleQuotationDetailsOnGridViewSelectionChanged(s, e) {
    SaleQuotationDetailsUpdateTitlePanel();

}

function SaleQuotationDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = SaleQuotationDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesQuotationDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesQuotationDetails.GetSelectedRowCount() - SaleQuotationDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesQuotationDetails.GetSelectedRowCount() > 0 && gvSalesQuotationDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesQuotationDetails.GetSelectedRowCount() > 0);
    //}

    //btnRemoveDetail.SetEnabled(gvSalesQuotationDetails.GetSelectedRowCount() > 0);
}

function SaleQuotationDetailsGetSelectedFilteredRowCount() {
    return gvSalesQuotationDetails.cpFilteredRowCountWithoutPage + gvSalesQuotationDetails.GetSelectedKeysOnPage().length;
}

function SaleQuotationDetailsSelectAllRows() {
    gvSalesQuotationDetails.SelectRows();
}

function SaleQuotationDetailsClearSelection() {
    gvSalesQuotationDetails.UnselectRows();
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

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "SalesQuotation/InitializePagination",
        type: "post",
        data: { id_salesQuotation: $("#id_quotation").val() },
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
    var id = parseInt($("#id_quotation").val());



    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "SalesQuotation/Actions",
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