// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionExpense.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionExpense.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvProductionExpense.PerformCallback({ ids: selectedRows });
            gvProductionExpense.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvProductionExpense.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvProductionExpense.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvProductionExpense.AddNewRow();
            keyToCopy = 0;

        }
    });
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

    var text = "Total de elementos seleccionados: <b>" + gvProductionExpense.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionExpense.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionExpense.GetSelectedRowCount() > 0 && gvProductionExpense.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionExpense.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvProductionExpense.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionExpense.cpFilteredRowCountWithoutPage + gvProductionExpense.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionExpense.SelectRows();
}

function UnselectAllRows() {
    gvProductionExpense.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});