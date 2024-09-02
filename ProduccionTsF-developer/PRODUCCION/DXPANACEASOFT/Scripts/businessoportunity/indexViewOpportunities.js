
// FILTER FORM BUTTONS

function btnSearch_click(s, e) {
    var data = $("#formFilterBusinessOportunityViewOpportunities").serialize();

    if (data !== null) {
        $.ajax({
            url: "BusinessOportunity/BusinessOportunityViewOpportunitiesResults",
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

    id_documentType.SetSelectedItem(null);
    address.SetText("");

    persons.ClearTokenCollection();
    startStartDate.SetDate(null);
    endStartDate.SetDate(null); 

    executivePersons.ClearTokenCollection();
    startEndDate.SetDate(null);
    endEndDate.SetDate(null);

    documentTypePhases.ClearTokenCollection();
    id_logicalOperator.SetSelectedItem(null);
    amount.SetValue(0);

    id_logicalOperatorPC.SetSelectedItem(null);
    percentClosure.SetValue(0);
    itemSizes.ClearTokenCollection();
}


// GRIDVIEW BUSINESS OPORTUNITIES RESULTS ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {

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
                gvBusinessOportunities.PerformCallback();
                // gvBusinessOportunities.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    //AddNewItemManual(s, e);
}

function CopyDocument(s, e) {
    //gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {
    //    if (values.length > 0) {
    //        showPage("PurchaseOrder/PurchaseOrderCopy", { id: values[0] });
    //    }
    //});
}

function ApproveDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/ApproveDocuments");
    //}, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/AutorizeDocuments");
    //}, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/ProtectDocuments");
    //}, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/CancelDocuments");
    //}, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchaseOrder/RevertDocuments");
    //}, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
}

function PrintResultQueryPhase(s, e) {
    showPage("BusinessOportunity/BusinessOportunityViewOpportunitiesResultsReport", null);
    //gvBusinessOportunities.GetSelectedFieldValues("id", function (values) {

    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "PurchaseOrder/PurchaseOrdersReport",
    //        type: "post",
    //        data: { ids: selectedRows },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            $("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            hideLoading();
    //        }
    //    });

    //});

}

function BusinessOportunityGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvBusinessOportunities.GetRowKey(e.visibleIndex)
        };
        showPage("BusinessOportunity/FormEditBusinessOportunity", data);
    }
}

// GRIDVIEW BUSINESS OPORTUNITY SELECTION

function OnRowDoubleClick(s, e) {
    //s.GetRowValues(e.visibleIndex, "id", function (value) {
    //    showPage("PurchaseOrder/FormEditPurchaseOrder", { id: value });
    //});
}

function BusinessOportunityOnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function BusinessOportunityOnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function BusinessOportunityOnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvBusinessOportunities.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBusinessOportunities.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvBusinessOportunities.GetSelectedRowCount() > 0 && gvBusinessOportunities.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBusinessOportunities.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);
    btnApprove.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);
    btnPrint.SetEnabled(gvBusinessOportunities.GetSelectedRowCount() === 1);

}

function GetSelectedFilteredRowCount() {
    return gvBusinessOportunities.cpFilteredRowCountWithoutPage + gvBusinessOportunities.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function BusinessOportunitySelectAllRows() {
    gvBusinessOportunities.SelectRows();
}

function BusinessOportunityClearSelection() {
    gvBusinessOportunities.UnselectRows();
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