
function GenerateInventoryMoveTransferEntry(s, e) {
    //// 
    //var natureMoveTmp = $("#natureMoveFilter").val();
    //var data = {
    //    id: 0,
    //    code: "34",//Ingreso Por Transferencia
    //    inventoryMoveDetailTransferExitsDetails: [],
    //    natureMoveType: natureMoveTmp
    //};
    //for (var i = 0; i < selectedInventoryMoveDetailTransferExitsRows.length; i++) {
    //    data.inventoryMoveDetailTransferExitsDetails.push(selectedInventoryMoveDetailTransferExitsRows[i]);
    //}

    //showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
    showLoading();
    gvInventoryMoveDetailTransferExits.GetSelectedFieldValues("id", function (values) {
        var selectedInventoryMoveDetails = [];
        var data = {
            id: 0,
            code: "34",//Ingreso Por Transferencia
            inventoryMoveDetailTransferExitsDetails: [],
            natureMoveType: natureMoveTmp
        };
        for (var i = 0; i < values.length; i++) {
            data.inventoryMoveDetailTransferExitsDetails.push(values[i]);
        }

        showPage("InventoryMoveTransfer/InventoryMoveEditFormPartial", data);
    });


}

// SELECTIONS

function OnGridViewInventoryMoveDetailTransferExitsInit(s, e) {
    UpdateTitlePanelInventoryMoveDetailTransferExits();
}

function OnGridViewInventoryMoveDetailTransferExitsSelectionChanged(s, e) {
    UpdateTitlePanelInventoryMoveDetailTransferExits();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

var selectedInventoryMoveDetailTransferExitsRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
    selectedInventoryMoveDetailTransferExitsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedInventoryMoveDetailTransferExitsRows.push(values[i]);
    }
}

function OnGridViewInventoryMoveDetailTransferExitsEndCallback(s, e) {
    UpdateTitlePanelInventoryMoveDetailTransferExits();
}

function UpdateTitlePanelInventoryMoveDetailTransferExits() {
    var selectedFilteredRowCount = GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryMoveDetailTransferExits.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryMoveDetailTransferExits.GetSelectedRowCount() - GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    console.log(gvInventoryMoveDetailTransferExits.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0 && gvInventoryMoveDetailTransferExits.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0);
    //}

    btnGenerateMoveTransferEntry.SetEnabled(gvInventoryMoveDetailTransferExits.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredInventoryMoveDetailTransferExitsRowCount() {
    return gvInventoryMoveDetailTransferExits.cpFilteredRowCountWithoutPage + gvInventoryMoveDetailTransferExits.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewInventoryMoveDetailTransferExitsClearSelection() {
    gvInventoryMoveDetailTransferExits.UnselectRows();
}

function GridViewPurchaseInventoryMoveDetailTransferExitsSelectAllRow() {
    gvInventoryMoveDetailTransferExits.SelectRows();
}

function inti() {
    
}

$(function () {

});