//PRINT REPORT OPTIONS
function PRInvoiceFiscal(s, e) {
    var codeReport = "RFFLV1";
    var data = "codeReport=" + codeReport + "&" + $("#SalesQuotationExteriorFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "SalesQuotationExterior/PRInvoiceFiscal",
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

                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

// Buttons
function AddNewDocument(s, e) {
    AddNewSalesQuotationExteriorManual(s, e);
}

function CopyDocument(s, e) {

    gvSalesQuotationExterior.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("SalesQuotationExterior/InvoiceCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        genericSelectedFieldActionCallBack("gvSalesQuotationExterior", "SalesQuotationExterior/ApproveDocuments",
            function (result) {

                if (result.codeReturn == 1) {
                    gvSalesQuotationExterior.UnselectRows();
                }

                if (result.message.length > 0) {
                    $("#msgInfoSalesQuotationExterior").empty();

                    $("#msgInfoSalesQuotationExterior").append(result.message)
                        .show()
                        .delay(5000)
                        .hide(0);
                }

            });
    }, "¿Desea aprobar los documentos seleccionados?");
}

//function AutorizeDocuments(s, e) {
//    showConfirmationDialog(function () {
//        genericSelectedFieldActionCallBack("gvSalesQuotationExterior", "SalesQuotationExterior/AutorizeDocuments",
//            function (result) {
//                if (result.codeReturn == 1) {
//                    gvSalesQuotationExterior.UnselectRows();
//                }
//                if (result.message.length > 0) {
//                    $("#msgInfoSalesQuotationExterior").empty();

//                    $("#msgInfoSalesQuotationExterior").append(result.message)
//                        .show()
//                        .delay(5000)
//                        .hide(0);
//                }
//            });
//    }, "¿Desea Autorizar los documentos seleccionados?");
//}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        genericPerformDocumentAction("gvSalesQuotationExterior", "SalesQuotationExterior/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        genericPerformDocumentAction("gvSalesQuotationExterior", "SalesQuotationExterior/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function PrintDocuments(s, e) {
    showConfirmationDialog(function () {
        genericSelectedFieldActionCallBack("gvSalesQuotationExterior", null,
            function (rows) {
                PrintInvoiceGeneric(rows);
            });
    }, "¿Desea visualizar los documentos seleccionados?");
}

function SalesQuotationExteriorOnSelectionChanged(s, e) {
    var _btnPrint = ASPxClientControl.GetControlCollection().GetByName("btnPrint");
    var _btnAutorize = ASPxClientControl.GetControlCollection().GetByName("btnAutorize");
    var _btnApprove = ASPxClientControl.GetControlCollection().GetByName("btnApprove");

    if (s.GetSelectedRowCount() == null || s.GetSelectedRowCount() == 0) {
        if (typeof _btnPrint != 'undefined' && _btnPrint != null) {
            _btnPrint.SetEnabled(false);
        }

        if (typeof _btnAutorize != 'undefined' && _btnAutorize != null) {
            _btnAutorize.SetEnabled(false);
        }

        if (typeof _btnApprove != 'undefined' && _btnApprove != null) {
            _btnApprove.SetEnabled(false);
        }
        return;
    }

    _btnPrint.SetEnabled(true);
    _btnApprove.SetEnabled(true);
}

function SalesQuotationExteriorOnRowDoubleClick(s, e) {

    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("SalesQuotationExterior/FormEditSalesQuotationExterior", { id: value });
    });
}

function SalesQuotationExteriorSelectAllRows() {
    gvSalesQuotationExterior.SelectRows();
}

function SalesQuotationExteriorClearSelection() {
    gvSalesQuotationExterior.UnselectRows();
}

function SalesQuotationExteriorGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvSalesQuotationExterior.GetRowKey(e.visibleIndex)
        };
        showPage("SalesQuotationExterior/FormEditSalesQuotationExterior", data);
    }
}

function UpdateButtons() {
    genericSetElementVisibility("btnHistory", false);
    genericSetElementVisibility("btnProtect", false);
}

