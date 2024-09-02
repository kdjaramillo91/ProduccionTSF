
// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvInventoryMoves.GetSelectedFieldValues("id", function (values) {

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
                gvInventoryMoves.Refresh();
                // gvInventoryMoves.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    var codeDocumentType = $("#codeDocumentTypefilter").val();
    AddNewInventoryMoveGeneral(codeDocumentType);
}

function CopyDocument(s, e) {
    gvInventoryMoves.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("InventoryMoveTransfer/InventoryMoveCopy", { id: values[0] });
        }
    });
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("InventoryMoveTransfer/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("InventoryMoveTransfer/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("InventoryMoveTransfer/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("InventoryMoveTransfer/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("InventoryMoveTransfer/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {
}

function Print(s, e) {
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs['code'] = $("#codeDocumentTypefilter").val();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryMoves.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryMoves.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryMoves.GetSelectedRowCount() > 0 && gvInventoryMoves.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryMoves.GetSelectedRowCount() > 0);
    //}

    //btnRemoveDetail.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);

    btnCopy.SetEnabled(gvInventoryMoves.GetSelectedRowCount() === 1);
    btnApprove.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvInventoryMoves.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvInventoryMoves.GetSelectedRowCount() === 1);
    //btnPrint.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvInventoryMoves.cpFilteredRowCountWithoutPage + gvInventoryMoves.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvInventoryMoves.SelectRows();
}

function UnSelectRows() {
    gvInventoryMoves.UnselectRows();
}

function OnClickEditInventoryMove(s, e) {
    var data = {
        id: gvInventoryMoves.GetRowKey(e.visibleIndex)
    };
    showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});