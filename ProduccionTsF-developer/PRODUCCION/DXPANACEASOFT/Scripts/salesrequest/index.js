//Validation 

//function OnValidation(s, e) {
//    e.isValid = true;
//}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}


// FILTERS FORM ACTIONS

function OnClickSearchSalesRequest(s, e) {

    var data = $("#formFilterSalesRequest").serialize();

    if (data !== null) {
        $.ajax({
            url: "SalesRequest/SalesRequestsResults",
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

}

function OnClickClearFiltersSalesRequest(s, e) {
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);

    id_customer.SetSelectedItem(null);
    id_priceList.SetSelectedItem(null);

    //startAuthorizationDate.SetDate(null);
    //endAuthorizationDate.SetDate(null);
    //authorizationNumber.SetText("");
    //accessKey.SetText("");

    items.ClearTokenCollection();

    
}

function AddNewManual(s, e) {
    var data = {
        id: 0,
        ids_quotationsDetails: []
    };

    showPage("SalesRequest/FormEditSalesRequest", data);
}

function AddNewFromSalesQuotation(s, e) {
    $.ajax({
        url: "SalesRequest/SalesQuotationsDetailsResults",
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
}

// GRIDVIEW SALES REQUEST RESULTS ACTIONS

function GenerateSalesRequest(s, e) {
    gridMessageErrorSalesQuotation.SetText("");
    $("#GridMessageErrorSalesQuotation").hide();

    gvSalesQuotationsDetails.GetSelectedFieldValues("id"/*"id_salesQuotation;id_item"*/, function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var data = {
            quotationsDetails: selectedRows,
            id: 0
        };

        //var data = {
        //    id: 0,
        //    quotationsDetails: []
        //};

        //for (var k = 0; k < selectedRows.length; k++) {
        //    data.quotationsDetails.push({
        //        id_salesQuotation: selectedRows[k][0],
        //        id_item: selectedRows[k][1]
        //    });
        //}
        $.ajax({
            url: "SalesRequest/ValidateSelectedRowsSalesQuotation",//ValidateSelectedRowsPurchaseRequest",
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
                    showPage("SalesRequest/FormEditSalesRequest", data);
                } else {
                    gridMessageErrorSalesQuotation.SetText(result.Message);
                    $("#GridMessageErrorSalesQuotation").show();
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

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvSalesRequests.GetSelectedFieldValues("id", function (values) {

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
                gvSalesRequests.PerformCallback();
                gvSalesRequests.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewManual(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesRequest/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesRequest/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesRequest/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesRequest/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesRequest/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function SalesRequestsGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvSalesRequests.GetRowKey(e.visibleIndex),
            ids_quotationsDetails: []
        };
        showPage("SalesRequest/FormEditSalesRequest", data);
    }
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

// GRIDVIEW SALES REQUEST SELECTION

function SalesRequestsOnGridViewInit(s, e) {
    SalesRequestsUpdateTitlePanel();
}

function SalesRequestsOnGridViewSelectionChanged(s, e) {
    SalesRequestsUpdateTitlePanel();
}

function SalesRequestsOnGridViewEndCallback() {
    SalesRequestsUpdateTitlePanel();
}

function SalesRequestsUpdateTitlePanel() {
    var selectedFilteredRowCount = SalesRequestsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesRequests.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesRequests.GetSelectedRowCount() - SalesRequestsGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesRequests.GetSelectedRowCount() > 0 && gvSalesRequests.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesRequests.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvSalesRequests.GetSelectedRowCount() === 1);
    btnApprove.SetEnabled(gvSalesRequests.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvSalesRequests.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvSalesRequests.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvSalesRequests.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvSalesRequests.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvSalesRequests.GetSelectedRowCount() === 1);
    btnPrint.SetEnabled(gvSalesRequests.GetSelectedRowCount() === 1);

}

function SalesRequestsGetSelectedFilteredRowCount() {
    return gvSalesRequests.cpFilteredRowCountWithoutPage + gvSalesRequests.GetSelectedKeysOnPage().length;
}

function SalesRequestsSelectAllRows() {
    gvSalesRequests.SelectRows();
}

function SalesRequestsClearSelection() {
    gvSalesRequests.UnselectRows();
}

// GRIDVIEW PURCHASE REQUEST RESULTS ACTIONS

function GenerateOrder(s, e) {

    gvSalesQuotationsDetails.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var data = {
            id: 0,
            ids_quotationsDetails: selectedRows
        };

        showPage("SalesRequest/FormEditSalesRequest", data);
    });
}

// GRIDVIEW PURCHASE REQUEST RESULTS SELECTION

function SalesQuotationsDetailsOnGridViewInit(s, e) {
    SalesQuotationDetailsUpdateTitlePanel();
}

function SalesQuotationsDetailsOnGridViewSelectionChanged(s, e) {
    SalesQuotationDetailsUpdateTitlePanel();
}

function SalesQuotationsDetailsOnGridViewEndCallback() {
    SalesQuotationDetailsUpdateTitlePanel();
}

function SalesQuotationDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = SalesQuotationDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesQuotationsDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesQuotationsDetails.GetSelectedRowCount() - SalesQuotationDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesQuotationsDetails.GetSelectedRowCount() > 0 && gvSalesQuotationsDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesQuotationsDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateRequest.SetEnabled(gvSalesQuotationsDetails.GetSelectedRowCount() > 0);
}

function SalesQuotationDetailsGetSelectedFilteredRowCount() {
    return gvSalesQuotationsDetails.cpFilteredRowCountWithoutPage + gvSalesQuotationsDetails.GetSelectedKeysOnPage().length;
}

function SalesQuotationsDetailsSelectAllRow() {
    gvSalesQuotationsDetails.SelectRows();
}

function SalesQuotationsDetailsClearSelection() {
    gvSalesQuotationsDetails.UnselectRows();
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
}

$(function () {
    init();
});