function SalesQuotationExteriorResultsDetailViewDetails_BeginCallback(s, e) {
    e.customArgs["id_invoiceExterior"] = $("#id_invoiceExterior").val();
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, fechaEmisionDesde.GetValue(), fechaEmisionHasta.GetValue(), "Rango de Fecha no válido");
}

function OnInitDateDesde() {
    var d = new Date();
    fechaEmisionDesde.SetDate(new Date(d.getFullYear(), d.getMonth(), 1));
}

function OnInitDateHasta() {
    fechaEmisionHasta.SetDate(new Date());
}

function btnSearch_click(s, e) {
    var data = {

        fechaEmisionDesde: fechaEmisionDesde.GetText(),
        fechaEmisionHasta: fechaEmisionHasta.GetText(),
        id_documentState: id_documentState.GetValue(),
        number: number.GetValue(),
        reference: reference.GetValue(),
        purchaseOrder: purchaseOrder.GetValue(),
        id_customer: id_customer.GetValue(),
        id_consignee: id_consignee.GetValue(),
        id_seller: id_seller.GetValue(),
        items: items.GetValue().split(',')
    };

    if (data != null) {
        $.ajax({
            url: "SalesQuotationExterior/SalesQuotationExteriorResults",
            type: "post",
            data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}

function btnClear_click(s, e) {

    number.SetText("");
    reference.SetText("");
    var d = new Date();
    fechaEmisionDesde.SetDate(new Date(d.getFullYear(), d.getMonth(), 1));
    fechaEmisionHasta.SetDate(new Date());
    id_documentState.SetSelectedItem(null);
    id_customer.SetSelectedItem(null);
    id_consignee.SetSelectedItem(null);
    id_seller.SetSelectedItem(null);
    items.SetText("");
}

function OnRangeEmbarqueDateValidation(s, e) {
    OnRangeDateValidation(e, fechaEmbarqueDesde.GetValue(), fechaEmbarqueHasta.GetValue(), "Rango de Fecha no válido");
}

function AddNewSalesQuotationExteriorManual(s, e) {
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("SalesQuotationExterior/FormEditSalesQuotationExterior", data);
}

var globalIdMetricUnitInvoice = 0;
var globalDinamycIdMetricUnitInvoice = 0;

function validationSalesQuotationExteriorMetricUnitInvoice(s, e) {
    if (s.GetValue() == null) {
        s.SetValue(globalIdMetricUnitInvoice);
    }
    else {
        globalIdMetricUnitInvoice = s.GetValue();
    }
}

function onInitSalesQuotationExteriorMetricUnitInvoice(s, e) {
    globalIdMetricUnitInvoice = s.GetValue();
    globalDinamycIdMetricUnitInvoice = s.GetValue();
    validateFirstExistMetricUnitInvoice(s, e);
}

function onChangeSalesQuotationExteriorMetricUnitInvoice(s, e) {
    if (globalDinamycIdMetricUnitInvoice != s.GetValue()) {
        validateFirstExistMetricUnitInvoice(s, e);
        PostChangeMetricUnitInvoice();
        globalDinamycIdMetricUnitInvoice = s.GetValue();
    }
}

function validateFirstExistMetricUnitInvoice(s, e) {
    var objid_metricUnitInvoice = null;
    if (s != null) {
        objid_metricUnitInvoice = s.GetValue();
    }
    else {
        objid_metricUnitInvoice = (ASPxClientControl.GetControlCollection().GetByName("id_metricUnitInvoice") == null) ? null : ASPxClientControl.GetControlCollection().GetByName("id_metricUnitInvoice").GetValue();
    }

    var _psv_metricUnitInvoiceDetail = (objid_metricUnitInvoice == undefined) ? 0 : ((objid_metricUnitInvoice == null || objid_metricUnitInvoice == 0) ? 0 : objid_metricUnitInvoice);
    var _action = (objid_metricUnitInvoice == undefined) ? false : ((objid_metricUnitInvoice == null) ? false : true);

    $("#id_metricUnitInvoiceDetail").val(_psv_metricUnitInvoiceDetail);

    //btnNewDetail.SetEnabled(_action);
}

function PostChangeMetricUnitInvoice() {
    if (gvInvoiceDetail.cpVisibleRowCount > 0 && !gvInvoiceDetail.IsEditing()) {
        GenericFreeStyleShowConfirmationDialogTwoOptionsWithActionRightNow("Al cambiar la unidad de medida se cambiará a todos los items ingresados?<br>Esto podría hacer que varie la cantidad del producto.<br>Si existe esta variación que desea que ocurra?", "Afectar al Precio", "Afectar al Total", function () { afectarPrecio() }, function () { afectarTotal() });
    }
}


function afectarPrecio() {
    var _data = { id_MetricUnitInvoice: id_metricUnitInvoice.GetValue(), accion: "PRICE" };
    genericPerformDocumentActionWithData("gvInvoiceDetail", "SalesQuotationExterior/ChangeMetricUnitInvoiceMaster", _data);
}


function afectarTotal() {
    var _data = { id_MetricUnitInvoice: id_metricUnitInvoice.GetValue(), accion: "TOTAL" };
    genericPerformDocumentActionWithData("gvInvoiceDetail", "SalesQuotationExterior/ChangeMetricUnitInvoiceMaster", _data);
}

function tariffHeadingDetail_SelectedIndexChanged(s, e) { }


// validaciones
function OnValidation(s, e) {
    e.isValid = true;
}


// --------------------------
// Detalle
// --------------------------
function SalesQuotationExteriorDetail_OnGridViewInit(s, e) {
    updateEstatusButton();
}

function SalesQuotationExteriorDetail_OnSelectionChanged(s, e) {
    updateEstatusButton();
}

function updateEstatusButton() {
    btnRemoveDetail.SetEnabled(gvInvoiceDetail.GetSelectedRowCount() > 0);
}

function Invoice_ChangeInvoiceTotal(s, e) {
    reloadInvoiceTotal();
}

function reloadInvoiceTotal() {
    var _valueTransportationExpenses = (valueTransportationExpenses.GetValue() == null) ? 0 : valueTransportationExpenses.GetValue().toString().replace(".", ",");
    var _valueCustomsExpenditures = (valueCustomsExpenditures.GetValue() == null) ? 0 : valueCustomsExpenditures.GetValue().toString().replace(".", ",");
    var _valueInternationalInsurance = (valueInternationalInsurance.GetValue() == null) ? 0 : valueInternationalInsurance.GetValue().toString().replace(".", ",");
    var _valueInternationalFreight = (valueInternationalFreight.GetValue() == null) ? 0 : valueInternationalFreight.GetValue().toString().replace(".", ",");

    var data = {
        valueTransportationExpenses: _valueTransportationExpenses,
        valueCustomsExpenditures: _valueCustomsExpenditures,
        valueInternationalInsurance: _valueInternationalInsurance,
        valueInternationalFreight: _valueInternationalFreight
    };

    var errCallBack = function (error) { console.log(error) };
    var okCallBackTotales = function (result) {
        if (result != null) {
            $("#objTotalesPartial").empty();
            $("#objTotalesPartial").html(result);
            SalesQuotationExteriorTermsNegotiation_SelectedIndexChanged(null, null);
            gvSalesQuotationExteriorPaymentTermDetailView.PerformCallback();
        }
    };

    var okCallBackInforAdicional = function (result) {
        if (result != null) {
            $("#objInfoAdicional").empty();
            $("#objInfoAdicional").html(result);
        }
    };

    genericAjaxCall('SalesQuotationExterior/InvoiceExternalTotales', false, data, errCallBack, showLoading(), okCallBackTotales, hideLoading());
    genericAjaxCall('SalesQuotationExterior/InvoiceExternalWeight', false, {}, errCallBack, showLoading(), okCallBackInforAdicional, hideLoading());
}


// Inicializacion Grid
function SalesQuotationExteriorDetail_OnEndCallback(s, e) {
    /* edicion actualizacion*/
    if (s.editMode == 1 && s.editState == 1) {
        numBoxesValueChanged(null, null);
    }

    //if (s.editMode == 1 && s.editState == 0) {
    reloadInvoiceTotal();
    //OnInvoiceTotalValueReady();
    //}
    if (gvInvoiceDetail.cpError !== null && gvInvoiceDetail.cpError !== "") {
        NotifyError(gvInvoiceDetail.cpError);
    }
}
function OnNombreExtranjero_CheckedChanged(s, e) {
    gvInvoiceDetail.PerformCallback();
}
function ItemCombo_BeginCallback(s, e) {
    e.customArgs["id_Buyer"] = id_buyer.GetValue();
    e.customArgs["busquedaNombre"] = bsNombreExtranjero.GetChecked();
    e.customArgs["nameItemFilter"] = nameItemFilter.GetText();
    e.customArgs["sizeBegin"] = idSizeBegin.GetValue();
    e.customArgs["sizeEnd"] = idSizeEnd.GetValue();

    e.customArgs["id_inventoryLine"] = id_inventoryLine.GetValue();
    e.customArgs["id_itemType"] = id_itemType.GetValue();
    e.customArgs["id_itemTypeCategory"] = id_itemTypeCategory.GetValue();
    e.customArgs["id_group"] = id_group.GetValue();
    e.customArgs["id_subgroup"] = id_subgroup.GetValue();
    e.customArgs["id_size"] = id_size.GetValue();
    e.customArgs["id_trademark"] = id_trademark.GetValue();
    e.customArgs["id_trademarkModel"] = id_trademarkModel.GetValue();
    e.customArgs["id_color"] = id_color.GetValue();
    e.customArgs["nameCodigoItemFilter"] = nameCodigoItemFilter.GetText();
}

function SalesQuotationExteriorDetail_OnBeginCallback(s, e) {
    e.customArgs["id_invoice"] = $("#id_invoice").val();
    e.customArgs["id_Buyer"] = id_buyer.GetValue();
    e.customArgs["busquedaNombre"] = bsNombreExtranjero.GetChecked();
    e.customArgs["nameItemFilter"] = nameItemFilter.GetText();
    e.customArgs["sizeBegin"] = idSizeBegin.GetValue();
    e.customArgs["sizeEnd"] = idSizeEnd.GetValue();

    e.customArgs["id_inventoryLine"] = id_inventoryLine.GetValue();
    e.customArgs["id_itemType"] = id_itemType.GetValue();
    e.customArgs["id_itemTypeCategory"] = id_itemTypeCategory.GetValue();
    e.customArgs["id_group"] = id_group.GetValue();
    e.customArgs["id_subgroup"] = id_subgroup.GetValue();
    e.customArgs["id_size"] = id_size.GetValue();
    e.customArgs["id_trademark"] = id_trademark.GetValue();
    e.customArgs["id_trademarkModel"] = id_trademarkModel.GetValue();
    e.customArgs["id_color"] = id_color.GetValue();
    e.customArgs["nameCodigoItemFilter"] = nameCodigoItemFilter.GetText();

    //debugger;
    validateFirstExistMetricUnitInvoice(null, null);

    //debugger;
    if (e.command == 'STARTEDIT') {
        
        id_metricUnitInvoice.SetEnabled(false);
        //btnNewDetail.SetEnabled(false);
        btnRefreshDetails.SetEnabled(false);
        $("#amount").val(0);
        $("#id_amountInvoice").val(0);
        // numBoxesValueChanged(s, e)        
        var rowKey = s.GetRowKey(s.GetFocusedRowIndex());

    }
    if (e.command == 'ADDNEWROW') {
        //  e.customArgs["id_metricUnitInvoice"] = ASPxClientControl.GetControlCollection().GetByName("id_metricUnitInvoice");
        id_metricUnitInvoice.SetEnabled(false);
        //btnNewDetail.SetEnabled(false);
        btnRefreshDetails.SetEnabled(false);
        $("#amount").val(0);
        $("#id_amountInvoice").val(0);
        $("#id_metricUnitInvoiceDetail").val(0);
    }
    if (e.command == 'UPDATEEDIT') {

        id_metricUnitInvoice.SetEnabled(true);
        //btnNewDetail.SetEnabled(true);
        validateFirstExistMetricUnitInvoice(null, null);
        btnRefreshDetails.SetEnabled(true);
        $("#amount").val(0);
        $("#id_amountInvoice").val(0);

        e.customArgs["id_metricUnitInvoiceDetail"] = $("#id_metricUnitInvoiceDetail").val();
    }
    if (e.command == 'DELETEROW') {
        id_metricUnitInvoice.SetEnabled(true);
        validateFirstExistMetricUnitInvoice(null, null);
        //btnNewDetail.SetEnabled(true);
        btnRefreshDetails.SetEnabled(true);
    }

    if (e.command == 'CANCELEDIT') {
        id_metricUnitInvoice.SetEnabled(true);
        validateFirstExistMetricUnitInvoice(null, null);
        btnRefreshDetails.SetEnabled(true);
    }
}

// TabIndex
function DevInvoiceDetail_foreignName_Init(s, e){
    var foreignName = s;
    foreignName.inputElement.tabIndex = -1;
    var _selectItemAux = id_item.GetSelectedItem();
    if (_selectItemAux !== undefined && _selectItemAux !== null) {
        foreignName.SetText(_selectItemAux.GetColumnText("foreignName"));
    }
}

function DevInvoiceDetail_descriptionCustomer_Init(s, e) {
    var aDescriptionCustomer = s.GetText();
    if (aDescriptionCustomer === "" || aDescriptionCustomer === null) {
        s.SetValue(foreignName.GetText());
    }
}

function DevInvoiceDetail_masterCode_Init(s, e) {
    var masterCode = s;
    masterCode.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_auxCode_Init(s, e) {
    var auxCode = s;
    auxCode.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_proformaUsedNumBoxes_Init(s, e) {
    var proformaUsedNumBoxes = s;
    proformaUsedNumBoxes.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_proformaPendingNumBoxes_Init(s, e) {
    var proformaPendingNumBoxes = s;
    proformaPendingNumBoxes.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_netWeight_Init(s, e) {
    var netWeight = s;
    netWeight.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_glaseo_Init(s, e) {
    var glaseo = s;
    glaseo.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_amountDisplay_Init(s, e) {
    var glaseo = s;
    glaseo.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_amountInvoiceDisplay_Init(s, e) {
    var amountInvoiceDisplay = s;
    amountInvoiceDisplay.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_totalPriceWithoutTax_Init(s, e) {
    var totalPriceWithoutTax = s;
    totalPriceWithoutTax.inputElement.tabIndex = -1;
}
function DevInvoiceDetail_proformaNumBoxesPlusMinus_Init(s, e) {
    var proformaNumBoxesPlusMinus = s;
    proformaNumBoxesPlusMinus.inputElement.tabIndex = -1;
}
// Generic Print
function PrintInvoiceGeneric(idsInvoice) {

    if (typeof idsInvoice === 'undefined' || idsInvoice === null || idsInvoice.length == 0) {
        return;
    }


    for (var i = 0; i < idsInvoice.length; i++) {

        data = { id_inv: idsInvoice[i] };

        $.ajax({
            url: 'SalesQuotationExterior/PrintSalesQuotationExteriorReport',
            data: data,
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {

                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        var _nameWindow = Math.round(Math.random() * 10000000000000) + "_prt";
                        newWindow = window.open(url, _nameWindow, 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }



}


function personValidation(s, e) {


    //var _id_buyer = ASPxClientControl.GetControlCollection().GetByName("id_buyer").GetValue();
    var _id_buyer = id_buyer.GetValue();
    if (e.value == null && id_buyer != null) {
        //e.errorText = "Debe seleccionar"
        //e.isValid = false;
        e.value = _id_buyer;
    }

    return e.value;

}


//  funciones Default

function init() {

}

$(function () {


    var chkReadyStateMain = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyStateMain);
            UpdateButtons();
            SetCollapseButton();
        }
    }, 100);


    init();
});

function SetCollapseButton() {

    //btnCollapse
}

