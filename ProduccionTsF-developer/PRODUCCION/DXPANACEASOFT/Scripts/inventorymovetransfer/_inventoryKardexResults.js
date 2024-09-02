
// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

function Print(s, e) {
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {

}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryMoveDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryMoveDetails.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryMoveDetails.GetSelectedRowCount() > 0 && gvInventoryMoveDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //}

    //btnRemoveDetail.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);

    //btnCopy.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() === 1);
    //btnApprove.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvInventoryMoveDetails.GetSelectedRowCount() === 1);
    //btnPrint.SetEnabled(gvPurchaseOrders.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvInventoryMoveDetails.cpFilteredRowCountWithoutPage + gvInventoryMoveDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvInventoryMoveDetails.SelectRows();
}

function UnSelectRows() {
    gvInventoryMoveDetails.UnselectRows();
}

//function OnClickEditInventoryMove(s, e) {
//    var data = {
//        id: gvInventoryMoveDetails.GetRowKey(e.visibleIndex)
//    };
//    showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
//}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});