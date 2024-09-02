
//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeAutorizationDateValidation(s, e) {
    OnRangeDateValidation(e, startAuthorizationDate.GetValue(), endAuthorizationDate.GetValue(), "Rango de Fecha no válido");
}


//BOTONES PANTALLA FILTROS


function btnSearch_click(s, e) {
    var data = $("#formFilterSalesQuotation").serialize();

    if (data != null) {
        $.ajax({
            url: "SalesQuotation/SalesQuotationResults",
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

function btnClear_click(s, e) {
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startAuthorizationDate.SetDate(null);
    endAuthorizationDate.SetDate(null);
    authorizationNumber.SetText("");
    accessKey.SetText("");
    items.ClearTokenCollection();

    id_customer.SetSelectedItem(null);
    id_employeeSeller.SetSelectedItem(null);

    items.ClearTokenCollection();
}

function AddNewSalesQuotation(s, e) {

    var data = {
        id: 0
    };

    showPage("SalesQuotation/SalesQuotationEditForm", data);
}

// GRIDVIEW PURCHASE ORDERS SELECTION

function SalesQuotationOnGridViewInit(s, e) {
    SalesQuotationUpdateTitlePanel();
}

function SalesQuotationOnGridViewSelectionChanged(s, e) {
    SalesQuotationUpdateTitlePanel();
}

function SalesQuotationOnGridViewEndCallback() {
    SalesQuotationUpdateTitlePanel();
}

function SalesQuotationUpdateTitlePanel() {
    var selectedFilteredRowCount = SalesQuotationGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesQuotations.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesQuotations.GetSelectedRowCount() - SalesQuotationGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesQuotations.GetSelectedRowCount() > 0 && gvSalesQuotations.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesQuotations.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvSalesQuotations.GetSelectedRowCount() === 1);
    btnApprove.SetEnabled(gvSalesQuotations.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvSalesQuotations.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvSalesQuotations.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvSalesQuotations.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvSalesQuotations.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvSalesQuotations.GetSelectedRowCount() === 1);
    btnPrint.SetEnabled(gvSalesQuotations.GetSelectedRowCount() === 1);

}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SalesQuotationGetSelectedFilteredRowCount() {
    return gvSalesQuotations.cpFilteredRowCountWithoutPage + gvSalesQuotations.GetSelectedKeysOnPage().length;
}

function SalesQuotationSelectAllRows() {
    gvSalesQuotations.SelectRows();
}

function SalesQuotationClearSelection() {
    gvSalesQuotations.UnselectRows();
}

// SALE QUOTATIONS ORDERS RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvSalesQuotations.GetSelectedFieldValues("id", function (values) {

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
                gvSalesQuotations.PerformCallback();
                // gvSalesQuotations.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewSalesQuotation(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesQuotation/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesQuotation/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesQuotation/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesQuotation/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("SalesQuotation/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {

}

function SaleQuoatationsGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvSalesQuotations.GetRowKey(e.visibleIndex)
        };
        showPage("SalesQuotation/SalesQuotationEditForm", data);
    }
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

