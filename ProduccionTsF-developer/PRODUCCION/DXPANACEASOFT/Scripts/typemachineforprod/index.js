// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvTypeMachineForProd.AddNewRow();
}

function RemoveItems(s, e) {
    gvTypeMachineForProd.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvTypeMachineForProd.PerformCallback({ ids: selectedRows });
            gvTypeMachineForProd.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvTypeMachineForProd.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTypeMachineForProd.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTypeMachineForProd.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvTypeMachineForProd.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTypeMachineForProd.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTypeMachineForProd.GetSelectedRowCount() > 0 && gvTypeMachineForProd.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTypeMachineForProd.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTypeMachineForProd.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvTypeMachineForProd.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvTypeMachineForProd.cpFilteredRowCountWithoutPage + gvTypeMachineForProd.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTypeMachineForProd.SelectRows();
}

function UnselectAllRows() {
    gvTypeMachineForProd.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});