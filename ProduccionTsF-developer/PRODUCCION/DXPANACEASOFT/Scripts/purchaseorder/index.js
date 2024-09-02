//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

// FILTERS FORM ACTIONS

function btnSearch_click(s, e) {

    var data = $("#formFilterPurchaseOrder").serialize();

    if (data != null) {
        $.ajax({
            url: "PurchaseOrder/PurchaseOrderResults",
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

    id_documentState.SetSelectedItem(null);
    number.SetText("");
    //reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    //startAuthorizationDate.SetDate(null);
    //endAuthorizationDate.SetDate(null);
    //authorizationNumber.SetText("");
    //accessKey.SetText("");
    items.ClearTokenCollection();

    id_provider.SetSelectedItem(null);
    id_buyer.SetSelectedItem(null);
    id_priceList.SetSelectedItem(null);
    id_certification.SetSelectedItem(null);
    id_paymentTerm.SetSelectedItem(null);
    id_paymentMethod.SetSelectedItem(null);
    filterLogistic.SetChecked(true);
}

function AddNewItemManual(s, e) {
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
}

function AddNewItemFromPurchaseRequest(s, e) {
    $.ajax({
        url: "PurchaseOrder/PurchaseRequestDetailsResults",
        type: "post",
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

    event.preventDefault();
}

function btnRefreshLPThird_click(s, e) {
    $.ajax({
        url: "PurchaseOrder/UpdatePriceListThird",
        type: "post",
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
        },
        complete: function () {
            hideLoading();
        }
    });
    event.preventDefault();
}

// COMMON GRIDVIEW AUXILIARS FUNCTIONS

function GetGridViewSelectedRows(gv, key) {
    var selectedRows = [];
    gv.GetSelectedFieldValues(key, function (values) {
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
    });
    return selectedRows;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvPurchaseOrders.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: url,
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
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvPurchaseOrders.PerformCallback();
               // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewItemManual(s, e);
}

function CopyDocument(s, e) {
    gvPurchaseOrders.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("PurchaseOrder/PurchaseOrderCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseOrder/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseOrder/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseOrder/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseOrder/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchaseOrder/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}


function Print(s, e) {

    gvPurchaseOrders.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }


         
        //   var id = $("#id_order").val();
        var id = selectedRows[0];

        var data = { ReportName: "PurchaseOrdersReport", ReportDescription: "Orden Compra", ListReportParameter: [] };
        if (id !== 0 && id !== null) {
            var ids = [id];
            $.ajax({
                url: 'PurchaseOrder/PurchaseOrdersReport?id=' + id,
                data: data,
                async: true,
                cache: false,
                type: 'POST',
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                     

                    try {
                        if (response.isvalid) {
                            var reportModel = response.reportModel;
                            var url = 'Report/Index?reportModel=' + reportModel;
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
    

    });

}

function PurchaseOrdersGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var aId = gvPurchaseOrders.GetRowKey(e.visibleIndex);
        $.ajax({
            url: 'PurchaseOrder/LockedDocument',
            data: {
                id_document: aId,
                nameDocument: "Orden de Compra",
                code_sourceLockedDocument: "OC",
                namesourceLockedDocument: "Orden de Compra"
            },
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.Code !== 0) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                    return;
                }

                var data = {
                    id: aId
                };
                showPage("PurchaseOrder/FormEditPurchaseOrder", data);

            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

// GRIDVIEW PURCHASE ORDERS SELECTION

function OnRowDoubleClick(s, e) {
    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("PurchaseOrder/FormEditPurchaseOrder", { id: value });
    });
}

function PurchaseOrdersOnGridViewInit(s, e) {
    PurchaseOrdersUpdateTitlePanel();
}

function PurchaseOrdersOnGridViewSelectionChanged(s, e) {
    PurchaseOrdersUpdateTitlePanel();
}

function PurchaseOrdersOnGridViewEndCallback() {
    PurchaseOrdersUpdateTitlePanel();
}

function PurchaseOrdersUpdateTitlePanel() {
    var selectedFilteredRowCount = PurchaseOrdersGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrders.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrders.GetSelectedRowCount() - PurchaseOrdersGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrders.GetSelectedRowCount() > 0 && gvPurchaseOrders.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrders.GetSelectedRowCount() > 0);
    //}



    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(false);

    btnCopy.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() === 1);
    //btnApprove.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() === 1);
    //btnPrint.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() === 1);

}

function PurchaseOrdersGetSelectedFilteredRowCount() {
    return gvPurchaseOrders.cpFilteredRowCountWithoutPage + gvPurchaseOrders.GetSelectedKeysOnPage().length;
}

function PurchaseOrdersSelectAllRows() {
    gvPurchaseOrders.SelectRows();
}

function PurchaseOrdersClearSelection() {
    gvPurchaseOrders.UnselectRows();
}

// GRIDVIEW PURCHASE REQUEST RESULTS ACTIONS

function GenerateOrder(s, e) {

    gridMessageErrorPurchaseRequest.SetText("");
    $("#GridMessageErrorPurchaseRequest").hide();

    gvPurchaseRequestDetails.GetSelectedFieldValues("id"/*"id_purchaseRequest;id_item"*/, function (values) {
        
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var data = {
            requestDetails: selectedRows,
            id: 0
        };

        //for (var k = 0; k < selectedRows.length; k++) {
        //    data.requestDetails.push({
        //        id_purchaseRequest: selectedRows[k][0],
        //        id_item: selectedRows[k][1]
        //    });
        //}


        $.ajax({
            url: "PurchaseOrder/ValidateSelectedRowsPurchaseRequest",
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
                
            },
            success: function (result) {
                //resultFunction = result.enabledBtnGenerateLot;
                if (result.Message == "OK") {
                    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
                } else {
                    gridMessageErrorPurchaseRequest.SetText(result.Message);
                    $("#GridMessageErrorPurchaseRequest").show();
                    hideLoading();
                }
            },
            complete: function () {
                //hideLoading();
                //gvProductionLotReceptions.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

        
    });
}

// GRIDVIEW PURCHASE REQUEST RESULTS SELECTION

function PurchaseRequestDetailsOnGridViewInit(s, e) {
    PurchaseRequestDetailsUpdateTitlePanel();
}

function PurchaseRequestDetailsOnGridViewSelectionChanged(s, e) {
    PurchaseRequestDetailsUpdateTitlePanel();
}

function PurchaseRequestDetailsOnGridViewEndCallback() {
    PurchaseRequestDetailsUpdateTitlePanel();
}

function PurchaseRequestDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = PurchaseRequestDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseRequestDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseRequestDetails.GetSelectedRowCount() - PurchaseRequestDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseRequestDetails.GetSelectedRowCount() > 0 && gvPurchaseRequestDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseRequestDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateOrder.SetEnabled(gvPurchaseRequestDetails.GetSelectedRowCount() > 0);
}

function PurchaseRequestDetailsGetSelectedFilteredRowCount() {
    return gvPurchaseRequestDetails.cpFilteredRowCountWithoutPage + gvPurchaseRequestDetails.GetSelectedKeysOnPage().length;
}

function PurchaseRequestDetailsSelectAllRow() {
    gvPurchaseRequestDetails.SelectRows();
}

function PurchaseRequestDetailsClearSelection() {
    gvPurchaseRequestDetails.UnselectRows();
}

// MASTER DETAILS FUNCTIONS 

function PurchaseOrderResultsDetailViewDetails_BeginCallback(s, e) {
    e.customArgs["id_purchaseOrder"] = $("#id_purchaseOrder").val();
}

//BY GRAMMAGE
function PurchaseOrderResultsDetailViewDetails_BeginCallbackBG(s, e) {
    e.customArgs["id_purchaseOrder"] = $("#id_purchaseOrder").val();
}

// MAIN FUNCTIONS

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });

    $("#btnAdvancedFilter").click(function (event) {
        popupAdvancedFilter.PerformCallback();
        popupAdvancedFilter.Show();
    });

}

$(function () {
    init();
});



